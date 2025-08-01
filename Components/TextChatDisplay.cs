using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

namespace PeakTextChat;

public class TextChatDisplay : MonoBehaviour {
    int maxMessages = 30;
    Vector2 boxSize = new Vector2(500,300);
    float fadeInTime = 0.03f;
    float fadeOutTime = 5;
    float hideTime = 5;
    float fadeOutDelay = 15;
    float hideDelay = 40;
    float messageHideDelay = 40;
    float fontSize = 25;
    bool usingIMGUI = false;

    TMP_InputField inputField;
    RectTransform chatLogViewportTransform;
    RectTransform baseTransform;
    CanvasGroup canvasGroup;

    float fade = 1;
    float fadeTimer = -1;

    float hide = 0;
    float hideTimer = -1;

    KeysHelper.KeyCodeInfo keyInfo;

    Color offWhite = new Color(0.87f,0.85f,0.76f);
    Color red = new Color(0.99f,0.33f,0);

    List<ChatMessage> messages = new List<ChatMessage>();

    public bool isBlockingInput = false;
    public int framesSinceInputBlocked = 0;

    public static TextChatDisplay instance;

    string imguiFieldString = "";
    bool imguiTyping = false;

    void Awake() {
        instance = this;
    }

    void Start() {
        keyInfo = KeysHelper.GetKeyCodeShortInfo(PeakTextChatPlugin.configKey.Value);
        fontSize = PeakTextChatPlugin.configFontSize.Value < 0 ? 1000000000 : PeakTextChatPlugin.configFontSize.Value;
        hideDelay = PeakTextChatPlugin.configHideDelay.Value < 0 ? Mathf.Infinity : PeakTextChatPlugin.configHideDelay.Value;
        fadeOutDelay = Mathf.Min(PeakTextChatPlugin.configFadeDelay.Value < 0 ? Mathf.Infinity : PeakTextChatPlugin.configFadeDelay.Value,hideDelay);
        messageHideDelay = PeakTextChatPlugin.configMessageFadeDelay.Value < 0 ? Mathf.Infinity : PeakTextChatPlugin.configMessageFadeDelay.Value;
        usingIMGUI = PeakTextChatPlugin.configIMGUI.Value;
        
        var chatSizeConfigSplit = PeakTextChatPlugin.configChatSize.Value.Split(':');
        if (chatSizeConfigSplit.Length >= 2) {
            var xString = chatSizeConfigSplit[0].Replace(" ","");
            var yString = chatSizeConfigSplit[1].Replace(" ","");

            if (float.TryParse(xString,out float chatSizeX) && float.TryParse(yString,out float chatSizeY)) {
                boxSize = new Vector2(Mathf.Max(chatSizeX,10),Mathf.Max(chatSizeY,10));
            } else {
                boxSize = new Vector2(500,300);
            }
        }

        ResetTimers();
        SetupChatGUI();
    }

    GameObject currentSelection;

    void Update() {
        if (isBlockingInput)
            framesSinceInputBlocked = -1;
        else
            framesSinceInputBlocked = Mathf.Min(framesSinceInputBlocked + 1,50);

        if (!this.gameObject.activeInHierarchy) {
            isBlockingInput = false;
            return;
        }

        if (currentSelection != EventSystem.current.currentSelectedGameObject) {
            currentSelection = EventSystem.current.currentSelectedGameObject;
            if (currentSelection != inputField.gameObject) {
                isBlockingInput = false;
            }
        }

        if (!GUIManager.instance.windowBlockingInput && Input.GetKeyDown(keyInfo.key) && inputField != null && EventSystem.current != null) {
            if (usingIMGUI) {
                imguiTyping = true;
                GUI.FocusControl("ptctf");
            } else {
                EventSystem.current.SetSelectedGameObject(inputField.gameObject,null);
                inputField.ActivateInputField();
            }
            isBlockingInput = true;
        }

        if (isBlockingInput)
            ResetTimers();

        if (PeakTextChatPlugin.configPos.Value == PeakTextChatPlugin.TextChatPosition.BottomLeft)
            UpdatePosition(true);

        fade = Mathf.Clamp(fadeTimer <= 0 ? fade - (Time.deltaTime / fadeOutTime) : fade + (Time.deltaTime / fadeInTime),0,1);
        hide = Mathf.Clamp(hideTimer <= 0 ? hide + (Time.deltaTime / hideTime) : hide - (Time.deltaTime / fadeInTime),0,1);
        fadeTimer -= Time.deltaTime;
        hideTimer -= Time.deltaTime;
        if (canvasGroup != null) {
            canvasGroup.alpha = fade * 0.5f + (1 - hide) * 0.5f;
        }

        foreach (var chatMessage in messages)
            chatMessage.Update();
    }

