using System.Collections.Generic;
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
    static int maxMessages = 30;

    static Canvas textChatCanvas;
    static TMP_InputField inputField;

    static RectTransform chatLogViewportTransform;

    public static bool isBlockingInput = false;

    static List<ChatMessage> messages = new List<ChatMessage>();

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
        var baseObj = new GameObject("Image");
        var baseTransform = baseObj.AddComponent<RectTransform>();
        baseTransform.SetParent(canvasObj.transform,false);
        baseTransform.anchorMax = Vector2.zero;
        baseTransform.anchorMin = Vector2.zero;
        baseTransform.pivot = Vector2.zero;
        baseTransform.anchoredPosition = new Vector2(30,120);
        baseTransform.sizeDelta = new Vector2(350,250);

        inputField = CreateInputField();
        var inputFieldTransform = (RectTransform)inputField.transform;
        inputFieldTransform.pivot = new Vector2(0.5f,0);
        inputFieldTransform.anchorMin = new Vector2(0,0);
        inputFieldTransform.anchorMax = new Vector2(1,0);
        inputFieldTransform.offsetMin = new Vector2(0,0);
        inputFieldTransform.offsetMax = new Vector2(0,30);
        inputFieldTransform.SetParent(baseTransform,false);

        var bgImage = baseObj.AddComponent<Image>();
        bgImage.color = new Color(0,0,0,0.6f);

        var chatLogHolderObj = new GameObject("ChatLog");
        var chatLogHolderTransform = chatLogHolderObj.AddComponent<RectTransform>();
        chatLogHolderTransform.SetParent(baseTransform,false);
        chatLogHolderTransform.anchorMin = Vector2.zero;
        chatLogHolderTransform.anchorMax = Vector2.one;
        chatLogHolderTransform.offsetMin = new Vector2(0,30);
        chatLogHolderTransform.offsetMax = Vector2.zero;
        chatLogHolderObj.AddComponent<RectMask2D>();

        var chatLogViewportObj = new GameObject("Viewport");
        chatLogViewportTransform = chatLogViewportObj.AddComponent<RectTransform>();
        chatLogViewportTransform.SetParent(chatLogHolderTransform,false);
        chatLogViewportTransform.pivot = new Vector2(0.5f,0);
        chatLogViewportTransform.anchorMin = Vector2.zero;
        chatLogViewportTransform.anchorMax = new Vector2(1,0);
        chatLogViewportTransform.offsetMin = Vector2.zero;
        chatLogViewportTransform.offsetMax = new Vector2(0,5000);

        var chatLogLayout = chatLogViewportObj.AddComponent<VerticalLayoutGroup>();
        chatLogLayout.childControlWidth = true;
        chatLogLayout.childControlHeight = false;
        chatLogLayout.childForceExpandWidth = true;
        chatLogLayout.childForceExpandHeight = false;
        chatLogLayout.childScaleWidth = false;
        chatLogLayout.childScaleHeight = false;
        chatLogLayout.childAlignment = TextAnchor.LowerCenter;

        inputField.onSubmit.AddListener((e) => {
            inputField.text = "";
            AddMessage(e);
        });

        inputField.onEndEdit.AddListener((e) => {
            EventSystem.current.SetSelectedGameObject(null);
            isBlockingInput = false;
        });
    }

    static TMP_InputField CreateInputField() {
        var inputFieldObj = new GameObject("InputField");
        TMP_InputField inputField = inputFieldObj.AddComponent<TMP_InputField>();
        inputField.enabled = false;

        var inputFieldGraphic = inputField.gameObject.AddComponent<Image>();
        inputField.targetGraphic = inputFieldGraphic;
        inputFieldGraphic.color = new Color(1,1,1,0.3f);

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
        text.horizontalAlignment = HorizontalAlignmentOptions.Left;
        text.verticalAlignment = VerticalAlignmentOptions.Middle;

        return text;
    }

    static void AddMessage(string message) {
        if (chatLogViewportTransform != null) {
            var tmpText = CreateText(chatLogViewportTransform);
            tmpText.text = message;
            ((RectTransform)tmpText.transform).sizeDelta = new Vector2(0,tmpText.preferredHeight);
            var chatMessage = new ChatMessage(message,tmpText.gameObject);
            messages.Add(chatMessage);
            if (messages.Count > maxMessages) {
                var firstMessage = messages[0];
                if (firstMessage != null && firstMessage.textObj != null) {
                    GameObject.Destroy(firstMessage.textObj);
                }
                messages.RemoveAt(0);
            }
        }
    }

    public class ChatMessage {
        public string message;
        public GameObject textObj;
        public ChatMessage(string message,GameObject textObject) {
            this.message = message;
            this.textObj = textObject;
        }
    }
}