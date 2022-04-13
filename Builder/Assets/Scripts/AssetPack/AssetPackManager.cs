using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GLTFast;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;

namespace AssetPacks
{
    public class AssetPackManager : MonoBehaviour
    {
        public delegate void Hook();
        public delegate void Callback();

        public TextAsset json; //Testing only
        public Transform parentOfInstances;
        public bool isDownloading { get; private set; }

        private List<Hook> _downloadFinishedHooks = new List<Hook>();
        private AssetPacksData _data;
        private AssetPack[] _assetPacks;
        public AssetPack[] assetPacks { get { return _assetPacks; } }
        public bool[] assetPacksDownloaded { get; private set; }
        private Color _outlineColor = new Color(1, 0.6f, 0);
        private float _outlineWidth = 5f;
        private static AssetPackManager _instance;
        public static AssetPackManager instance { get { return _instance; } }
        public const int ASSET_TYPE_TEXTURE = 0;
        public const int ASSET_TYPE_MESH = 1;

        private void Awake()
        {
            if (_instance == null) _instance = this;
            else
                Debug.LogError("There are more than one Asset Pack Manager in scene");
        }

        private void Start()
        {
            GetUserAssetPacksData();
            CreateAssetPackStructure();
            //DownloadAssetPack(0);
        }

        public async void DownloadAssetPacksImages(Callback callback = null)
        {
            for (int i = 0; i < assetPacks.Length; i++)
            {
                await DownloadAssetPackImage(_data.assetPacks[i].image, assetPacks[i]);           
            }

            if(callback != null)
                callback();
        }

        private async Task DownloadAssetPackImage(string url, AssetPack assetPack)
        {
            var www = UnityWebRequestTexture.GetTexture(url);
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                print(sprite);
                assetPack.image = sprite;
            }
        }

        public void CreateAssetPackStructure()
        {
            _assetPacks = new AssetPack[_data.assetPacks.Length];
            assetPacksDownloaded = new bool[_data.assetPacks.Length];

            for (int i = 0; i < _assetPacks.Length; i++)
            {
                _assetPacks[i] = new AssetPack();
                _assetPacks[i].name = _data.assetPacks[i].name;
                foreach (var section in _data.assetPacks[i].sections)
                {
                    _assetPacks[i].AddSection(section);
                }
            }
        }

        public void DownloadAssetPacks()
        {
            for (int i = 0; i < _assetPacks.Length; i++)
            {
                DownloadAssetPack(i);
            }
        }


        public async void DownloadAssetPack(int i, Callback callback = null)
        {
            var urls = _data.assetPacks[i].urls;

            for (int j = 0; j < urls.Length; j++)
            {
                switch (_data.assetPacks[i].types[j])
                {
                    case ASSET_TYPE_TEXTURE:
                        await DownloadTexture(urls[j], _assetPacks[i], _data.assetPacks[i].sections[j]);
                        break;

                    case ASSET_TYPE_MESH:
                        await DownloadGLTF(urls[j], _assetPacks[i], _data.assetPacks[i].sections[j]);
                        break;
                }
            }
            assetPacksDownloaded[i] = true;
            if(callback != null)
                callback();
        }


        public void SpawnAsset(int assetPackIndex, int sectionIndex, int assetIndex)
        {
            var prefab = (GameObject)_assetPacks[assetPackIndex].sections[sectionIndex].assets[assetIndex];
            var go = Instantiate(prefab);
            go.SetActive(true);
            go.tag = "Selectable";
        }

        private async Task DownloadGLTF(string url, AssetPack assetPack, string sectionName)
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
                
                var collider = go.AddComponent<MeshCollider>();
                var filter = go.transform.GetChild(0).gameObject.GetComponentInChildren<MeshFilter>();
                if(filter == null)          
                    collider.sharedMesh = go.transform.GetChild(0).gameObject.GetComponentInChildren<SkinnedMeshRenderer>().sharedMesh;
                
                else
                    collider.sharedMesh = filter.mesh;

                var outline = go.AddComponent<Outline>();
                outline.OutlineColor = _outlineColor;
                outline.enabled = false;
                outline.OutlineWidth = _outlineWidth;

                go.SetActive(false);

                assetPack.AddAsset(go, sectionName);
            }
            else
            {
                Debug.LogError("An error occurred while trying to download the gltf from " + url);
            }
        }


        private async Task DownloadTexture(string url, AssetPack assetPack, string sectionName)
        {
            var www = UnityWebRequestTexture.GetTexture(url);
            await www.SendWebRequest();

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

        public void PrintAssetPacks()
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
