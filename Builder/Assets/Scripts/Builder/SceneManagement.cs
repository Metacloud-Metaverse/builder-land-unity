using UnityEngine;
using AssetPacks;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class SceneManagement : MonoBehaviour
{
    private Outline _selectedObjectOutline;
    public PivotController pivot;
    public TransformModal transformModal;
    public Vector2Int chunks;
    public MeshRenderer floorPrefab;
    public Vector3 chunkSize;
    public Camera editorCamera;
    private static SceneManagement _instance;
    public static SceneManagement instance { get { return _instance; } }
    private AssetPackManager _apm;
    public string sceneName = "Untitled";
    private List<List<MeshRenderer>> _chunksRenderers = new List<List<MeshRenderer>>();
    private int _currentGroundId = -1;
    public float maxHeightCoef = 20;
    public float maxHeight { get; private set; }
    public int maxTriangles = 10000;
    public int maxMeshes = 300;
    private int _maxMaterials;
    public int maxMaterials
    {
        get { return _maxMaterials; }
        set
        {
            if (chunks.x + chunks.y > 2)
                _maxMaterials = (int)(Mathf.Log(chunks.x * chunks.y, 2) * value);
            else
                _maxMaterials = value;
        }
    }

    public int maxMaterialsCoef = 20;
    private int _maxTextures;
    public int maxTextures
    {
        get { return _maxTextures; }
        set
        {
            if (chunks.x + chunks.y > 2)
                _maxTextures = (int)(Mathf.Log(chunks.x * chunks.y, 2) * value);
            else
                _maxTextures = value;

        }

    }
    public int maxTexturesCoef = 10;
    public float cameraCoef = 5;
    public Vector3 initialCameraPos
    {
        get { return new Vector3(chunkSize.x / 2 * -1, 7.7f, chunkSize.z / 2 * -1); }
    }

    private int _trianglesCount;
    private int _meshesCount;
    private int _materialsCount;
    private int _texturesCount;

    public TextAsset json; //Testing only
    public AssetPackManager.Callback loadedSceneCallback;

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else
            Debug.LogError("There are more than one scene managemente in the scene");

        SetMaxHeight();
        maxMaterials = maxMaterialsCoef;
        maxTextures = maxTexturesCoef;
    }


    private void Start()
    {
        CreateChunks();
        SetCameraPosition();
        CreateWalls();
        //Invoke("Load", 6f);
    }

    private void CreateChunks()
    {
        float x = 0;
        float y = 0;
        float z = 0;

        for (int i = 0; i < chunks.y; i++)
        {
            var row = new List<MeshRenderer>();
            for (int j = 0; j < chunks.x; j++)
            {
                var chunk = Instantiate(floorPrefab);
                chunk.transform.position = new Vector3(x, y, z);
                x += chunkSize.x;
                row.Add(chunk);
            }
            x = 0;
            z += chunkSize.z;
            _chunksRenderers.Add(row);
        }
    }

    private float _oneChunkYPos = 7;
    private void SetCameraPosition()
    {
        editorCamera.transform.position = initialCameraPos + transform.right * chunkSize.x * chunks.x;
        editorCamera.transform.Translate(0, 0, -chunkSize.z * ((chunks.y + chunks.x) / cameraCoef));
        if (chunks.x * chunks.y == 1)
            editorCamera.transform.position = new Vector3(editorCamera.transform.position.x, _oneChunkYPos, editorCamera.transform.position.z);
    }

    public void ResetChunks()
    {
        ResetChunks(3, 2);
    }

    public void ResetChunks(int x, int y)
    {
        for (int i = 0; i < _chunksRenderers.Count; i++)
        {
            for (int j = 0; j < _chunksRenderers[i].Count; j++)
            {
                Destroy(_chunksRenderers[i][j].gameObject);
            }
            _chunksRenderers[i].Clear();
        }
        _chunksRenderers.Clear();

        chunks.x = x;
        chunks.y = y;

        CreateChunks();
        SetCameraPosition();
    }

    private void SetMaxHeight()
    {
        maxHeight = Mathf.Log((chunks.x * chunks.y) + 1, 2) * maxHeightCoef;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask = 1 << Layers.SELECTABLE;

            if (Physics.Raycast(ray, out hit, 1000, layerMask))
            {
                print(hit.transform.gameObject.name);

                if (hit.transform.tag == "Selectable")
                {
                    if (TransformModal.instance != null && TransformModal.instance.isMouseInside) return;

                    if (_selectedObjectOutline != null) _selectedObjectOutline.enabled = false;
                    _selectedObjectOutline = hit.transform.GetComponent<Outline>();
                    _selectedObjectOutline.enabled = true;
                    //pivot.gameObject.SetActive(true);
                    //pivot.transform.position = _selectedObjectOutline.transform.position;
                    //pivot.transformableObject = _selectedObjectOutline.gameObject;
                    transformModal.gameObject.SetActive(true);
                    transformModal.SetTarget(_selectedObjectOutline.transform);
                }
                else
                    UnselectObject();
            }
            else if (TransformModal.instance != null && TransformModal.instance.isMouseInside) return;

            else
            {
                UnselectObject();
            }
        }
    }


    private void UnselectObject()
    {
        if (TransformModal.instance != null && !TransformModal.instance.isMouseInside)
        {
            print(_selectedObjectOutline);

            if (_selectedObjectOutline != null)
                _selectedObjectOutline.enabled = false;
            transformModal.gameObject.SetActive(false);
        }
    }


    public void DeleteObject()
    {
        if (_selectedObjectOutline.gameObject == null) return;

        Destroy(_selectedObjectOutline.gameObject);
    }


    public void SetFloorTexture(Texture2D texture)
    {
        foreach (var row in _chunksRenderers)
        {
            foreach (var renderer in row)
            {
                renderer.material.mainTexture = texture;
                renderer.material.color = Color.white;
            }
        }
    }

    public void SetFloorTexture(int assetId)
    {
        var asset = AssetPackManager.instance.GetAsset(assetId);
        var texture = (Texture2D)asset.asset;
        _currentGroundId = assetId;

        SetFloorTexture(texture);
    }

    public Transform GetChunk(int x, int y)
    {
        print($"{x}, {y}");
        return _chunksRenderers[y][x].transform;
    }


    public void Save()
    {
        if (!ValidateCounts())
        {
            FeedbackLabel.instance.ShowError("There are too many elements in the scene.");
            return;
        }

        var gameObjects = GameObject.FindGameObjectsWithTag("Selectable");
        var correctGameObjects = GetCorrectGameObjects(gameObjects);
        var data = new SceneData();
        data.name = sceneName;

        data.chunksX = chunks.x;
        data.chunksY = chunks.y;

        data.ground = new SharedObjectData();

        if (_currentGroundId != -1)
        {
            var textureAsset = AssetPackManager.instance.GetAsset(_currentGroundId);
            if (textureAsset != null)
            {
                data.ground.url = textureAsset.url;
                data.ground.id = textureAsset.id;
                data.ground.name = textureAsset.name;
                data.ground.tags = textureAsset.tags;
            }
        }
        else
        {
            data.ground.id = _currentGroundId;
        }

        var dataComponents = new List<Data>();
        foreach (var correctGameObject in correctGameObjects)
        {
            dataComponents.Add(correctGameObject.GetComponent<Data>());
        }

        var dataDictionary = CreateObjectDataDictionary(dataComponents);
        data.sharedObjects = new SharedObjectData[dataDictionary.Count];

        int i = 0;
        foreach (var dataList in dataDictionary)
        {
            var asset = AssetPackManager.instance.GetAsset(dataList.Value[0].meshId);
            AssetPackManager.instance.PrintAssetPacks();
            var sharedObject = new SharedObjectData
            {
                id = dataList.Key,
                url = asset.url,
                name = asset.name,
                tags = asset.tags,
                objectsData = new SceneObjectData[dataList.Value.Count]
            };

            for (int j = 0; j < dataList.Value.Count; j++)
            {
                var objectData = new SceneObjectData();
                var dataComponent = dataList.Value[j];

                dataComponent.SetChunkParent();

                objectData.positionX = dataComponent.transform.localPosition.x;
                objectData.positionY = dataComponent.transform.localPosition.y;
                objectData.positionZ = dataComponent.transform.localPosition.z;

                objectData.rotationX = dataComponent.transform.localRotation.x;
                objectData.rotationY = dataComponent.transform.localRotation.y;
                objectData.rotationZ = dataComponent.transform.localRotation.z;

                objectData.scaleX = dataComponent.transform.localScale.x;
                objectData.scaleY = dataComponent.transform.localScale.y;
                objectData.scaleZ = dataComponent.transform.localScale.z;

                objectData.chunkX = dataComponent.chunkX;
                objectData.chunkY = dataComponent.chunkY;

                sharedObject.objectsData[j] = objectData;
            }
            data.sharedObjects[i] = sharedObject;
            i++;
        }

        var json = JsonUtility.ToJson(data);
        print(json);
    }


    public void Load()
    {
        var sceneData = JsonUtility.FromJson<SceneData>(json.text);
        print(sceneData);
        Load(sceneData);
    }

    private SceneData _sceneData;
    public void Load(SceneData data)
    {
        _sceneData = data;
        ResetChunks(_sceneData.chunksX, _sceneData.chunksY);
        DestroySceneObjects();
        AssetPackManager.instance.LoadSceneAndCreateAssetPack(data, LoadedSceneCallback);
    }

    private void DestroySceneObjects()
    {
        var selectables = GameObject.FindGameObjectsWithTag("Selectable");
        foreach (var selectable in selectables)
        {
            Destroy(selectable);
        }
    }

    private void LoadedSceneCallback()
    {
        if(_sceneData.ground.id != -1) SetFloorTexture(_sceneData.ground.id);
        loadedSceneCallback();
    }


    private Dictionary<int, List<Data>> CreateObjectDataDictionary(List<Data> dataComponents)
    {
        var dictionary = new Dictionary<int, List<Data>>();

        foreach (var data in dataComponents)
        {
            if (!dictionary.ContainsKey(data.meshId))
                dictionary.Add(data.meshId, new List<Data>());

            dictionary[data.meshId].Add(data);
        }

        return dictionary;
    }


    private List<GameObject> GetCorrectGameObjects(GameObject[] gameObjects)
    {
        var correctGameObjects = new List<GameObject>();
        foreach (var go in gameObjects)
        {
            if (!go.GetComponent<PositionRestriction>().isProhibited)
                correctGameObjects.Add(go);
        }

        return correctGameObjects;
    }

    public Vector3 GetMiddlePosition()
    {
        return new Vector3((chunks.x - 1) * chunkSize.x / 2, 0, (chunks.y - 1) * chunkSize.z / 2);
    }

    private void CreateWalls()
    {

        CreateWall(
            "Wall N",
            new Vector3(chunkSize.x / 2 * -1, 0, chunkSize.z / 2 * (chunks.y - 1)),
            new Vector3(1, maxHeight, chunkSize.z * chunks.y),
            Vector3.zero,
            new Vector3(-0.5f, 0.5f, 0)
        );

        CreateWall(
            "Wall W",
            new Vector3(chunkSize.x / 2 * (chunks.x - 1), 0, chunkSize.z / 2 * -1),
            new Vector3(1, maxHeight, chunkSize.x * chunks.x),
            new Vector3(0, 90, 0),
            new Vector3(0.5f, 0.5f, 0)

        );

        CreateWall(
            "Wall E",
            new Vector3(chunkSize.x / 2 * (chunks.x - 1), 0, chunkSize.z * chunks.y - chunkSize.z / 2),
            new Vector3(1, maxHeight, chunkSize.x * chunks.x),
            new Vector3(0, 90, 0),
            new Vector3(-0.5f, 0.5f, 0)
        );

        CreateWall(
            "Wall S",
            new Vector3(chunkSize.x * chunks.x - chunkSize.x / 2, 0, chunkSize.z / 2 * (chunks.y - 1)),
            new Vector3(1, maxHeight, chunkSize.z * chunks.y),
            Vector3.zero,
            new Vector3(0.5f, 0.5f, 0)
        );

        CreateWall(
            "Wall B",
            new Vector3(chunkSize.x / 2 * (chunks.x - 1), -1, chunkSize.z / 2 * (chunks.y - 1)),
            new Vector3(chunkSize.x * chunks.x, 1, chunkSize.z * chunks.y),
            Vector3.zero,
            new Vector3(0, -0.5f, 0)
        );

        CreateWall(
            "Wall U",
            new Vector3(chunkSize.x / 2 * (chunks.x - 1), maxHeight, chunkSize.z / 2 * (chunks.y - 1)),
            new Vector3(chunkSize.x * chunks.x, 1, chunkSize.z * chunks.y),
            Vector3.zero,
            new Vector3(0, 0.5f, 0)
        );
    }


    private void CreateWall(string name, Vector3 position, Vector3 scale, Vector3 rotation, Vector3 center)
    {
        var wall = new GameObject(name);

        var collider = wall.AddComponent<BoxCollider>();
        collider.center = center;
        collider.isTrigger = true;

        wall.transform.position = position;
        wall.transform.localScale = scale;
        wall.transform.eulerAngles = rotation;

        wall.AddComponent<Wall>();

        var rb = wall.AddComponent<Rigidbody>();
        rb.isKinematic = true;
    }


    public int GetTrianglesCount(GameObject[] selectables = null)
    {
        if (selectables == null) selectables = GameObject.FindGameObjectsWithTag("Selectable");
        int triangles = 0;

        foreach (var selectable in selectables)
        {
            var meshFilters = selectable.GetComponentsInChildren<MeshFilter>();
            foreach (var meshFilter in meshFilters)
            {
                triangles += meshFilter.mesh.triangles.Length;
            }

            var renderers = selectable.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (var renderer in renderers)
            {
                triangles += renderer.sharedMesh.triangles.Length;
            }
        }

        return triangles;
    }


    public int GetMeshesCount(GameObject[] selectables = null)
    {
        if (selectables == null) selectables = GameObject.FindGameObjectsWithTag("Selectable");

        int meshesCount = 0;
        foreach (var selectable in selectables)
        {
            var meshFilters = selectable.GetComponentsInChildren<MeshFilter>();
            foreach (var meshFilter in meshFilters)
            {
                meshesCount++;
            }

            var renderers = selectable.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (var renderer in renderers)
            {
                meshesCount++;
            }
        }

        return meshesCount;
    }


    public int GetTexturesCount(GameObject[] selectables = null)
    {
        if (selectables == null) selectables = GameObject.FindGameObjectsWithTag("Selectable");
        int textureCount = 0;
        foreach (var selectable in selectables)
        {
            var renderers = selectable.GetComponentsInChildren<Renderer>();
            foreach (var renderer in renderers)
            {
                var nameIds = renderer.material.GetTexturePropertyNameIDs();
                foreach (var nameId in nameIds)
                {
                    var tex = renderer.material.GetTexture(nameId);
                    if (tex != null)
                        textureCount++;
                }       
            }
        }
        return textureCount;
    }


    public int GetMaterialsCount(GameObject[] selectables = null)
    {
        if (selectables == null) selectables = GameObject.FindGameObjectsWithTag("Selectable");
        int materialsCount = 0;

        foreach (var selectable in selectables)
        {
            var renderers = selectable.GetComponentsInChildren<Renderer>();
            materialsCount += renderers.Length;
        }

        return materialsCount;
    }


    public bool ValidateCounts()
    {
        var selectables = GameObject.FindGameObjectsWithTag("Selectable");

        if (GetTrianglesCount(selectables) <= maxTriangles &&
            GetMeshesCount   (selectables) <= maxMeshes    &&
            GetMaterialsCount(selectables) <= maxMaterials &&
            GetTexturesCount (selectables) <= maxTextures)
            return true;

        return false;
    }
}