    void SetupChatGUI() {
        var guiManager = GUIManager.instance;
        
        baseTransform = this.gameObject.GetComponent<RectTransform>() ?? this.gameObject.AddComponent<RectTransform>();
        baseTransform.SetParent(this.transform,false);
        UpdatePosition();
        baseTransform.sizeDelta = boxSize;

        canvasGroup = this.gameObject.AddComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        canvasGroup.ignoreParentGroups = false;

        var shadow = new GameObject("Shadow");
        shadow.transform.SetParent(baseTransform,false);
        var shadowTransform = shadow.AddComponent<RectTransform>();
        shadowTransform.anchorMin = Vector2.zero;
        shadowTransform.anchorMax = Vector2.one;
        shadowTransform.offsetMin = Vector2.zero;
        shadowTransform.offsetMax = Vector2.zero;
        var shadowImg = shadow.AddComponent<ProceduralImage>();
        shadowImg.color = new Color(0,0,0,0.3f);
        shadowImg.FalloffDistance = 10;
        shadowImg.SetModifierType<UniformModifier>().Radius = fontSize / 4 + 10;

        // var bgImage = this.gameObject.AddComponent<ProceduralImage>();
        // bgImage.color = new Color(0,0,0,0.6f);
        // bgImage.BorderWidth = 3;
        // bgImage.SetModifierType<UniformModifier>().Radius = 5;

        var chatLogHolderObj = new GameObject("ChatLog");
        var chatLogHolderTransform = chatLogHolderObj.AddComponent<RectTransform>();
        chatLogHolderTransform.SetParent(baseTransform,false);
        chatLogHolderTransform.anchorMin = Vector2.zero;
        chatLogHolderTransform.anchorMax = Vector2.one;
        chatLogHolderTransform.offsetMin = new Vector2(0,fontSize * 2);
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

        inputField = CreateInputField();
        var inputFieldTransform = (RectTransform)inputField.transform;
        inputFieldTransform.pivot = new Vector2(0.5f,0);
        inputFieldTransform.anchorMin = new Vector2(0,0);
        inputFieldTransform.anchorMax = new Vector2(1,0);
        inputFieldTransform.offsetMin = new Vector2(5,5);
        inputFieldTransform.offsetMax = new Vector2(-5,fontSize * 1.75f);
        inputFieldTransform.SetParent(baseTransform,false);

        var chatLogLayout = chatLogViewportObj.AddComponent<VerticalLayoutGroup>();
        chatLogLayout.childControlWidth = true;
        chatLogLayout.childControlHeight = false;
        chatLogLayout.childForceExpandWidth = true;
        chatLogLayout.childForceExpandHeight = false;
        chatLogLayout.childScaleWidth = false;
        chatLogLayout.childScaleHeight = false;
        chatLogLayout.childAlignment = TextAnchor.LowerCenter;
        chatLogLayout.padding = new RectOffset((int)(fontSize * 0.6f),(int)(fontSize * 0.6f),(int)(fontSize / 20),(int)(fontSize / 20));
        chatLogLayout.spacing = -fontSize / 8;

        inputField.onSubmit.AddListener((e) => {
            inputField.text = "";
            inputField.textComponent.text = "";
            TextChatManager.instance?.SendChatMessage(e);
        });

        inputField.onEndEdit.AddListener((e) => {
            EventSystem.current.SetSelectedGameObject(null);
            isBlockingInput = false;
        });

        var border = new GameObject("Border");
        border.transform.SetParent(baseTransform,false);
        var borderTransform = border.AddComponent<RectTransform>();
        borderTransform.anchorMin = Vector2.zero;
        borderTransform.anchorMax = Vector2.one;
        borderTransform.offsetMin = Vector2.zero;
        borderTransform.offsetMax = Vector2.zero;
        var borderImg = border.AddComponent<ProceduralImage>();
        borderImg.color = offWhite;
        borderImg.BorderWidth = 2;
        borderImg.SetModifierType<UniformModifier>().Radius = fontSize / 4 + 5;
    }

