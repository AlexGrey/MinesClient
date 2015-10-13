using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TestPhotonLib.Common;
using TestPhotonLib.Common.CustomEventArgs;

public class ChatInfo : MonoBehaviour {

    public Text chatLogText;
    public InputField inputField;
    public Scrollbar scrollBar;
    public Text amountOfPlayerOnline;

    private string message = "";
    private string chatLog = "";

	// Use this for initialization
	void Start () {
        PhotonServer.Instance.OnRecieveChatMessage += OnRecieveChatMessage;
        PhotonServer.Instance.OnRecieveAmountOfPlayers += OnRecieveAmountOfPlayers;
        PhotonServer.Instance.GetRecentChatMessage();
        PhotonServer.Instance.GetAmountOfPlayers();
    }

    void OnDestroy() {
        PhotonServer.Instance.OnRecieveChatMessage -= OnRecieveChatMessage;
        PhotonServer.Instance.OnRecieveAmountOfPlayers -= OnRecieveAmountOfPlayers;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private void OnRecieveChatMessage(object o, ChatMessageEventArgs e) {
        chatLog += e.Message + "\r\n";
        chatLogText.text = chatLog;
        scrollBar.value = 0;
    }

    private void OnRecieveAmountOfPlayers(object o, AmountOfPlayersEventArgs e) {
        Debug.Log(e.AmountOfPlayers);
        amountOfPlayerOnline.text = e.AmountOfPlayers.ToString();
    }

    public void SendMessage() {
        message = inputField.text;
        PhotonServer.Instance.SendChatMessage(message);
        message = "";
        chatLogText.text = "";
    }

}
