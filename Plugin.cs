using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace PeakTextChat;

[BepInPlugin("com.borealityy.peaktextchat", "PeakTextChat", MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
        
    Harmony harmony;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"PeakTextChat is loaded!");

        harmony = new Harmony("com.borealityy.peaktextchat");
        harmony.PatchAll(typeof(CharacterPatch));
        harmony.PatchAll(typeof(StaminaBarPatch));
        harmony.PatchAll(typeof(GUIManagerPatch));
        harmony.PatchAll(typeof(TextChatBlockInput));
    }

    private void OnDestroy() {
        if (TextChatDisplay.instance != null)
            GameObject.Destroy(TextChatDisplay.instance.gameObject);
        if (TextChatManager.instance != null)
            GameObject.Destroy(TextChatManager.instance);
        if (GUIManagerPatch.textChatCanvasObj != null)
            GameObject.Destroy(GUIManagerPatch.textChatCanvasObj);
        if (StaminaBarPatch.textChatDummyTransform != null)
            GameObject.Destroy(StaminaBarPatch.textChatDummyTransform.gameObject);

        harmony.UnpatchSelf();
    }
}
