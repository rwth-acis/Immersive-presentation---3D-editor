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
