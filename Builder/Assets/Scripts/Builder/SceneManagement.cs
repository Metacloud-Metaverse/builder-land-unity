using UnityEngine;
using AssetPacks;

public class SceneManagement : MonoBehaviour
{
    private Outline _selectedObjectOutline;
    public PivotController pivot;
    public TransformModal transformModal;
    private static SceneManagement _instance;
    public static SceneManagement instance { get { return _instance; } }
    private AssetPackManager _apm;

    public MeshRenderer[] chunksRenderers;

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else
            Debug.LogError("There are more than one scene managemente in the scene");
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
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
                    print(transformModal.target);
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
        foreach (var renderer in chunksRenderers)
        {
            renderer.material.mainTexture = texture;
            renderer.material.color = Color.white;
        }
    }
}
