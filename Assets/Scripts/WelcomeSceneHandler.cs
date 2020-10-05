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
    public TMP_InputField email;
    public TMP_InputField password;
    public TextMeshProUGUI errorText;
    public GameObject welcomeWindow;
    public GameObject selectionWindow;

    public void Login()
    {
        string emailString = email.text.Replace("\u200B", "");
        string passwordString = password.text.Replace("\u200B", "");
        BackendConnection.BC.Login(emailString, passwordString, LoginSucceed, LoginFailed);
    }

    private void LoginSucceed(LoginResponse response)
    {
        print("Login Succeed");
        welcomeWindow.SetActive(false);
        selectionWindow.SetActive(true);
    }

    private void LoginFailed(string msg)
    {
        print("Login Failed");
        errorText.text = "Login failed. Please check your credentials.";
    }

    public void OpenPresentation()
    {
        StaticInformation.SelectedPresName = "TestPresName";
        SceneManager.LoadScene("Scenes/EditorScene");
    }
}
