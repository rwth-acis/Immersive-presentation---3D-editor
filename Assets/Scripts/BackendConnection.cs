using Newtonsoft.Json;
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

    private void Awake()
    {
        if (BC != null)
            GameObject.Destroy(BC);
        else
            BC = this;

        DontDestroyOnLoad(this);
    }
}
