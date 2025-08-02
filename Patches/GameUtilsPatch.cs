using HarmonyLib;
using UnityEngine;

namespace PeakTextChat;

public static class GameUtilsPatch {
    [HarmonyPatch(typeof(GameUtils),"Awake")]
    [HarmonyPostfix]
    public static void AwakePatch(GameUtils __instance) {
        var textChatManagerObj = new GameObject("TextChatManager");
        textChatManagerObj.transform.SetParent(__instance.transform,false);
        textChatManagerObj.AddComponent<TextChatManager>();
    }
}