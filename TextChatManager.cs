
using PeakTextChat;
using Photon.Pun;
using UnityEngine;

namespace PeakTextChat;

public class TextChatManager: MonoBehaviour {
    public static TextChatManager instance;
    Character character;

    void Start() {
        instance = this;
        character = GetComponent<Character>();
    }

    [PunRPC]
    public void ReceiveChatMessage(string senderName,string message) {
        GUIManagerTextChat.AddMessage($"[{senderName}]: {message}");
    }

    public void SendChatMessage(string message) {
        if (!string.IsNullOrWhiteSpace(message) && character != null) {
            character.photonView.RPC("ReceiveChatMessage",RpcTarget.All,PhotonNetwork.NickName,message);
        }
    }
}