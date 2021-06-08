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
using i5.Toolkit.Core.OpenIDConnectClient;
using i5.Toolkit.Core.ServiceCore;
using i5.Toolkit.Core.Utilities;

public class WelcomeSceneHandler : MonoBehaviour
{
    [SerializeField]
    private ClientDataObject learningLayersClientData;

    private bool isSubscribedToOidc = false;

    public TMP_InputField email;
    public TMP_InputField password;
    public TMP_InputField joinInput;
    public TMP_InputField joinGuestInput;
    public TextMeshProUGUI errorText;
    public GameObject welcomeWindow;
    public GameObject selectionWindow;

    public TextMeshProUGUI selectedPresText;

    public GameObject LoginLoader;

    public GameObject ScrollObjectCollectionPresButtons;
    public GameObject PresListButtonPrefab;
    public int objectsPerFrame = 5;

    private bool loginstarted = false;

    private bool rememberMe = true;

    public void Start()
    {
        email.text = PlayerPrefs.GetString("email", "");
        password.text = PlayerPrefs.GetString("pwd", "");
        if (BackendConnection.BC != null && BackendConnection.BC.loggedIn)
        {
            welcomeWindow.SetActive(false);
            LoginLoader.SetActive(true);
            BackendConnection.BC.LoadPresList(LoadListSucceed, LoadListFailed);
        }
    }

    public void Login()
    {
        if (loginstarted == true) return;

        

        loginstarted = true;
        string emailString = email.text.Replace("\u200B", "");
        string passwordString = password.text.Replace("\u200B", "");
        if (rememberMe)
        {
            PlayerPrefs.SetString("email", emailString);
            PlayerPrefs.SetString("pwd", passwordString);
            PlayerPrefs.Save();
        }
        LoginLoader.SetActive(true);
        BackendConnection.BC.Login(emailString, passwordString, LoginSucceed, LoginFailed);
    }

    private void LoginSucceed(LoginResponse response)
    {
        print("Login Succeed");
        BackendConnection.BC.loggedIn = true;
        BackendConnection.BC.LoadPresList(LoadListSucceed, LoadListFailed);
    }

    private void LoginFailed(string msg)
    {
        loginstarted = false;
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

        SceneManager.LoadScene("Scenes/EditorScene", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("WelcomeScene");
    }
    public void LoginLearningLayers()
    {
        loginstarted = true;
        LoginLoader.SetActive(true);

        LearningLayersOidcProvider provider = new LearningLayersOidcProvider();
        provider.ClientData = learningLayersClientData.clientData;
        ServiceManager.GetService<OpenIDConnectService>().OidcProvider = provider;

        ServiceManager.GetService<OpenIDConnectService>().RedirectURI = "i5:/";
        // only subscribe to the event if it was not yet done before, e.g. in a failed login attempt
        if (!isSubscribedToOidc)
        {
            ServiceManager.GetService<OpenIDConnectService>().LoginCompleted += OpenIDConnect_LoginCompleted;
            isSubscribedToOidc = true;
        }
        ServiceManager.GetService<OpenIDConnectService>().OpenLoginPage();
    }

    private async void OpenIDConnect_LoginCompleted(object sender, System.EventArgs e)
    {
        i5Debug.Log("Login completed", this);
        i5Debug.Log(ServiceManager.GetService<OpenIDConnectService>().AccessToken, this);
        ServiceManager.GetService<OpenIDConnectService>().LoginCompleted -= OpenIDConnect_LoginCompleted;
        isSubscribedToOidc = false;


        IUserInfo userInfo = await ServiceManager.GetService<OpenIDConnectService>().GetUserDataAsync();
        i5Debug.Log("Currently logged in user: " + userInfo.FullName
            + " (username: " + userInfo.Username + ") with the mail address " + userInfo.Email, this);
        BackendConnection.BC.authOpenIDConnect(userInfo.Email, ServiceManager.GetService<OpenIDConnectService>().AccessToken, LoginSucceed, LoginFailed);
    }

    public void terminateApplication()
    {
        Application.Quit();
    }

    public void logout()
    {
        selectionWindow.SetActive(false);
        welcomeWindow.SetActive(true);
        BackendConnection.BC.loggedIn = false;
    }

    public void toogleRememberMe()
    {
        rememberMe = !rememberMe;
    }

    public void joinPres()
    {
        string inputShortCode = joinInput.text.Replace("\u200B", "");
        if (inputShortCode == "") return;

        StaticInformation.shortCode = inputShortCode;
        SceneManager.LoadScene("Scenes/PresentationScene", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("WelcomeScene");
    }
    public void joinPresGuest()
    {
        string inputShortCode = joinGuestInput.text.Replace("\u200B", "");
        if (inputShortCode == "") return;

        StaticInformation.shortCode = inputShortCode;
        SceneManager.LoadScene("Scenes/PresentationScene", LoadSceneMode.Additive);
        SceneManager.UnloadSceneAsync("WelcomeScene");
    }
}
