using System.Reflection;
using HarmonyLib;
using PeakTextChat;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PeakTextChat;

[HarmonyPatch(typeof(GUIManager),nameof(GUIManager.UpdateItemPrompts))]
public static class GUIManagerTextChat {
    static Canvas textChatCanvas;
    static TMP_InputField inputField;

    public static bool isBlockingInput = false;

    [HarmonyPostfix]
    public static void Postfix() {
        if (textChatCanvas == null && GUIManager.instance != null) {
            SetupChatGUI();
        }
        if (Input.GetKeyDown(KeyCode.Slash) && inputField != null && EventSystem.current != null && !GUIManager.instance.windowBlockingInput) {
            EventSystem.current.SetSelectedGameObject(inputField.gameObject, null);
            inputField.OnPointerClick(new PointerEventData(EventSystem.current));
            isBlockingInput = true;
        }
    }

    static void SetupChatGUI() {
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

        inputField = CreateInputField();
        var inputFieldTransform = (RectTransform)inputField.transform;
        inputFieldTransform.anchorMin = Vector2.zero;
        inputFieldTransform.anchorMax = new Vector2(1,1);
        inputFieldTransform.pivot = new Vector2(0.5f,0);
        inputFieldTransform.offsetMin = Vector2.zero;
        inputFieldTransform.offsetMax = Vector2.zero;
        inputFieldTransform.SetParent(rectTransform,false);

        var bgImage = imageObj.AddComponent<Image>();
        bgImage.color = new Color(0,0,0,0.6f);

        inputField.onEndEdit.AddListener((e) => isBlockingInput = false);
    }

    static TMP_InputField CreateInputField() {
        var inputFieldObj = new GameObject("InputField");
        TMP_InputField inputField = inputFieldObj.AddComponent<TMP_InputField>();
        inputField.enabled = false;

        var inputFieldGraphic = inputField.gameObject.AddComponent<Image>();
        inputField.targetGraphic = inputFieldGraphic;
        inputFieldGraphic.color = new Color(0,0,0,0);

        var textAreaObj = new GameObject("Text Area");
        var textAreaTransform = textAreaObj.AddComponent<RectTransform>();
        textAreaTransform.anchorMax = Vector2.one;
        textAreaTransform.anchorMin = Vector2.zero;
        textAreaTransform.offsetMax = new Vector2(-10,-7);
        textAreaTransform.offsetMin = new Vector2(10,6);
        textAreaTransform.SetParent(inputField.transform,false);
        var textAreaMask = textAreaObj.AddComponent<RectMask2D>();
        textAreaMask.padding = new Vector4(-8,-5,-8,-5);
        
        var placeholderText = CreateText(textAreaTransform);
        placeholderText.color = new(0,0,0,0.6f);
        placeholderText.text = "Press / to chat";
        placeholderText.name = "Placeholder";

        var mainText = CreateText(textAreaTransform);
        mainText.color = new(0,0,0,1);

        inputField.textViewport = textAreaTransform;
        inputField.textComponent = mainText;
        inputField.placeholder = placeholderText;
        inputField.enabled = true;

        return inputField;
    }

    static TMP_Text CreateText(Transform parent) {
        var textObj = new GameObject("Text");
        var textTransform = textObj.AddComponent<RectTransform>();
        textTransform.anchorMax = Vector2.one;
        textTransform.anchorMin = Vector2.zero;
        textTransform.offsetMax = new Vector2(-10,-7);
        textTransform.offsetMin = new Vector2(10,6);
        textTransform.SetParent(parent,false);
        
        var text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "New Text";
        text.fontSize = 16;

        return text;
    }

    static void AddMessage(string message) {
        
    }
}