    TMP_InputField CreateInputField() {
        var inputFieldObj = new GameObject("InputField");
        TMP_InputField inputField = inputFieldObj.AddComponent<TMP_InputField>();
        inputField.enabled = false;

        var inputFieldImg = inputField.gameObject.AddComponent<ProceduralImage>();
        inputField.targetGraphic = inputFieldImg;
        // inputField.transition = Selectable.Transition.None;
        inputFieldImg.color = offWhite;
        inputFieldImg.SetModifierType<UniformModifier>().Radius = fontSize / 4;

        var textAreaObj = new GameObject("Text Area");
        var textAreaTransform = textAreaObj.AddComponent<RectTransform>();
        textAreaTransform.anchorMax = Vector2.one;
        textAreaTransform.anchorMin = Vector2.zero;
        textAreaTransform.offsetMax = new Vector2(-fontSize / 2,-fontSize * 0.3f);
        textAreaTransform.offsetMin = new Vector2(fontSize / 2,fontSize * 0.3f);
        textAreaTransform.SetParent(inputField.transform,false);
        var textAreaMask = textAreaObj.AddComponent<RectMask2D>();
        textAreaMask.padding = new Vector4(-fontSize * 0.4f,-fontSize / 4,-fontSize * 0.4f,-fontSize / 4);
        
        var placeholderText = CreateText(textAreaTransform);
        placeholderText.color = new(0,0,0,0.6f);
        placeholderText.text = $"Press {keyInfo.keyText} to chat";
        placeholderText.name = "Placeholder";

        var mainText = CreateText(textAreaTransform);
        mainText.color = new(0,0,0,1);

        inputField.textViewport = textAreaTransform;
        inputField.textComponent = mainText;
        inputField.placeholder = placeholderText;
        inputField.richText = false;
        inputField.restoreOriginalTextOnEscape = false;
        inputField.enabled = true;

        return inputField;
    }

    TMP_Text CreateText(Transform parent) {
        var textObj = new GameObject("Text");
        var textTransform = textObj.AddComponent<RectTransform>();
        textTransform.anchorMax = Vector2.one;
        textTransform.anchorMin = Vector2.zero;
        textTransform.offsetMax = new Vector2(-2,0);
        textTransform.offsetMin = new Vector2(2,4);
        textTransform.SetParent(parent,false);
        
        var text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = "New Text";
        if (GUIManagerPatch.darumaDropOneFont != null)
            text.font = GUIManagerPatch.darumaDropOneFont;
        text.fontSize = fontSize;
        text.horizontalAlignment = HorizontalAlignmentOptions.Left;
        text.verticalAlignment = VerticalAlignmentOptions.Middle;

        return text;
    }

    public void AddMessage(string message) {
        if (chatLogViewportTransform != null) {
            var tmpText = CreateText(chatLogViewportTransform);
            tmpText.text = message;
            tmpText.color = offWhite;
            tmpText.lineSpacing = -fontSize * 0.75f;
            var prefValues = tmpText.GetPreferredValues(message,boxSize.x - 24,1000);

            ((RectTransform)tmpText.transform).sizeDelta = new Vector2(0,prefValues.y);
            var chatMessage = new ChatMessage(message,tmpText.gameObject,messageHideDelay);
            messages.Add(chatMessage);
            if (messages.Count > maxMessages) {
                var firstMessage = messages[0];
                if (firstMessage != null && firstMessage.textObj != null) {
                    GameObject.Destroy(firstMessage.textObj);
                }
                messages.RemoveAt(0);
            }
            ResetTimers();
        }
    }

    public void AddMessage(TextChatManager.Message messageData) {
        if (chatLogViewportTransform != null) {
            // string deadLabel = !messageData.isDead ? $"<color=#{ColorUtility.ToHtmlStringRGB(red)}>[DEAD]</color>" : "";
            Color usernameColor = messageData.character != null ? messageData.character.refs.customization.PlayerColor : new Color(0.64f,0.69f,0.83f);
            string usernameLabel = $"<color=#{ColorUtility.ToHtmlStringRGB(usernameColor)}>[{messageData.character?.characterName ?? "Unknown"}]</color>";

            string message = messageData.message;
            if (!PeakTextChatPlugin.configRichTextEnabled.Value) {
                message = Regex.Replace(message, "<.*?>", string.Empty);
            }

            AddMessage($"{usernameLabel}: {message}");
        }
    }

