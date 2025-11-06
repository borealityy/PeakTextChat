
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

namespace PeakTextChat;

public class TextChatManager : MonoBehaviour {
    public static TextChatManager instance;
    Character character;

    byte chatEventCode = 81;

    void Start() {
        instance = this;
        character = GetComponent<Character>();
    }

    void OnEnable() {
        PhotonNetwork.NetworkingClient.EventReceived += OnEventReceived;
    }

    void OnDisable() {
        PhotonNetwork.NetworkingClient.EventReceived -= OnEventReceived;
    }

    void OnEventReceived(EventData eventData) {
        if (eventData.Code == chatEventCode) {
            var data = (object[])eventData.CustomData;
            if (data.Length < 4)
                return;
            string nickname = data[0]?.ToString() ?? "???";
            string message = data[1]?.ToString() ?? "";
            string userId = data[2]?.ToString() ?? "";
            bool isDead = bool.TryParse(data[3]?.ToString(), out var d) && d;

            ReceiveChatMessage(userId,message,isDead);
        }
    }

    public void ReceiveChatMessage(string userId,string message,bool isDead) {
        if (TextChatDisplay.instance != null) {
            TextChatDisplay.instance.AddMessage(new Message(userId,message,isDead));
        }
    }

    public void SendChatMessage(string message) {
        if (!string.IsNullOrWhiteSpace(message)) {
            bool isDead = false;
            try {
                if (Character.localCharacter?.data != null) {
                    isDead = Character.localCharacter.data.dead;
                }
            } catch {}

            object[] payload = {
                PhotonNetwork.LocalPlayer.NickName,
                message,
                PhotonNetwork.LocalPlayer.UserId,
                isDead
            };

            PhotonNetwork.RaiseEvent(
                chatEventCode,
                payload,
                new RaiseEventOptions() { Receivers = ReceiverGroup.All },
                SendOptions.SendReliable
            );
        }
    }

    public static void CleanupObjects() {
        if (instance != null) {
            GameObject.Destroy(instance.gameObject);
        }
    }

    public class Message {
        public Character character;
        public string message;
        public bool isDead;

        public Message(string userId,string message,bool isDead) {
           this.character = Character.AllCharacters.Find((c) => c.photonView?.Owner?.UserId == userId);
           this.message = message;
           this.isDead = isDead;
        }
    }
}