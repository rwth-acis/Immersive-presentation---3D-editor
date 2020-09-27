using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;

public class WelcomeSceneHandler : MonoBehaviour
{
    public TextMeshProUGUI email;
    public TMP_InputField password;
    public GameObject welcomeWindow;
    public GameObject selectionWindow;

    public void Login()
    {
        print(email.text);
        print(password.text);

        welcomeWindow.SetActive(false);
        selectionWindow.SetActive(true);

        //Real API Call

        BackendConnection.BC.Login(LoginSucceed, LoginFailed);
    }

    private void LoginSucceed(LoginResponse response)
    {
        print("Succeed");
    }

    private void LoginFailed(string msg)
    {
        print("Failed");
    }

    public void OpenPresentation()
    {
        StaticInformation.SelectedPresName = "TestPresName";
        SceneManager.LoadScene("Scenes/EditorScene");
    }
}
