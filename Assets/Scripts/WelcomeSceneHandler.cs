using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

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
    }

    public void OpenPresentation()
    {
        StaticInformation.SelectedPresName = "TestPresName";
        SceneManager.LoadScene("Scenes/EditorScene");
    }
}