    void ResetTimers() {
        fadeTimer = fadeOutDelay;
        hideTimer = hideDelay;
    }

    void UpdatePosition(bool isBottomLeft = false) {
        if (isBottomLeft || PeakTextChatPlugin.configPos.Value == PeakTextChatPlugin.TextChatPosition.BottomLeft) {
            if (baseTransform != null && StaminaBarPatch.barGroupChildWatcher.textChatDummyTransform != null) {
                baseTransform.pivot = Vector2.zero;
                baseTransform.anchorMax = Vector2.zero;
                baseTransform.anchorMin = Vector2.zero;
                baseTransform.position = StaminaBarPatch.barGroupChildWatcher.textChatDummyTransform.position;
                baseTransform.anchoredPosition += new Vector2(0,40);
            }
        } else if (PeakTextChatPlugin.configPos.Value == PeakTextChatPlugin.TextChatPosition.TopLeft) {
            baseTransform.pivot = new Vector2(0,1);
            baseTransform.anchorMax = new Vector2(0,1);
            baseTransform.anchorMin = new Vector2(0,1);
            baseTransform.anchoredPosition = new Vector2(64,-62);
        } else if (PeakTextChatPlugin.configPos.Value == PeakTextChatPlugin.TextChatPosition.TopRight) {
            baseTransform.pivot = Vector2.one;
            baseTransform.anchorMax = Vector2.one;
            baseTransform.anchorMin = Vector2.one;
            baseTransform.anchoredPosition = new Vector2(-64,-62);
        }
    }

    void OnGUI() {
        if (!imguiTyping)
            return;

        GUI.SetNextControlName("ptctf");
        var guiStyle = new GUIStyle();
        guiStyle.fontSize = (int)(fontSize * 0.8f);
        // var bg = new Texture2D(1,1,TextureFormat.RGBAFloat,false);
        // bg.SetPixel(0,0,new Color(0,0,0,0.4f));
        // bg.Apply();
        // guiStyle.normal.background = bg;
        guiStyle.clipping = TextClipping.Clip;
        guiStyle.padding = new RectOffset((int)(fontSize * 0.7f),(int)(fontSize * 0.7f),(int)(fontSize * 0.24f),0);
        var rect = RectTransformToScreenSpace((RectTransform)inputField.transform);
        PeakTextChatPlugin.Logger.LogInfo(rect.xMin + " " + rect.yMin);
        imguiFieldString = GUI.TextArea(rect,imguiFieldString,guiStyle);

        inputField.text = " ";

        if (GUI.GetNameOfFocusedControl() != "ptctf")
            GUI.FocusControl("ptctf");

        if (imguiFieldString.Contains("\n")) {
            TextChatManager.instance?.SendChatMessage(imguiFieldString.Replace("\n",""));
            imguiFieldString = "";
            GUI.FocusControl("");
            Event.current.Use();
            imguiTyping = false;
            isBlockingInput = false;
            inputField.text = "";
        } else if (Event.current.isKey && Event.current.keyCode == KeyCode.Escape) {
            GUI.FocusControl("");
            imguiTyping = false;
            inputField.text = imguiFieldString;
            isBlockingInput = false;
            Event.current.Use();
        }
    }

    public static Rect RectTransformToScreenSpace(RectTransform transform)
    {
        // return RectTransformUtility.PixelAdjustRect(transform,GUIManager.instance.hudCanvas);
        Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
        var pos = new Vector2(transform.position.x,Screen.height - transform.position.y);
        return new Rect(pos - (size * new Vector2(0.5f,1)), size);
    }

    public class ChatMessage {
        public string message;
        public GameObject textObj;
        public TMP_Text text;
        Color textColor;

        float hideDelay = 40;
        float hideTime = 10;

        float hideTimer = -1;
        float hide = 0;

        public void Update() {
            hideTimer -= Time.deltaTime;

            if (hideTimer <= 0) {
                if (hide < 1) {
                    hide += Time.deltaTime / hideTime;
                    text.color = new Color(textColor.r,textColor.g,textColor.b,1 - hide);
                }
            }
        }
        
        public ChatMessage(string message,GameObject textObject,float hideDelay) {
            this.message = message;
            this.textObj = textObject;
            this.text = textObject.GetComponent<TMP_Text>();
            this.textColor = this.text.color;
            this.hideDelay = hideDelay;
            hideTimer = hideDelay;
        }
    }
}