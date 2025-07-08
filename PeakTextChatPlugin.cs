using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace PeakTextChat;

[BepInPlugin("com.borealityy.peaktextchat", "PeakTextChat", MyPluginInfo.PLUGIN_VERSION)]
public class PeakTextChatPlugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
        
    Harmony harmony;

    public static ConfigEntry<float> configFontSize;
    public static ConfigEntry<float> configMessageFadeDelay;
    public static ConfigEntry<float> configFadeDelay;
    public static ConfigEntry<float> configHideDelay;
    public static ConfigEntry<KeyCodeShort> configKey;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"PeakTextChat is loaded!");

        configKey = Config.Bind<KeyCodeShort>(
                                "Display",
                                "ChatKey",
                                KeyCodeShort.Slash,
                                "The key that activates typing in chat"
                            );

        configFontSize = Config.Bind<float>(
                                "Display",
                                "ChatFontSize",
                                20,
                                "Size of the chat's text"
                            );

        configFadeDelay = Config.Bind<float>(
                                "Display",
                                "ChatFadeDelay",
                                15,
                                "How long before the chat fades out (a negative number means never)"
                            );


        configHideDelay = Config.Bind<float>(
                                "Display",
                                "ChatHideDelay",
                                40,
                                "How long before the chat hides completely (a negative number means never)"
                            );

        configMessageFadeDelay = Config.Bind<float>(
                                    "Display",
                                    "ChatMessageHideDelay",
                                    40,
                                    "How long before a chat message disappears (a negative number means never)"
                                );

        harmony = new Harmony("com.borealityy.peaktextchat");
        harmony.PatchAll(typeof(CharacterPatch));
        harmony.PatchAll(typeof(StaminaBarPatch));
        harmony.PatchAll(typeof(GUIManagerPatch));
        harmony.PatchAll(typeof(InputBlockingPatches));
    }

    private void OnDestroy() {
        if (TextChatDisplay.instance != null)
            GameObject.Destroy(TextChatDisplay.instance.gameObject);
        if (GUIManagerPatch.textChatCanvas != null)
            GameObject.Destroy(GUIManagerPatch.textChatCanvas);
            
        TextChatManager.CleanupObjects();
        StaminaBarPatch.CleanupObjects();

        harmony.UnpatchSelf();
    }
}
