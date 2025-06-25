using System.Reflection;
using HarmonyLib;

namespace PeakTextChat;

[HarmonyPatch(typeof(GUIManager),nameof(GUIManager.UpdateWindowStatus))]
public static class TextChatBlockInput {
    private static readonly MethodInfo windowBlockingInput = AccessTools.PropertySetter(typeof(GUIManager),"windowBlockingInput");
    [HarmonyPostfix]
    public static void Postfix() {
        if (GUIManagerTextChat.isBlockingInput) {
            windowBlockingInput?.Invoke(GUIManager.instance,[ true ]);
        }
    }
}