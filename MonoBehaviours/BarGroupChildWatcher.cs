using UnityEngine;

namespace PeakTextChat;

public class BarGroupChildWatcher : MonoBehaviour {
    public Transform textChatDummyTransform;

    void OnTransformChildrenChanged() {
        if (textChatDummyTransform != null && textChatDummyTransform.GetSiblingIndex() > 0) {
            textChatDummyTransform.SetAsFirstSibling();
        }
    }
}