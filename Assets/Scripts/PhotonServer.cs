using UnityEngine;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using TestPhotonLib.Common;
using TestPhotonLib.Common.CustomEventArgs;
using System;

public class PhotonServer : MonoBehaviour, IPhotonPeerListener {

    private const string CONNECTION_STRING = "localhost:5055";
    private const string APP_NAME = "MyTestServer";

    private static PhotonServer _instance;

    public static PhotonServer Instance {
        get { return _instance; }
    }

    private PhotonPeer PhotonPeer { get; set; }

    public event EventHandler<LoginEventArgs> OnLoginResponse;
    public event EventHandler<ChatMessageEventArgs> OnRecieveChatMessage;
    public event EventHandler<AmountOfPlayersEventArgs> OnRecieveAmountOfPlayers;


    void Awake() {
        if (Instance != null) {
            DestroyObject(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        Application.runInBackground = true;
        _instance = this;
    }

    // Use this for initialization
    void Start () {
        PhotonPeer = new PhotonPeer(this, ConnectionProtocol.Udp);
        Connect();
	}
	
	// Update is called once per frame
	void Update () {
        if (PhotonPeer != null) {
            PhotonPeer.Service();
        }
	}

    void OnApplicationQuit() {
        Disconnect();
    }

    private void Connect() {
        if (PhotonPeer != null) {
            PhotonPeer.Connect(CONNECTION_STRING, APP_NAME);
        }
    }

    private void Disconnect() {
        if (PhotonPeer != null) {
            PhotonPeer.Disconnect();
        }
    }

    public void DebugReturn(DebugLevel level, string message) {
    }

    public void OnEvent(EventData eventData) {
        switch (eventData.Code) {
            case (byte)EventCode.ChatMessage:
                ChatMessageHandler(eventData);
                break;
            case (byte)EventCode.AmountOfPlayers:
                Debug.Log("AmountOfPlayers operation: " + eventData.Code);
                AmountOfPlayerHandler(eventData);
                break;
            default:
                Debug.Log("Unknown operation: " + eventData.Code);
                break;
        }
    }

    public void OnOperationResponse(OperationResponse operationResponse) {
        switch (operationResponse.OperationCode) {
            case (byte)OperationCode.Login:
                LoginHandler(operationResponse);
                break;
            default:
                Debug.Log("Unknown operation: " + operationResponse.OperationCode);
                break;
        }
    }

    public void OnStatusChanged(StatusCode statusCode) {
        switch (statusCode) {
            case StatusCode.Connect:
                Debug.Log("Connected to server!");
                break;
            case StatusCode.Disconnect:
                Debug.Log("Disconnected to server!");
                break;
            default:
                Debug.Log("Unknown status: " + statusCode.ToString());
                break;
        }

    }


    public void SendLoginOperation(string name) {
        PhotonPeer.OpCustom((byte)OperationCode.Login, new Dictionary<byte, object> { {(byte)ParameterCode.CharacterName, name } }, true);
    }

    public void SendChatMessage(string message) {
        PhotonPeer.OpCustom((byte)OperationCode.SendChatMessage, new Dictionary<byte, object> { { (byte)ParameterCode.ChatMessage, message } }, true);
    }

    public void GetRecentChatMessage() {
        PhotonPeer.OpCustom((byte)OperationCode.GetRecentChatMessage, new Dictionary<byte, object> { { (byte)ParameterCode.ChatMessage, "" } }, true);
    }

    public void GetAmountOfPlayers() {
        PhotonPeer.OpCustom((byte)OperationCode.GetAmountOfPlayers, new Dictionary<byte, object> { { (byte)ParameterCode.AmountOfPlayer, "" } }, true);
    }


    private void LoginHandler(OperationResponse operationResponse) {
        if (operationResponse.ReturnCode != 0) {
            ErrorCode errorCode = (ErrorCode)operationResponse.ReturnCode;

            switch (errorCode) {
                case ErrorCode.Ok:
                    break;
                case ErrorCode.InvalidParameters:
                    break;
                case ErrorCode.NameIsExist:
                    if (OnLoginResponse != null) {
                        OnLoginResponse(this, new LoginEventArgs(ErrorCode.NameIsExist));
                    }
                    break;
                case ErrorCode.RequestNotImplemented:
                    break;
                default:
                    Debug.Log("Error login returnCode: " + operationResponse.ReturnCode);
                    break;
            }
            return;
        }

        if (OnLoginResponse != null) {
            OnLoginResponse(this, new LoginEventArgs(ErrorCode.Ok));
        }

    }

    private void ChatMessageHandler(EventData eventData) {
        string message = (string)eventData.Parameters[(byte)ParameterCode.ChatMessage];

        if (OnRecieveChatMessage != null) {
            OnRecieveChatMessage(this, new ChatMessageEventArgs(message));
        }
    }

    private void AmountOfPlayerHandler(EventData eventData) {
        int amount = (int)eventData.Parameters[(byte)ParameterCode.AmountOfPlayer];

        if (OnRecieveAmountOfPlayers != null) {
            OnRecieveAmountOfPlayers(this, new AmountOfPlayersEventArgs(amount));
        }
    }
}
