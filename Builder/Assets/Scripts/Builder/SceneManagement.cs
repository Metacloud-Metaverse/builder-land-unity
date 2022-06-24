using System;
using UnityEngine;
using AssetPacks;
using System.Collections.Generic;
using APISystem;

public class SceneManagement : MonoBehaviour
{
    private Outline _selectedObjectOutline;
    public PivotController pivot;
    public TransformModal transformModal;
    public Vector2Int chunks;
    public MeshRenderer floorPrefab;
    public Vector3 chunkSize;
    public Camera editorCamera;
    public Camera menuCamera;
    public Canvas menuCanvas;
    public Canvas previewCanvas;
    public GameObject fpc;
    public Navbar navbar;
    public GameObject floorRuler;
    
    public static SceneManagement Instance { get; private set; }
    private AssetPackManager _apm;
    public string sceneName = "Untitled";
    private List<List<MeshRenderer>> _chunksRenderers = new List<List<MeshRenderer>>();
    private int _currentGroundId = -1;
    public float maxHeightCoef = 20;
    public float maxHeight { get; private set; }
    public int maxTrianglesCoef = 10000;
    public int maxMeshesCoef = 300;
    private int _maxTriangles;
    public bool activeEditor = true;
    
    public int maxTriangles
    {
        get => _maxTriangles;
        private set => _maxTriangles = chunks.x * chunks.y * value;
    }

    private int _maxMeshes;

    public int maxMeshes
    {
        get => _maxMeshes;
        private set => _maxMeshes = chunks.x * chunks.y * value;
    }
    private int _maxMaterials;
    private bool _previewMode;
    private BuilderAPI _builderAPI;
    
    public int maxMaterials
    {
        get { return _maxMaterials; }
        set
        {
            // if (chunks.x + chunks.y > 2)
            //     _maxMaterials = (int)(Mathf.Log(chunks.x * chunks.y, 2) * value);
            // else
            //     _maxMaterials = value;
            _maxMaterials = (int)(Mathf.Log(chunks.x * chunks.y + 1, 2) * value);
        }
    }

    public int maxMaterialsCoef = 20;
    private int _maxTextures;
    public int maxTextures
    {
        get { return _maxTextures; }
        set
        {
            // if (chunks.x + chunks.y > 2)
            //     _maxTextures = (int)(Mathf.Log(chunks.x * chunks.y, 2) * value);
            // else
            //     _maxTextures = value;
            _maxTextures = (int)(Mathf.Log(chunks.x * chunks.y + 1, 2) * value);
        }
    }
    public int maxTexturesCoef = 10;
    public float cameraCoef = 5;
    public Vector3 initialCameraPos => new Vector3(chunkSize.x / 2 * -1, 7.7f, chunkSize.z / 2 * -1);

    private int _trianglesCount;
    private int _meshesCount;
    private int _materialsCount;
    private int _texturesCount;
    private UsersAPI _usersAPI;
    public TextAsset json; //Testing only
    public AssetPackManager.Callback loadedSceneCallback;
    
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
            Debug.LogError("There are more than one scene management in the scene");

        SetMaxHeight();
        maxMaterials = maxMaterialsCoef;
        maxTextures = maxTexturesCoef;
        maxMeshes = maxMeshesCoef;
        maxTriangles = maxTrianglesCoef;
        
