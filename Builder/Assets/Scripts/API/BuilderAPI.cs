using UnityEngine;
using UnityEngine.Networking;

namespace APISystem
{
    public class BuilderAPI : API
    {
        public new string url { get; private set; } = "https://builder.api.meta-cloud.io";
        private string _createPath = "/builder/save";
        private string _loadPath = "/scene/fetch-by-id/{id}";
        private string _savePath = "/edit-scene/{id}";
        
        public BuilderAPI(MonoBehaviour invoker) : base(invoker) { }
        
        public void CreateScene(APIConnection.ConnectionCallback callback, SceneData data)
        {
            var endpoint = CreateEndpoint(url, _createPath);
            var json = JsonUtility.ToJson(data);
            _invoker.StartCoroutine(APIConnection.Send(APIConnection.Method.Post, endpoint, null, json));
        }
        
        public void SaveScene(APIConnection.ConnectionCallback callback, SaveSceneData data, int sceneId)
        {
            var endpoint = CreateEndpoint(url, _savePath, sceneId.ToString());
            var json = JsonUtility.ToJson(data);
            _invoker.StartCoroutine(APIConnection.Send(UnityWebRequest.kHttpVerbPUT, endpoint, callback, json, token));
        }

        public void LoadScene(APIConnection.ConnectionCallback callback, int sceneID)
        {
            var endpoint = CreateEndpoint(url, _loadPath, sceneID.ToString());
            _invoker.StartCoroutine(APIConnection.Send(APIConnection.Method.Get, endpoint, callback, null, token));
        }
    }
}

