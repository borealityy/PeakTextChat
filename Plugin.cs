using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace PeakTextChat;

[BepInPlugin("com.borealityy.peaktextchat", "PeakTextChat", MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
        
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        Harmony harmony = new Harmony("com.borealityy.peaktextchat");
        harmony.PatchAll(typeof(CharacterPatch));
        harmony.PatchAll(typeof(GUIManagerPatch));
        harmony.PatchAll(typeof(TextChatBlockInput));
    }
}
