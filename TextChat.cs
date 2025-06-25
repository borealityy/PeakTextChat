using HarmonyLib;
using PeakTextChat;
using UnityEngine;
using UnityEngine.UI;

[HarmonyPatch(typeof(GUIManager),nameof(GUIManager.UpdateItemPrompts))]
public static class GUIManagerTextChat {
    static Canvas textChatCanvas;

    [HarmonyPostfix]
    public static void Postfix() {
        Plugin.Logger.LogInfo("portuguese");
        if (textChatCanvas == null && GUIManager.instance != null) {
            var guiManager = GUIManager.instance;
            var canvasObj = new GameObject("TextChatCanvas");
            canvasObj.transform.SetParent(guiManager.transform);
            textChatCanvas = canvasObj.AddComponent<Canvas>();
            textChatCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            var imageObj = new GameObject("Image");
            var rectTransform = imageObj.AddComponent<RectTransform>();
            rectTransform.SetParent(canvasObj.transform,false);
            rectTransform.anchorMax = Vector2.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.pivot = Vector2.zero;
            rectTransform.anchoredPosition = new Vector2(30,100);
            rectTransform.sizeDelta = new Vector2(350,250);
            var bgImage = imageObj.AddComponent<Image>();
            bgImage.color = new Color(0,0,0,0.6f);
        }
    }
}