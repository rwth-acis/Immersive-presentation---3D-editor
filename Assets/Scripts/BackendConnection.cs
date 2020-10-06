using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class BackendConnection : MonoBehaviour
{
    //Use the Singelton Best Practice
    public static BackendConnection BC;

    private string baseurl = "http://binarybros.de";

    private bool loggedIn = false;
    private string token;
    private long exp;
    private int userId;

    public void Login(string email, string pwd, UnityAction<LoginResponse> callbackOnSuccess, UnityAction<string> callbackOnFail)
    {
        StartCoroutine(MakeLoginRequest(email, pwd, callbackOnSuccess, callbackOnFail));
    }
    IEnumerator MakeLoginRequest(string email, string pwd, UnityAction<LoginResponse> callbackOnSuccess, UnityAction<string> callbackOnFail)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", pwd);
        UnityWebRequest request = UnityWebRequest.Post(baseurl + "/auth/login", form);
        yield return request.SendWebRequest();

        if(request.isNetworkError || request.isHttpError)
        {
            callbackOnFail?.Invoke(request.error);
        }
        else
        {
            LoginResponse output = JsonConvert.DeserializeObject<LoginResponse>(request.downloadHandler.text);
            loggedIn = true;
            token = output.token;
            exp = output.exp;
            userId = output.user.iduser;
            callbackOnSuccess?.Invoke(output);
        }
    }

    public void LoadPresList(UnityAction<ListResponse> callbackOnSuccess, UnityAction<string> callbackOnFail)
    {
        StartCoroutine(MakePresListRequest(callbackOnSuccess, callbackOnFail));
    }

    IEnumerator MakePresListRequest(UnityAction<ListResponse> callbackOnSuccess, UnityAction<string> callbackOnFail)
    {
        UnityWebRequest request = UnityWebRequest.Get(baseurl + "/presentations");
        request.SetRequestHeader("Authorization", "Bearer " + token);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            callbackOnFail?.Invoke(request.error);
        }
        else
        {
            ListResponse output = JsonConvert.DeserializeObject<ListResponse>(request.downloadHandler.text);
            callbackOnSuccess?.Invoke(output);
        }
    }

    /// <summary>
    /// Downloads the presentation file from the backend coordinator
    /// </summary>
    /// <param name="id">Identifier of the presentation</param>
    /// <param name="path">Locatio where the presentation file should be saved</param>
    /// <param name="callbackOnSuccess">Callback that is called when download succeeded</param>
    /// <param name="callbackOnFail">Callback that is called when download failed</param>
    public void DownloadPresentation(string id, string path, UnityAction<string> callbackOnSuccess, UnityAction<string> callbackOnFail)
    {
        StartCoroutine(MakeDownloadPresentation(id, path, callbackOnSuccess, callbackOnFail));
    }

    IEnumerator MakeDownloadPresentation(string id, string path, UnityAction<string> callbackOnSuccess, UnityAction<string> callbackOnFail)
    {
        UnityWebRequest request = UnityWebRequest.Get(baseurl + string.Format("/presentation/foreditor?idpresentation={0}", Uri.EscapeDataString(id)));
        request.SetRequestHeader("Authorization", "Bearer " + token);
        request.downloadHandler = new DownloadHandlerFile(path);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            callbackOnFail?.Invoke(request.error);
        }
        else
        {
            callbackOnSuccess?.Invoke(path);
        }
    }

    private void Awake()
    {
        if (BC != null)
            GameObject.Destroy(BC);
        else
            BC = this;

        DontDestroyOnLoad(this);
    }
}
