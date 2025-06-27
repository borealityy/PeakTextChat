using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using PeakTextChat;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PeakTextChat;

[HarmonyPatch(typeof(StaminaBar),"Start")]
public static class StaminaBarPatch {
    public static RectTransform textChatDummyTransform;

    [HarmonyPostfix]
    public static void Postfix(StaminaBar __instance) {
        if (GUIManager.instance != null) {
            var textChatDummyObj = new GameObject("TextChatPos");
            var parent = (RectTransform)__instance.transform.parent;
            parent.offsetMax = new Vector2(parent.offsetMax.x,1000);
            textChatDummyObj.transform.SetParent(parent);
            var transform = textChatDummyObj.AddComponent<RectTransform>();
            transform.SetAsFirstSibling();
            transform.sizeDelta = Vector2.zero;
            textChatDummyTransform = (RectTransform)textChatDummyObj.transform;
        }
    }
}


[HarmonyPatch(typeof(GUIManager),"Start")]
public static class GUIManagerPatch {
    public static GameObject textChatCanvasObj;
    public static TMP_FontAsset darumaDropOneFont;
    
    [HarmonyPostfix]
    public static void Postfix(GUIManager __instance) {
        var transform = __instance.transform;
        textChatCanvasObj = new GameObject("TextChatCanvas");
        textChatCanvasObj.transform.SetParent(transform,false);
        var textChatCanvas = textChatCanvasObj.AddComponent<Canvas>();
        textChatCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        
        var textChatCanvasScaler = textChatCanvas.gameObject.GetComponent<CanvasScaler>() ?? textChatCanvas.gameObject.AddComponent<CanvasScaler>();
        textChatCanvasScaler.referencePixelsPerUnit = 100;
        textChatCanvasScaler.matchWidthOrHeight = 1;
        textChatCanvasScaler.referenceResolution = new Vector2(1920,1080);
        textChatCanvasScaler.scaleFactor = 1;
        textChatCanvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        textChatCanvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        var textChatObj = new GameObject("TextChat");
        textChatObj.transform.SetParent(textChatCanvas.transform,false);
        textChatObj.AddComponent<TextChatDisplay>();
        var fogNotif = __instance.hudCanvas?.transform.Find("Notification/Fog")?.gameObject.GetComponent<TMP_Text>();
        if (fogNotif != null)
            darumaDropOneFont = fogNotif.font;
    }
}