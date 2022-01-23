# Visible In Background

A small mod that makes the game visible even when it's not focused (for multi-monitor setups). It requires Borderless window mode (does not work in Fullscreen).
Special thanks to **Sinai** for showing me the asset patcher solution.

### How to uninstall the mod:
Change the config entry `Visible in Background` to disabled, then exit the game. Now you can safely uninstall the mod.  
It's strongly recommended to use Outward Config Manager.  
If you changed the config file while the game wasn't running, you have to start it at least once for the setting to take effect.

The mod patches one of the game's asset files (`globalgamemanagers` in `OutwardGame_Data` folder) 
and creates a backup (`globalgamemanagers.bak`). If anything goes wrong, you can manually restore it.

**WARNING: Use the mod at your own risk!**  
There is a slight chance that the mod may break (and break your game) in various exciting ways, 
because of the aforementioned asset patching.

## Planned Features
- Possibly a less invasive solution

## Used 3rd party libraries and tools
- `AssetTools.NET`
- Parts of the code is based on `Raicuparta's unity-vr-patcher`
- classdata.tpk from `UABE`


## Changelog
### v1.1.1
- Minor backup restore fix

### v1.1.0
- Added enable/disable setting

### v1.0.0
- Initial release