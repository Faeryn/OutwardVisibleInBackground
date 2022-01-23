using System;
using System.IO;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;

namespace BetterChat {
	[BepInPlugin(GUID, NAME, VERSION)]
	public class VisibleInBackground : BaseUnityPlugin {
		public const string GUID = "faeryn.visibleinbackground";
		public const string NAME = "VisibleInBackground";
		public const string VERSION = "1.1.1";
		private const string DISPLAY_NAME = "Visible in Background";
		internal static ManualLogSource Log;
		
		public static ConfigEntry<bool> Enabled;
		
		private bool restored = false;

		internal void Awake() {
			Log = this.Logger;
			Log.LogMessage($"Starting {NAME} {VERSION}");
			InitializeConfig();
		}

		private void InitializeConfig() {
			Enabled = Config.Bind(DISPLAY_NAME, DISPLAY_NAME, true, "The game is visible even in the background (requires restart)");
			Enabled.SettingChanged += (sender, args) => {
				if (!Enabled.Value && !restored) {
					RestoreFromBackup();
					restored = true;
				}
			};
		}

		private void RestoreFromBackup() {
			Log.LogInfo("Restoring original GlobalGameManagers");
			string ggmFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Outward_Data", "globalgamemanagers");
			string backupPath = ggmFilePath + ".bak";
			if (!File.Exists(backupPath)) {
				Log.LogInfo("Original GlobalGameManagers file doesn't exist. Assuming the mod is already disabled. If not: Don't panic, the patcher syncs this setting on launch.");
				return;
			}
			File.Delete(ggmFilePath);
			File.Copy(backupPath, ggmFilePath);
		}
	}
}