using System.Reflection;
using HarmonyLib;

namespace PeakTextChat;

public static class InputBlockingPatches {
    private static readonly MethodInfo windowBlockingInput = AccessTools.PropertySetter(typeof(GUIManager),"windowBlockingInput");

    [HarmonyPatch(typeof(GUIManager),nameof(GUIManager.UpdateWindowStatus))]
    [HarmonyPostfix]
    public static void UpdateWindowStatusPatch() {
        if (TextChatDisplay.instance?.isBlockingInput == true) {
            windowBlockingInput?.Invoke(GUIManager.instance,[ true ]);
        }
    }

    [HarmonyPatch(typeof(CinemaCamera),"Update")]
    [HarmonyPrefix]
    public static bool UpdateCinemaCamPatch(CinemaCamera __instance) {
        if (GUIManager.instance?.windowBlockingInput == true && !__instance.on) {
            return false;
        }
        return true;
    }

    [HarmonyPatch(typeof(Character),"UpdateVariablesFixed")]
    [HarmonyPrefix]
    public static bool UpdateCharacterVariablesPatch(Character __instance) {
        if (GUIManager.instance?.windowBlockingInput == true)
            __instance.input.interactIsPressed = false;
        return true;
    }
}