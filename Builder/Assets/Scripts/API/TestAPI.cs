using System.Collections;
using System.Collections.Generic;
using System.Text;
using APISystem;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TestAPI : MonoBehaviour
{
    public Text responseText;

    void Start()
    {
    }

    void TestStringBetween()
    {
        string url = "/avatar/fetch/";
        var parts = url.Split('}');
        var parameters = new List<string>();

        for (int i = 0; i < parts.Length - 1; i++)
        {
            var index = parts[i].IndexOf('{') + 1;
            if (index != -1)
                parameters.Add(parts[i].Substring(index));
        }

        foreach (var p in parameters)
        {
            print(p);
        }
    }

    IEnumerator Login()
    {
        //var form = new WWWForm();
        //form.AddField("user_id", 4);
        //form.AddField("guest_private_secret", "AC_1804c7e43a5");

        var url = "https://user.api.meta-cloud.io/user/login-guest";
        var data = "{\"user_id\": 4, \"guest_private_secret\": \"AC_1804c7e43a5\"}";
        
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(data);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError(request.error);
        responseText.text = System.Text.Encoding.UTF8.GetString(request.downloadHandler.data);
    }

    IEnumerator Ping()
    {
        var request = new UnityWebRequest("https://user.api.meta-cloud.io");
        yield return request.SendWebRequest();
        
        print(request.responseCode);
    }

    IEnumerator CreateGuest()
    {
        var url = "https://user.api.meta-cloud.io/user/generate-guest";
        var www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
            Debug.LogError(www.error);
        Debug.Log(System.Text.Encoding.UTF8.GetString(www.downloadHandler.data));

    }

    IEnumerator SaveAvatar()
    {
        var url = "https://avatar.api.meta-cloud.io/avatar/save";
        var data = "{\"user_id\":4, \"data\":\"{ }\" }";
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(data);

        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjo0LCJndWVzdF9wcml2YXRlX3NlY3JldCI6IkFDXzE4MDRjN2U0M2E1IiwiaWF0IjoxNjUxNTA1MzQ2LCJleHAiOjE2NTE1MzQxNDZ9.j0r3M-gJwGdYpOeNEd50eqkOJzN1aHwLpnw7pmeG85I");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
            Debug.LogError(request.error);
        Debug.Log(System.Text.Encoding.UTF8.GetString(request.downloadHandler.data));
    }
}
