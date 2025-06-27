
using PeakTextChat;
using Photon.Pun;
using UnityEngine;

namespace PeakTextChat;

public class TextChatManager : MonoBehaviour {
    public static TextChatManager instance;
    Character character;

    void Start() {
        instance = this;
        character = GetComponent<Character>();
    }

    [PunRPC]
    public void ReceiveChatMessage(string senderName,string message) {
        if (TextChatDisplay.instance != null) {
            TextChatDisplay.instance.AddMessage($"[{senderName}]: {message}");
        }
    }

    public void SendChatMessage(string message) {
        Plugin.Logger.LogInfo("fnaf");
        if (!string.IsNullOrWhiteSpace(message) && character != null) {
            character.photonView.RPC("ReceiveChatMessage",RpcTarget.All,PhotonNetwork.NickName,message);
        }
    }
}