        _usersAPI = new UsersAPI(this);
        _builderAPI = new BuilderAPI(this);
    }

    private int _sceneId;
    private void Start()
    {
        BuilderUI.Instance.loadingSceneLabel.SetActive(true);
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            var credentials = GetUserAndPass();
            if (credentials.user == -1 || string.IsNullOrEmpty(credentials.pass))
            {
                FeedbackLabelManager.Instance.ShowError("No credentials were passed. To test credentials, add them to the url as follows: \"?id=0&user=1&pass=secret\". This feature will only be available during development.");
                InitializeBuilder();
            }
            else
                LoginAndStart(credentials);
        }
        else
        {
            StartEditor();
        }
    }

    private string _token;

    private void LoginAndStart(Credentials credentials)
    {
        // var userApi = new UsersAPI(this);
        // _usersAPI.LoginGuest(credentials.user, credentials.pass, LoginCallback);
    }

    public void Login(int user, string pass, APIConnection.ConnectionCallback callback = null)
    {
        var connectionCallback = callback ?? LoginCallback;
        _usersAPI.LoginGuest(user, pass, connectionCallback);
    }
    
    public void CreateGuest(APIConnection.ConnectionCallback callback)
    {
        _usersAPI.CreateGuest(callback);
    }
    
    private void LoginCallback(string json)
    {
        Debug.Log("Login callback " + json);
        var responseData = JsonUtility.FromJson<LoginGuestResponseData>(json);
        if (responseData.statusCode == StatusCode.SUCCESS)
        {
            _token = responseData.data.access_token;
            StartWithSceneData();
        }
        else
        {
            FeedbackLabelManager.Instance.ShowError(responseData.message);
            FeedbackLabelManager.Instance.ShowError("Not all features will be available.");
            InitializeBuilder();
        }
    }

    public struct Credentials
    {
        public int user;
        public string pass;
    }

    
    private Credentials GetUserAndPass()
    {
        Debug.Log("Get user and pass");
        Credentials credentials;
        credentials.user = Browser.GetUser();
        credentials.pass = Browser.GetPass();
        return credentials;
    }
    
    private void StartNoIndex()
    {
        Debug.Log("StartNoIndex");
        FeedbackLabelManager.Instance.ShowError("No scene index passed. Started for testing only. Not all function will be available.");
        InitializeBuilder();
    }

    private void StartEditor()
    {
        FeedbackLabelManager.Instance.ShowSuccess("Started in editor mode.");
        InitializeBuilder();
    }

    
    private void StartTrue()
    {
        Debug.Log("Start True");
        _builderAPI.token = _token;
        _builderAPI.LoadScene(LoadSceneCallback, _sceneId);
    }

    private void StartWithSceneData()
    {
        Debug.Log("Start with scene Data");
        _sceneId = Browser.GetSceneId();
        if (_sceneId == -1)
            StartNoIndex();
        else
            StartTrue();
    }

    private void LoadSceneCallback(string json)
    {
        Debug.Log("Load scene callback" + json);
        var response = JsonUtility.FromJson<GetSceneResponseData>(json);
        if (response.statusCode == StatusCode.SUCCESS)
        {
            AssignValuesFromJsonToScene(response.data);
        }
        else
        {
            FeedbackLabelManager.Instance.ShowError(response.message);
            chunks.x = 1;
            chunks.y = 1;
        }
        InitializeBuilder();
    }
    
    private void AssignValuesFromJsonToScene(SceneData sceneData)
    {
        // var sceneData = JsonUtility.FromJson<SceneData>(data);
        chunks.x = sceneData.chunksX;
        chunks.y = sceneData.chunksY;
        // sceneData.

        if (chunks.x == 0) chunks.x = 1;
        if (chunks.y == 0) chunks.y = 1;
    }
    
    private void InitializeBuilder()
    {
        Debug.Log("Initialize Builder");
        CreateChunks();
        SetCameraPosition();
        CreateWalls();
        if(_sceneData != null) Load();
        BuilderUI.Instance.loadingSceneLabel.SetActive(false);
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
        if (_previewMode) return;
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            int layerMask = 1 << Layers.SELECTABLE;

            if (Physics.Raycast(ray, out hit, 1000, layerMask))
            {
                Debug.Log(hit.transform.gameObject.name);
                if (Navbar.Instance.isMouseInside) return;

                if (hit.transform.tag == "Selectable")
                {
                    if (TransformModal.instance != null && TransformModal.instance.isMouseInside) return;

                    SelectObject(hit.transform.gameObject);
                    //if (_selectedObjectOutline != null) _selectedObjectOutline.enabled = false;
                    //_selectedObjectOutline = hit.transform.GetComponent<Outline>();
                    //_selectedObjectOutline.enabled = true;
                    //transformModal.gameObject.SetActive(true);
                    //transformModal.SetTarget(_selectedObjectOutline.transform);

                    //pivot.gameObject.SetActive(true);
                    //pivot.transform.position = _selectedObjectOutline.transform.position;
                    //pivot.transformableObject = _selectedObjectOutline.gameObject;

                }
                else
                    UnselectObject();
            }
            else if (TransformModal.instance != null && TransformModal.instance.isMouseInside) return;
            else if (Navbar.Instance.isMouseInside) return;

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
            Debug.Log(_selectedObjectOutline);

            if (_selectedObjectOutline != null)
                _selectedObjectOutline.enabled = false;
            transformModal.gameObject.SetActive(false);
            _selectedObjectOutline = null;
        }
    }

    private void NulleateSelectedObject()
    {
        _selectedObjectOutline = null;
    }

    public void DeleteObject()
    {
        if (_selectedObjectOutline.gameObject == null) return;

        Destroy(_selectedObjectOutline.gameObject);
        SoundManager.Instance.PlaySound(SoundID.TRASH);
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
        SoundManager.Instance.PlaySound(SoundID.PAINT);
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
        Debug.Log($"{x}, {y}");
        return _chunksRenderers[y][x].transform;
    }


    public void Save()
    {
        if (!ValidateCounts())
        {
            FeedbackLabelManager.Instance.ShowError("There are too many elements in the scene.");
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

        var saveSceneData = new SaveSceneData();
        saveSceneData.title = data.name;
        saveSceneData.data = data;
        
        _builderAPI.SaveScene(SaveCallback, saveSceneData, _sceneId);
    }

    private void SaveCallback(string response)
    {
        FeedbackLabelManager.Instance.ShowSuccess("Scene saved.");
    }
    
    public void Save(APIConnection.ConnectionCallback callback, int? id = null)
    {
        if (id != null)
            _builderAPI.CreateScene(callback, null);
    }

    public void Load()
    {
        var sceneData = JsonUtility.FromJson<SceneData>(json.text);
        Debug.Log(sceneData);
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

    public void EnterPreviewMode()
    {
        Navbar.Instance.ResetPointerHandlers();
        UnselectObject();
        editorCamera.enabled = false;
        menuCamera.enabled = false;
        fpc.SetActive(true);
        menuCanvas.gameObject.SetActive(false);
        previewCanvas.gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        floorRuler.SetActive(true);
        _previewMode = true;
    }

    public void ExitPreviewMode()
    {
        editorCamera.enabled = true;
        menuCamera.enabled = true;
        fpc.SetActive(false);
        menuCanvas.gameObject.SetActive(true);
        previewCanvas.gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        floorRuler.SetActive(false);
        _previewMode = false;
    }

    public void DuplicateObject()
    {
        if (_selectedObjectOutline == null) return;
        var cloneParent = _selectedObjectOutline.gameObject;
        UnselectObject();
        var clone = Instantiate(cloneParent);
        clone.transform.position = cloneParent.transform.position;
        SoundManager.Instance.PlaySound(SoundID.PASTE);
        SelectObject(clone);        
    }


    private void SelectObject(GameObject selection)
    {
         UnselectObject();
        
        _selectedObjectOutline = selection.GetComponent<Outline>();
        _selectedObjectOutline.enabled = true;
        transformModal.SetTarget(selection.transform);
        transformModal.gameObject.SetActive(true);       
    }

    public void SpawnAsset(int selectedAssetPack, int sectionIndex, int assetIndex)
    {
        var asset = AssetPackManager.instance.SpawnAsset(selectedAssetPack, sectionIndex, assetIndex);
        SelectObject(asset);
    }
}
