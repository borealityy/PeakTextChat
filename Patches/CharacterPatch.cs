using HarmonyLib;

namespace PeakTextChat;

[HarmonyPatch(typeof(Character),"Awake")]
public static class CharacterPatch {
    [HarmonyPostfix]
    public static void AwakePatch(Character __instance) {
        __instance.gameObject.AddComponent<TextChatManager>();
    }
}