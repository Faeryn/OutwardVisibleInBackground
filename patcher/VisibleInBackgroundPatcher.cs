using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using AssetsTools.NET;
using AssetsTools.NET.Extra;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using Mono.Cecil;

namespace VisibleInBackground {
	
	/**
	 * Note: This preloader patcher does not actually patch any assemblies. It works on the GlobalGameManagers asset using AssetsTools.NET.
	 */
	public class VisibleInBackgroundPatcher {
		private static ManualLogSource Log = Logger.CreateLogSource("VisibleInBackground");
		
		// Keep these in sync with the main plugin!
		public const string GUID = "faeryn.visibleinbackground";
		private const string DISPLAY_NAME = "Visible in Background";

		public static IEnumerable<string> TargetDLLs { get; } = new string[0];

		public static void Initialize() {
			Log.LogInfo("Checking GlobalGameManagers");
			ConfigFile config = new ConfigFile(Utility.CombinePaths(Paths.ConfigPath, GUID+".cfg"), false);
			ConfigEntry<bool> enabled = config.Bind(DISPLAY_NAME, DISPLAY_NAME, true, "The game is visible even in the background (requires restart)");
			bool modEnabled = enabled.Value;
			Log.LogInfo("GlobalGameManagers patch should be "+(modEnabled?"enabled":"disabled"));
			
			string ggmFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Outward_Data", "globalgamemanagers");
			AssetsManager am = new AssetsManager();
			Stream classdata = Assembly.GetExecutingAssembly().GetManifestResourceStream("VisibleInBackgroundPatcher.classdata.tpk");
			am.LoadClassPackage(classdata);
			AssetsFileInstance ggm = am.LoadAssetsFile(ggmFilePath, false);
			AssetsFile ggmFile = ggm.file;
			AssetsFileTable ggmTable = ggm.table;
			am.LoadClassDatabaseFromPackage(ggmFile.typeTree.unityVersion);
			
			AssetFileInfoEx playerSettings = ggmTable.GetAssetInfo(1);
			AssetTypeValueField playerSettingsBase = am.GetTypeInstance(ggmFile, playerSettings).GetBaseField();
			AssetTypeValue visibleInBackgroundValue = playerSettingsBase.Get("visibleInBackground").GetValue();
			bool visibleInBackground = visibleInBackgroundValue.AsBool();
			if (visibleInBackground==modEnabled) {
				Log.LogInfo("GlobalGameManagers is already patched, exiting");
				am.UnloadAllAssetsFiles();
				return;
			}

			if (!visibleInBackground) {
				// If we do not have a patched ggm yet (visibleInBackground==false), then (try to) create backup
				CreateBackup(ggmFilePath);
			}

			Log.LogInfo("Patching GlobalGameManagers (visibleInBackground="+modEnabled+")");
			visibleInBackgroundValue.Set(modEnabled);
			byte[] playerSettingsBaseBytes = playerSettingsBase.WriteToByteArray();
			
			AssetsReplacerFromMemory replacer = new AssetsReplacerFromMemory(0, playerSettings.index, (int)playerSettings.curFileType, 0xffff, playerSettingsBaseBytes);
			string ggmTempFilePath = ggmFilePath + ".tmp";
			using (AssetsFileWriter writer = new AssetsFileWriter(File.OpenWrite(ggmTempFilePath))) {
				ggmFile.Write(writer, 0, new List<AssetsReplacer> { replacer }, 0);
			}
			am.UnloadAllAssetsFiles();
			File.Delete(ggmFilePath);
			File.Move(ggmTempFilePath, ggmFilePath);
			Log.LogInfo("Finished patching GlobalGameManagers");
		}

		private static void CreateBackup(string file) {
			string backupPath = file + ".bak";
			if (File.Exists(backupPath)){
				return;
			}
			File.Copy(file, backupPath);
		}
	
		public static void Patch(AssemblyDefinition assembly) {
			
		}
	}
}