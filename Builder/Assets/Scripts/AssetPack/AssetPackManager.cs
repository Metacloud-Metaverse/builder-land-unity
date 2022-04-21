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
        public delegate void Callback();

        public TextAsset json; //Testing only
        public Transform parentOfInstances;
        public bool isDownloading { get; private set; }

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
        private string _connectionToServerFailedMessage = "The connection to the server could not be established. Try again later.";

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
                FeedbackLabel.instance.ShowError(_connectionToServerFailedMessage);
            }
            else
            {
                var texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
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

                AddColliders(go);

                var outline = go.AddComponent<Outline>();
                outline.OutlineColor = _outlineColor;
                outline.enabled = false;
                outline.OutlineWidth = _outlineWidth;

                go.AddComponent<PositionRestriction>();

                go.layer = Layers.SELECTABLE;

                go.SetActive(false);

                assetPack.AddAsset(go, sectionName, url);
            }
            else
            {
                Debug.LogError("An error occurred while trying to download the gltf from " + url);
                FeedbackLabel.instance.ShowError(_connectionToServerFailedMessage);
            }
        }

        private void AddColliders(GameObject go)
        {
            var filters = go.transform.GetChild(0).gameObject.GetComponentsInChildren<MeshFilter>();

            if (filters.Length > 0)
            {
                for (int i = 0; i < filters.Length; i++)
                {
                    var collider = go.AddComponent<MeshCollider>();
                    collider.sharedMesh = filters[i].mesh;
                }


                //var renderers = go.transform.GetChild(0).gameObject.GetComponentsInChildren<Renderer>();
                //var boundBoxObject = new GameObject("Bound box");
                //boundBoxObject.transform.SetParent(go.transform);
                //boundBoxObject.transform.localPosition = Vector3.zero;
                //boundBoxObject.layer = Layers.BOUND_BOX;
                //var boundBox = boundBoxObject.AddComponent<BoundBox>();
                //boundBox.Initialize(renderers);
            }
            else
            {
                
                var renderers = go.transform.GetChild(0).gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();
                var colliders = new BoxCollider[renderers.Length];
                for (int i = 0; i < colliders.Length; i++)
                {
                    var collider = go.AddComponent<BoxCollider>();
                    colliders[i] = collider;
                }
                var resizeable = go.AddComponent<ResizeableBoxCollider>();
                resizeable.colliders = colliders;
                resizeable.renderers = renderers;
            }
        }

        private async Task DownloadTexture(string url, AssetPack assetPack, string sectionName)
        {
            var www = UnityWebRequestTexture.GetTexture(url);
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                FeedbackLabel.instance.ShowError(_connectionToServerFailedMessage);
            }
            else
            {
                Texture2D myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                assetPack.AddAsset(myTexture, sectionName, url);
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

        public string GetUrl(object asset)
        {
            foreach (var assetPack in _assetPacks)
            {
                for (int i = 0; i < assetPack.sections.Count; i++)
                {
                    for (int j = 0; j < assetPack.sections[i].assets.Count; j++)
                    {
                        if (asset == assetPack.sections[i].assets[j])
                            return assetPack.sections[i].urls[j];
                    }
                }
            }
            return null;
        }
    }
}
