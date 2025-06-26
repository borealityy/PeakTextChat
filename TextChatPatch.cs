using HarmonyLib;

namespace PeakTextChat;

[HarmonyPatch(typeof(Character),"Awake")]
public static class TextChatPatch {
    [HarmonyPostfix]
    public static void AwakePatch(Character __instance) {
        Plugin.Logger.LogInfo("fnaf");
        __instance.gameObject.AddComponent<TextChatManager>();
    }
}