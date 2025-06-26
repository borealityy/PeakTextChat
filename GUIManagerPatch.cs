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

[HarmonyPatch(typeof(GUIManager),"Start")]
public static class GUIManagerPatch {

    [HarmonyPostfix]
    public static void Postfix() {
        if (GUIManager.instance != null) {
            var textChatDisplayObj = new GameObject("TextChatCanvas");
            textChatDisplayObj.transform.SetParent(GUIManager.instance.transform);
            textChatDisplayObj.AddComponent<TextChatDisplay>();
        }
    }
}