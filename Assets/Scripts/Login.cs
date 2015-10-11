using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TestPhotonLib.Common.CustomEventArgs;
using TestPhotonLib.Common;

public class Login : MonoBehaviour {

    public InputField inputField;
    public Button button;
    public Text errorText;

    private string Error { get; set; }
    private string CharacterName { get; set; }



	// Use this for initialization
	void Start () {
        PhotonServer.Instance.OnLoginResponse += OnLoginHandler;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void SendLoginToServer() {
        CharacterName = inputField.text;
        Error = "";
        if (CharacterName.Equals("")) {
            errorText.text = "Введите имя!";
            return;
        }
        PhotonServer.Instance.SendLoginOperation(CharacterName);

    }

    private void OnLoginHandler(object o, LoginEventArgs e) {
        if (e.Error != ErrorCode.Ok) {
            if (e.Error.ToString().Equals("NameIsExist")) {
                errorText.text = "Пользователь с таким именем зарегистрирован!";
                return;
            }
        }

        PhotonServer.Instance.OnLoginResponse -= OnLoginHandler;
        Application.LoadLevel("Game");
    }

}
