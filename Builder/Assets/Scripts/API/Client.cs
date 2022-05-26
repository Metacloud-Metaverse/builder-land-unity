using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using APISystem;

public class Client: MonoBehaviourSingleton<Client>
{
    public delegate void LoginCallback();
    public LoginCallback loginCallback;
    public new string name;
    public string accessToken;
    public string password;
    public int id;
    private UserAPIHandler _userAPIHandler;
    
    //test client:
    //name  user_1042
    //token eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1c2VyX2lkIjo0MiwiZ3Vlc3RfcHJpdmF0ZV9zZWNyZXQiOiJBQ18xODA5NGMyYTQ4NCIsImlhdCI6MTY1MTc2MzIyNiwiZXhwIjoxNjUxNzkyMDI2fQ.-zzIh5TkvLDPsBpInER5eDlZXGwerx1tHY7SxohgufA
    //pass  AC_18094c2a484
    //id    42

    private void Start()
    {
        print(Application.persistentDataPath);

        _userAPIHandler = new UserAPIHandler(this);
        _userAPIHandler.loginCallback = CallAfterLogin;
        if (LoadDataLocal())
            LoginGuest();

        else
            CreateGuestAndLogin();
    }

    public void CreateGuestAndLogin()
    {
        _userAPIHandler.CreateGuestAndLogin();
    }

    public void CreateGuest()
    {
        _userAPIHandler.CreateGuest();
    }


    public void LoginGuest()
    {
        _userAPIHandler.LoginGuest();
    }

    string path {
        get {
            return Application.persistentDataPath + Path.DirectorySeparatorChar + "user-data.json";
        }
    }

    private void CallAfterLogin()
    {
        SaveDataLocal();
        loginCallback();
        //CheckAvatarCreated();
    }

    //private void CheckAvatarCreatedCallback(string json)
    //{
    //    var response = JsonUtility.FromJson<LoginGuestResponseData>(json);
    //    if (response.statusCode == "SUCCESS")
    //        SceneLoader.Instance.LoadScene(Scenes.WORLD);
    //    else
    //        SceneLoader.Instance.LoadScene(Scenes.SIGN_UP);
    //}

    //private void CheckAvatarCreated()
    //{
    //    var endpoint = API.CreateEndpoint(AvatarAPI.url, AvatarAPI.fetchDataPath, id.ToString());
    //    StartCoroutine(APIConnection.Send(APIConnection.Method.Get, endpoint, CheckAvatarCreatedCallback));
    //}


    private void SaveDataLocal()
    {
        var clientData = new ClientData
        {
            id = id,
            accessToken = accessToken,
            password = password,
            username = name
        };

        var json = JsonUtility.ToJson(clientData);
        using StreamWriter writer = new StreamWriter(path);
        writer.Write(json);
    }

    private bool LoadDataLocal()
    {
        if (!File.Exists(path)) return false;

        using StreamReader reader = new StreamReader(path);
        var json = reader.ReadToEnd();
        var clientData = JsonUtility.FromJson<ClientData>(json);
        id = clientData.id;
        accessToken = clientData.accessToken;
        password = clientData.password;
        name = clientData.username;

        return true;
    }

}


