using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System;
using Microsoft.MixedReality.Toolkit.Experimental.UI;
using Microsoft.MixedReality.Toolkit.UI;

public class WelcomeSceneHandler : MonoBehaviour
{
    public TMP_InputField email;
    public TMP_InputField password;
    public TextMeshProUGUI errorText;
    public GameObject welcomeWindow;
    public GameObject selectionWindow;

    public TextMeshProUGUI selectedPresText;

    public GameObject LoginLoader;

    public GameObject ScrollObjectCollectionPresButtons;
    public GameObject PresListButtonPrefab;
    public int objectsPerFrame = 5;

    public void Login()
    {
        string emailString = email.text.Replace("\u200B", "");
        string passwordString = password.text.Replace("\u200B", "");
        LoginLoader.SetActive(true);
        BackendConnection.BC.Login(emailString, passwordString, LoginSucceed, LoginFailed);
    }

    private void LoginSucceed(LoginResponse response)
    {
        print("Login Succeed");
        BackendConnection.BC.LoadPresList(LoadListSucceed, LoadListFailed);
    }

    private void LoginFailed(string msg)
    {
        print("Login Failed");
        errorText.text = "Login failed. Please check your credentials.";
    }

    private void LoadListSucceed(ListResponse response)
    {
        print("Get presentation list successfull");

        //Switch to selection window state
        LoginLoader.SetActive(false);
        welcomeWindow.SetActive(false);
        selectionWindow.SetActive(true);

        //Start lazy loading
        StartCoroutine(CreateListLazy(response.presentations, objectsPerFrame));
    }

    private void LoadListFailed(string msg)
    {
        print("Get presentation list not successfull");
        errorText.text = "Logged in but fetching presentations failed.";
    }

    private IEnumerator CreateListLazy(List<PresentationElement> presList, int instancesPerFrame)
    {
        int currentIndex = 0;

        ScrollingObjectCollection scrollObjectCollection = ScrollObjectCollectionPresButtons.GetComponent<ScrollingObjectCollection>();
        while (currentIndex < presList.Count)
        {
            for (int i = currentIndex; i < Math.Min(currentIndex + instancesPerFrame, presList.Count); i++)
            {
                PresentationElement pres = presList[i];
                //Create the Button
                GameObject g = Instantiate(PresListButtonPrefab);
                g.transform.parent = scrollObjectCollection.transform;
                g.transform.localScale = new Vector3(1, 1, 1);
                PresListButtonScript btnScript = g.GetComponent<PresListButtonScript>();
                btnScript.pres = pres;
                btnScript.showNameOfPres = selectedPresText;
                ButtonConfigHelper btnConfigHelper = g.GetComponent<ButtonConfigHelper>();
                btnConfigHelper.OnClick.AddListener(delegate { btnScript.Click(pres); });

                currentIndex++;
            }

            yield return null;
        }

        // List is ready to show
        GameObject loadingVizualiser = GameObject.Find("PresListLoadingIndicator");
        if(loadingVizualiser != null)
        {
            loadingVizualiser.SetActive(false);
        }
        ScrollObjectCollectionPresButtons.SetActive(true);

        scrollObjectCollection.UpdateCollection();
    }

    public void OpenPresentation()
    {
        if (StaticInformation.selectedPresElem == null) return;

        SceneManager.LoadScene("Scenes/EditorScene");
    }
}
