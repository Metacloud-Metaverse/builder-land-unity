using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace APISystem
{
    public class APIConnection
    {
        public delegate void ConnectionCallback(string response);
        public enum Method { Get, Post, Put };
        public static bool debugMode = true;
        
        /// <summary>
        /// Sends a HTTP request to the backend endpoint.
        /// </summary>
        /// <param name="method">HTTP method of the request.</param>
        /// <param name="endpoint">Complete URL of the endpoint.</param>
        /// <param name="callback">Method called after a successful response.</param>
        /// <param name="data">The body of the request. Must be in JSON format.</param>
        /// <param name="authenticationToken">If a value is passed, adds an bearer token authorization header to the request.</param>
        [System.Obsolete("Use function Send with string verb param.")]
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
        
        /// <summary>
        /// Sends a HTTP request to the backend endpoint.
        /// </summary>
        /// <param name="verb">HTTP verb of the request. You may use the constant strings in UnityWebRequest class.</param>
        /// <param name="endpoint">Complete URL of the endpoint.</param>
        /// <param name="callback">Method called after a successful response.</param>
        /// <param name="data">The body of the request. Must be in JSON format.</param>
        /// <param name="authenticationToken">If a value is passed, adds an bearer token authorization header to the request.</param>
        public static IEnumerator Send(string verb, string endpoint, ConnectionCallback callback = null, string data = null, string authenticationToken = null)
        {
            if (verb is UnityWebRequest.kHttpVerbHEAD or UnityWebRequest.kHttpVerbCREATE or UnityWebRequest.kHttpVerbDELETE)
                throw new Exception("Verb sent still not developed to send request. Only allowed verbs GET, POST and PUT");
            
            using (UnityWebRequest www = new UnityWebRequest(endpoint, verb))
            {
                if (verb is UnityWebRequest.kHttpVerbPOST or UnityWebRequest.kHttpVerbPUT)
                    SetUploadHandler(www, data);
                www.downloadHandler = new DownloadHandlerBuffer();
                yield return SendWebRequest(www, callback, data, authenticationToken);
            }
        }
        
        private static void SetUploadHandler(UnityWebRequest www, string data)
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(data);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
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

