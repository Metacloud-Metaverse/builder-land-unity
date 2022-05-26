using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using APISystem;
using System.IO;

public class UserAPIHandler
{
    private static UsersAPI _api;
    private Client _client;
    public delegate void Callback();
    public Callback loginCallback;

    public UserAPIHandler(Client client)
    {
        _client = client;
        _api = new UsersAPI(_client);
    }

    public void CreateGuestAndLogin()
    {
        _api.CreateGuestAndConnect(CreateGuestCallback, LoginGuestCallback);
    }

    public void CreateGuest()
    {
        _api.CreateGuest(CreateGuestCallback);
    }

    public void CreateGuestCallback(string response)
    {
        var userData = JsonUtility.FromJson<CreateGuestResponseData>(response).data;
        _client.id = userData.id;
        _client.name = userData.username;
        _client.password = userData.guest_private_secret;
    }


    public void LoginGuest()
    {
        _api.LoginGuest(_client.id, _client.password, LoginGuestCallback);
    }

    public void LoginGuestCallback(string response)
    {
        var loginGuestResponse = JsonUtility.FromJson<LoginGuestResponseData>(response);
        _client.accessToken = loginGuestResponse.data.access_token;
        loginCallback();
    }


}
