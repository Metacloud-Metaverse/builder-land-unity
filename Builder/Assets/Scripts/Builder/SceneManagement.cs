using UnityEngine;
using AssetPacks;
using System.Collections.Generic;

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
    private Texture2D _currentGroundTexture;
    public float maxHeightCoef = 20;
    public float maxHeight { get; private set; }
    public int maxTriangles = 10000;
    public int maxMeshes = 300;
    private int _maxMaterials;
    public int maxMaterials
    {
        get { return _maxMaterials; }
        set { _maxMaterials = (int)(Mathf.Log(chunks.x * chunks.y, 2) * value); }
    }
    public int maxMaterialsCoef = 20;
    private int _maxTextures;
    public int maxTextures
    {
        get { return _maxTextures; }
        set { _maxTextures = (int)(Mathf.Log(chunks.x * chunks.y, 2) * value); }

    }
    public int maxTexturesCoef = 10;

    private int _trianglesCount;
    private int _meshesCount;
    private int _materialsCount;
    private int _texturesCount;

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

    private void SetCameraPosition()
    {
        var pos = editorCamera.transform.position;
        var multiplier = chunks.x - 1;
        var coef = new Vector3(1.46f, 0.71f, 0.12f) * multiplier;
        editorCamera.transform.position = new Vector3(
            pos.x + pos.x * coef.x,
            pos.y + pos.y * coef.y,
            pos.z + pos.z * coef.z
        );
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
        _currentGroundTexture = texture;
    }

    public Transform GetChunk(int x, int y)
    {
        print($"{x}, {y}");
        return _chunksRenderers[y][x].transform;
    }

    public void Save()
    {
        if(!ValidateCounts())
        {
            FeedbackLabel.instance.ShowError("There are too many elements in the scene.");
        }
        var gameObjects = GameObject.FindGameObjectsWithTag("Selectable");
        var correctGameObjects = GetCorrectGameObjects(gameObjects);
        var data = new SceneData();
        data.name = sceneName;
        data.ground = AssetPackManager.instance.GetUrl(_currentGroundTexture);
        data.gameObjects = new SceneObjectData[correctGameObjects.Count];

        for (int i = 0; i < correctGameObjects.Count; i++)
        {
            var go = correctGameObjects[i];
            var objectData = new SceneObjectData();
            var dataComponent = go.GetComponent<Data>();

            dataComponent.SetChunkParent();

            objectData.positionX = go.transform.localPosition.x;
            objectData.positionY = go.transform.localPosition.y;
            objectData.positionZ = go.transform.localPosition.z;

            objectData.rotationX = go.transform.localRotation.x;
            objectData.rotationY = go.transform.localRotation.y;
            objectData.rotationZ = go.transform.localRotation.z;

            objectData.scaleX = go.transform.localScale.x;
            objectData.scaleY = go.transform.localScale.y;
            objectData.scaleZ = go.transform.localScale.z;

            objectData.chunkX = dataComponent.chunkX;
            objectData.chunkY = dataComponent.chunkY;

            objectData.url = dataComponent.url;

            data.gameObjects[i] = objectData;
        }

        var json = JsonUtility.ToJson(data);
        print(json);
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
            new Vector3(chunkSize.x / 2 * (chunks.x - 1), 0, chunkSize.z / 2 * (chunks.y - 1)),
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
