using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class BackendConnection : MonoBehaviour
{
    //Use the Singelton Best Practice
    public static BackendConnection BC;

    private string baseurl = "http://binarybros.de";

    public bool loggedIn = false;
    private string token;
    private long exp;
    public int userId;

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

    public void authOpenIDConnect(string email, string accessToken, UnityAction<LoginResponse> callbackOnSuccess, UnityAction<string> callbackOnFail)
    {
        StartCoroutine(MakeAuthOpenIDConnectRequest(email, accessToken, callbackOnSuccess, callbackOnFail));
    }
    IEnumerator MakeAuthOpenIDConnectRequest(string email, string accessToken, UnityAction<LoginResponse> callbackOnSuccess, UnityAction<string> callbackOnFail)
    {
        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("accesstoken", accessToken);
        UnityWebRequest request = UnityWebRequest.Post(baseurl + "/auth/openid", form);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
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

    /// <summary>
    /// Downloads the presentation file from the backend coordinator
    /// </summary>
    /// <param name="id">ShortCode of the presentation</param>
    /// <param name="path">Locatio where the presentation file should be saved</param>
    /// <param name="callbackOnSuccess">Callback that is called when download succeeded</param>
    /// <param name="callbackOnFail">Callback that is called when download failed</param>
    public void DownloadPresentationShortCode(string shortCode, string path, UnityAction<string> callbackOnSuccess, UnityAction<string> callbackOnFail)
    {
        StartCoroutine(MakeDownloadPresentationShortCode(shortCode, path, callbackOnSuccess, callbackOnFail));
    }

    IEnumerator MakeDownloadPresentationShortCode(string shortCode, string path, UnityAction<string> callbackOnSuccess, UnityAction<string> callbackOnFail)
    {
        UnityWebRequest request = UnityWebRequest.Get(baseurl + string.Format("/presentation/shortCode?shortCode={0}", Uri.EscapeDataString(shortCode)));
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

    public void UploadPresentation(string id, string path, UnityAction callbackOnSuccess, UnityAction<string> callbackOnFail)
    {
        StartCoroutine(MakeUploadPresentaion(id, path, callbackOnSuccess, callbackOnFail));
    }

    IEnumerator MakeUploadPresentaion(string id, string path, UnityAction callbackOnSuccess, UnityAction<string> callbackOnFail)
    {
        WWWForm form = new WWWForm();
        byte[] fileBytes = File.ReadAllBytes(path);
        form.AddBinaryData("presentation", fileBytes);
        form.AddField("idpresentation", id);
        UnityWebRequest request = UnityWebRequest.Post(baseurl + "/presentation/upload", form);
        request.SetRequestHeader("Authorization", "Bearer " + token);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
        {
            callbackOnFail?.Invoke(request.error);
        }
        else
        {
            callbackOnSuccess?.Invoke();
        }
    }

    /// <summary>
    /// Makes an API call to recieve the connection informations that belong to the shortCode.
    /// </summary>
    /// <param name="shortCode">The shortCode that is used to get the connection information</param>
    /// <param name="callbackOnSuccess">Will be called when the API call suceed</param>
    /// <param name="callbackOnFail">Will be called when the API call failed</param>
    public void GetConnectionInformation(string shortCode, UnityAction<ConnectionInformation> callbackOnSuccess, UnityAction<string> callbackOnFail)
    {
        StartCoroutine(MakeGetConnectionInformation(shortCode, callbackOnSuccess, callbackOnFail));
    }

    IEnumerator MakeGetConnectionInformation(string shortCode, UnityAction<ConnectionInformation> callbackOnSuccess, UnityAction<string> callbackOnFail)
    {
        UnityWebRequest request = UnityWebRequest.Get(baseurl + string.Format("/presentation/connectioninfos/shortCode?shortCode={0}", Uri.EscapeDataString(shortCode)));
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError)
        {
            callbackOnFail?.Invoke(request.error);
        }
        else
        {
            ConnectionInformation output = JsonConvert.DeserializeObject<ConnectionInformation>(request.downloadHandler.text);
            callbackOnSuccess?.Invoke(output);
        }
    }

    private void Awake()
    {
        if (BC != null)
        {
            GameObject.Destroy(BC);
            BC = this;
        }
        else
        {
            BC = this;
        }

        DontDestroyOnLoad(this);
    }
}
