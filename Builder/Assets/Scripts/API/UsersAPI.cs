using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

namespace APISystem
{
    public class UsersAPI: API
    {
        public new string url { get; private set; } = "https://user.api.meta-cloud.io";
        private string _loginGuestPath = "/user/login-guest";
        private string _createGuestPath = "/user/generate-guest";

        public UsersAPI(MonoBehaviour invoker) : base(invoker) { }
        
        public void CreateGuestAndConnect(APIConnection.ConnectionCallback createGuestCallback, APIConnection.ConnectionCallback loginCallback)
        {
            _invoker.StartCoroutine(CreateGuestAndConnectCoroutine(createGuestCallback, loginCallback));
        }


        private IEnumerator CreateGuestAndConnectCoroutine(APIConnection.ConnectionCallback createGuestCallback, APIConnection.ConnectionCallback loginCallback)
        {
            var endpointCreateGuestAPI = CreateEndpoint(url, _createGuestPath);
            yield return APIConnection.Send(APIConnection.Method.Get, endpointCreateGuestAPI, createGuestCallback);

            var credentials = new LoginGuestSendData();
            credentials.user_id = Client.Instance.id;
            credentials.guest_private_secret = Client.Instance.password;
            var data = JsonUtility.ToJson(credentials);
            var endpointLogin = CreateEndpoint(url, _loginGuestPath);
            yield return APIConnection.Send(APIConnection.Method.Post, endpointLogin, loginCallback, data);
        }


        public void CreateGuest(APIConnection.ConnectionCallback callback)
        {
            var endpoint = CreateEndpoint(url, _createGuestPath);
            _invoker.StartCoroutine(APIConnection.Send(APIConnection.Method.Get, endpoint, callback));
        }


        public void LoginGuest(int id, string password, APIConnection.ConnectionCallback callback)
        {
            var credentials = new LoginGuestSendData();
            credentials.user_id = id;
            credentials.guest_private_secret = password;
            var data = JsonUtility.ToJson(credentials);
            var endpoint = CreateEndpoint(url, _loginGuestPath);
            _invoker.StartCoroutine(APIConnection.Send(APIConnection.Method.Post, endpoint, callback, data));
        }
    }
}
