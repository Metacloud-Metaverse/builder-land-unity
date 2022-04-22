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

        private AssetPacksArrayData _data;
        public AssetPackData[] assetPacksData { get { return _data.assetPacks; } }
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
        private List<Asset> _temporalAssets = new List<Asset>();
        public List<Asset> temporalAssets { get { return _temporalAssets; } }

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
        }


        public async void DownloadAssetPacksImages(Callback callback = null)
        {
            for (int i = 0; i < assetPacks.Length; i++)
            {
                await DownloadAssetPackImage(_data.assetPacks[i].image, assetPacks[i]);           
            }

            callback?.Invoke();
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
                foreach (var asset in _data.assetPacks[i].assets)
                {
                    _assetPacks[i].AddSection(asset.section);
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


        public async void DownloadAssetPack(int assetPackIndex, Callback callback = null)
        {
            var assetsData = _data.assetPacks[assetPackIndex].assets;
            for (int i = 0; i < assetsData.Length; i++)
            {
                if (_assetPacks[assetPackIndex].ExistsAsset(assetsData[i])) continue;

                switch (GetAssetType(assetsData[i].url))
                {
                    case ASSET_TYPE_TEXTURE:
                        await DownloadTexture(_assetPacks[assetPackIndex], assetsData[i]);
                        break;

                    case ASSET_TYPE_MESH:
                        await DownloadGLTF(_assetPacks[assetPackIndex], assetsData[i]);
                        break;
                }
            }
            assetPacksDownloaded[assetPackIndex] = true;
            callback?.Invoke();
        }


        public async void DownloadAssets(List<int> assetPackIndexes, List<AssetData> assetsData, Callback callback)
        {
            _temporalAssets.Clear();

            for (int i = 0; i < assetsData.Count; i++)
            {
                if (_assetPacks[assetPackIndexes[i]].ExistsAsset(assetsData[i]))
                {
                    _temporalAssets.Add(_assetPacks[assetPackIndexes[i]].GetAsset(assetsData[i].id));
                    continue;
                }
                switch (GetAssetType(assetsData[i].url))
                {
                    case ASSET_TYPE_TEXTURE:
                        await DownloadTexture(_assetPacks[assetPackIndexes[i]], assetsData[i], true);
                        break;

                    case ASSET_TYPE_MESH:
                        await DownloadGLTF(_assetPacks[assetPackIndexes[i]], assetsData[i], true);
                        break;
                }
            }

            callback?.Invoke();
        }


        private int GetAssetType(string url)
        {
            var splittedUrl = url.Split('.');
            var format = splittedUrl[splittedUrl.Length - 1];
            format = format.ToUpper();

            switch(format)
            {
                case "JPEG":
                    return ASSET_TYPE_TEXTURE;
                case "JPG":
                    return ASSET_TYPE_TEXTURE;
                case "PNG":
                    return ASSET_TYPE_TEXTURE;
                case "GLB":
                    return ASSET_TYPE_MESH;
                case "GLTF":
                    return ASSET_TYPE_MESH;
            }

            throw new SystemException("Format not allowed: " + format);
        }


        public void SpawnAsset(int assetPackIndex, int sectionIndex, int assetIndex)
        {
            var prefab = (GameObject)_assetPacks[assetPackIndex].sections[sectionIndex].assets[assetIndex].asset;
            SpawnAsset(prefab);
        }


        public void SpawnAsset(Asset asset)
        {
            var prefab = (GameObject)asset.asset;
            SpawnAsset(prefab);
        }


        public void SpawnAsset(GameObject prefab)
        {
            var go = Instantiate(prefab);
            go.SetActive(true);
            go.tag = "Selectable";
        }


        private async Task DownloadGLTF(AssetPack assetPack, AssetData assetData, bool temporal = false)
        {
            var gltf = new GltfImport();
            var success = await gltf.Load(assetData.url);
            GameObject go;

            if (success)
            {
                gltf.InstantiateMainScene(parentOfInstances);
                go = parentOfInstances.GetChild(0).gameObject;

                var data = go.AddComponent<Data>();
                data.url = assetData.url;
                go.transform.parent = null;

                AddColliders(go);

                var outline = go.AddComponent<Outline>();
                outline.OutlineColor = _outlineColor;
                outline.enabled = false;
                outline.OutlineWidth = _outlineWidth;

                go.AddComponent<PositionRestriction>();

                go.layer = Layers.SELECTABLE;

                go.SetActive(false);

                var asset = assetPack.AddAsset(assetData, go);
                if (temporal) _temporalAssets.Add(asset);
            }
            else
            {
                Debug.LogError("An error occurred while trying to download the gltf from " + assetData.url);
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


        private async Task DownloadTexture(AssetPack assetPack, AssetData assetData, bool temporal = false)
        {
            var www = UnityWebRequestTexture.GetTexture(assetData.url);
            await www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                FeedbackLabel.instance.ShowError(_connectionToServerFailedMessage);
            }
            else
            {
                Texture2D texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
                print(texture);
                var asset = assetPack.AddAsset(assetData, texture);
                if (temporal) _temporalAssets.Add(asset);
            }
        }


        public void GetUserAssetPacksData()
        {
            _data = JsonUtility.FromJson<AssetPacksArrayData>(json.text);
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
                        print(asset.name);
                        print(asset.id);
                        print("---");
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
                        if (asset == assetPack.sections[i].assets[j].asset)
                            return assetPack.sections[i].assets[j].url;
                    }
                }
            }
            return null;
        }

        
    }
}
