using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace APISystem
{
    public class APIConnection
    {
        public delegate void ConnectionCallback(string response);
        public enum Method { Get, Post };
        public static bool debugMode = true;


        /// <summary>
        /// Sends a HTTP request to the backend endpoint.
        /// </summary>
        /// <param name="method">HTTP method of the request.</param>
        /// <param name="endpoint">Complete URL of the endpoint.</param>
        /// <param name="callback">Method called after a successfull response.</param>
        /// <param name="data">The body of the request. Must be in JSON format.</param>
        /// <param name="authenticationToken">If a value is passed, adds an bearer token authorization header to the request.</param>
        public static IEnumerator Send(Method method, string endpoint, ConnectionCallback callback = null, string data = null, string authenticationToken = null)
        {
            if (debugMode) Debug.Log($"Sending {method} to {endpoint}");
            
            switch (method)
            {
                case Method.Get:
                    using (var www = new UnityWebRequest(endpoint, UnityWebRequest.kHttpVerbGET))
                    {
                        www.downloadHandler = new DownloadHandlerBuffer();
                        yield return SendWebRequest(www, callback, data, authenticationToken);
                    }
                    break;
                case Method.Post:
                    using (UnityWebRequest www = new UnityWebRequest(endpoint, UnityWebRequest.kHttpVerbPOST))
                    {
                        byte[] bodyRaw = Encoding.UTF8.GetBytes(data);
                        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                        www.downloadHandler = new DownloadHandlerBuffer();
                        yield return SendWebRequest(www, callback, data, authenticationToken);
                    }
                    break;
            }
        }


        private static IEnumerator SendWebRequest(UnityWebRequest www, ConnectionCallback callback = null, string data = null, string authenticationToken = null)
        {
            if (!string.IsNullOrEmpty(authenticationToken))
            {
                www.SetRequestHeader("Authorization", "Bearer " + authenticationToken);
            }

            if (!string.IsNullOrEmpty(data))
            {
                www.SetRequestHeader("Content-Type", "application/json");
            }

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(www.error);
            }
            
            var response = System.Text.Encoding.UTF8.GetString(www.downloadHandler.data);
            if (debugMode) Debug.Log($"Server response: {response}");
            callback?.Invoke(response);
            www.downloadHandler.Dispose();
            www.Dispose();
        }
    }
}

