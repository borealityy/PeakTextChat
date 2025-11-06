using System.Reflection;
using HarmonyLib;

namespace PeakTextChat;

public static class InputBlockingPatches {
    private static MethodInfo windowBlockingInput;

    [HarmonyPatch(typeof(GUIManager),nameof(GUIManager.UpdateWindowStatus))]
    [HarmonyPostfix]
    public static void UpdateWindowStatusPatch() {
        try {
            if (windowBlockingInput == null) {
                    windowBlockingInput = AccessTools.PropertySetter(typeof(GUIManager),"windowBlockingInput");
            }
            if (TextChatDisplay.instance?.isBlockingInput == true) {
                windowBlockingInput?.Invoke(GUIManager.instance,[ true ]);
            }
        } catch {}
    }

    [HarmonyPatch(typeof(CinemaCamera),"Update")]
    [HarmonyPrefix]
    public static bool UpdateCinemaCamPatch(CinemaCamera __instance) {
        if (IsInputBlocked() && !__instance.on) {
            return false;
        }
        return true;
    }

    [HarmonyPatch(typeof(Character),"UpdateVariablesFixed")]
    [HarmonyPrefix]
    public static bool UpdateCharacterVariablesPatch(Character __instance) {
        try {
            if (IsInputBlocked())
                __instance.input.interactIsPressed = false;
        } catch {
            PeakTextChatPlugin.Logger.LogError("update character variables patch failed!! YIPPEEEE");
        }
        return true;
    }

    public static bool IsInputBlocked() {
        try {
            return GUIManager.instance?.windowBlockingInput ?? false;
        } catch {
            PeakTextChatPlugin.Logger.LogError("window blocking input check failed!! YAY");
        }
        return false;
    }
}