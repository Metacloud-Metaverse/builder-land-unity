using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLTFast;
using UnityEngine.Networking;

namespace AssetPacks
{
    public class AssetPackManager : MonoBehaviour
    {
        public TextAsset json; //Testing only
        public Thumbnail thumbnail; //Testing only

        public Transform parentOfInstances;
        private AssetPacksData _data;
        private AssetPack[] _assetPacks;

        public const int ASSET_TYPE_TEXTURE = 0;
        public const int ASSET_TYPE_MESH = 1;

        private void Start()
        {
            GetUserAssetPacksData();
            DownloadAssetPack();
            Invoke("PrintAssetPacks", 5);
        }

        public void DownloadAssetPack()
        {
            _assetPacks = new AssetPack[_data.assetPacks.Length];

            for (int i = 0; i < _assetPacks.Length; i++)
            {
                _assetPacks[i] = new AssetPack();
            }

            for (int i = 0; i < _assetPacks.Length; i++)
            {
                _assetPacks[i].name = _data.assetPacks[i].name;

                var urls = _data.assetPacks[i].urls;

                for (int j = 0; j < urls.Length; j++)
                {
                    switch (_data.assetPacks[i].types[j])
                    {
                        case ASSET_TYPE_TEXTURE:
                            DownloadTexture(urls[j], _assetPacks[i], _data.assetPacks[i].sections[j]);
                            break;
                        case ASSET_TYPE_MESH:
                            DownloadGLTF(urls[j], _assetPacks[i], _data.assetPacks[i].sections[j]);
                            break;
                    }
                }
            }
        }

        private async void DownloadGLTF(string url, AssetPack assetPack, string sectionName)
        {
            var gltf = new GltfImport();
            var success = await gltf.Load(url);
            GameObject go;
            if (success)
            {
                gltf.InstantiateMainScene(parentOfInstances);
                go = parentOfInstances.GetChild(0).gameObject;
                var data = go.AddComponent<Data>();
                data.url = url;
                go.transform.parent = null;
                assetPack.AddAsset(go, sectionName);
            }
            else
                Debug.LogError("An error occurred while trying to download the gltf from " + url);       
        }


        private void DownloadTexture(string url, AssetPack assetPack, string sectionName)
        {
            StartCoroutine(GetTexture(url, assetPack, sectionName));
        }

        IEnumerator GetTexture(string url, AssetPack assetPack, string sectionName)
        {
            UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                assetPack.AddAsset(myTexture, sectionName);
            }
        }

        public void GetUserAssetPacksData()
        {
            _data = JsonUtility.FromJson<AssetPacksData>(json.text);
        }

        private void PrintAssetPacks()
        {
            foreach (var assetPack in _assetPacks)
            {
                print("Asset Pack: " + assetPack.name);
                foreach (var section in assetPack.sections)
                {
                    print("Section: " + section.name);
                    foreach (var asset in section.assets)
                    {
                        print(asset);
                    }
                }
                print("=========================");
            }
        }
    }
}
