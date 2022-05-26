using UnityEngine;

namespace APISystem
{
    public class BuilderAPI : API
    {
        public new string url { get; private set; } = "https://user.api.meta-cloud.io";
        private string _savePath = "/builder/save";

        public BuilderAPI(MonoBehaviour invoker) : base(invoker) { }

        public void SaveScene(APIConnection.ConnectionCallback callback, SceneData data)
        {
            var endpoint = CreateEndpoint(url, _savePath);
            var json = JsonUtility.ToJson(data);
            _invoker.StartCoroutine(APIConnection.Send(APIConnection.Method.Post, endpoint, null, json));
        }

        public void LoadScene(APIConnection.ConnectionCallback callback)
        {

        }
    }
}

