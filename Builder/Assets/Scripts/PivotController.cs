using UnityEngine;

public class PivotController : MonoBehaviour
{
    public Renderer xAxis;
    public Renderer yAxis;
    public Renderer zAxis;
    public Renderer center;

    public Material xAxisMaterial;
    public Material yAxisMaterial;
    public Material zAxisMaterial;
    public Material centerMaterial;
    public Material selectedMaterial;
    public Pivot movementPivot;
    public Pivot rotationPivot;
    public Pivot scalePivot;

    private bool _zClicked;
    private bool _yClicked;
    private bool _xClicked;
    private bool _centerClicked;
    private Pivot _currentPivot;
    private Camera _camera;
    public GameObject transformableObject;
    public Vector3 initialScale = new Vector3(0.5f, 0.5f, 0.5f);
    public enum Mode { move, scale, rotate };
    public static Mode mode;
    public delegate void TransformFunction(float mouseX, float mouseY);
    private static PivotController _instance;
    public static PivotController instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else
            Debug.LogError("There are more than one PivotController in the scene");
    }

    private void Start()
    {
        _camera = Camera.main;
        SetMode(Mode.move);
    }

    void Update()
    {
        CheckPivotCollision();
        UpdatePivot();
        ResetPivot();
        RescalePivot();
    }

    private void CheckPivotCollision()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var hits = Physics.RaycastAll(ray, 1000);
            foreach (var hit in hits)
            {
                if (hit.transform.gameObject == _currentPivot.zAxis.gameObject)
                {
                    _zClicked = true;
                    _currentPivot.zAxis.SetSelected(true);
                }
                if (hit.transform.gameObject == _currentPivot.xAxis.gameObject)
                {
                    _xClicked = true;
                    _currentPivot.xAxis.SetSelected(true);
                }
                if (hit.transform.gameObject == _currentPivot.yAxis.gameObject)
                {
                    _yClicked = true;
                    _currentPivot.yAxis.SetSelected(true);
                }
            }
        }
    }


    private void UpdatePivot()
    {
        var mouseX = Input.GetAxis("Mouse X");
        var mouseY = Input.GetAxis("Mouse Y");

        if (_zClicked)
        {
            if (mouseX != 0)
            {
                TransformSelectable(MoveZ, ScaleZ, RotateZ, mouseX, mouseY);
            }
        }
        else if (_xClicked)
        {
            if (mouseX != 0)
            {
                TransformSelectable(MoveX, ScaleX, RotateX, mouseX, mouseY);
            }
        }
        else if (_yClicked)
        {
            if (mouseY != 0)
            {
                TransformSelectable(MoveY, ScaleY, RotateY, mouseX, mouseY);
            }
        }
    }

    private void UpdateRotationPivot(float mouseX, float mouseY)
    {

    }

    private void UpdateNotRotationPivot(float mouseX, float mouseY)
    {

    }

    private void TransformSelectable(TransformFunction moveFunction, TransformFunction scaleFunction, TransformFunction rotateFunction, float mouseX, float mouseY)
    {
        switch(mode)
        {
            case Mode.move:
                moveFunction(mouseX, mouseY);
                break;
            case Mode.scale:
                scaleFunction(mouseX, mouseY);
                break;
            case Mode.rotate:
                rotateFunction(mouseX, mouseY);
                break;
        }
    }

    private void MoveX(float mouseX, float mouseY)
    {
        gameObject.transform.position -= new Vector3(mouseX + mouseY, 0, 0);
        transformableObject.transform.position -= new Vector3(mouseX + mouseY, 0, 0);
    }

    private void MoveY(float mouseX, float mouseY)
    {
        gameObject.transform.position += new Vector3(0, mouseY, 0);
        transformableObject.transform.position += new Vector3(0, mouseY, 0);

    }

    private void MoveZ(float mouseX, float mouseY)
    {
        gameObject.transform.position += new Vector3(0, 0, mouseX + mouseY);
        transformableObject.transform.position += new Vector3(0, 0, mouseX + mouseY);
    }

    private void ScaleX(float mouseX, float mouseY)
    {
        var currentScale = transformableObject.transform.localScale;
        transformableObject.transform.localScale = new Vector3(currentScale.x - mouseX - mouseY, currentScale.y, currentScale.z);
    }

    private void ScaleY(float mouseX, float mouseY)
    {
        var currentScale = transformableObject.transform.localScale;
        transformableObject.transform.localScale = new Vector3(currentScale.x, currentScale.y + mouseX + mouseY, currentScale.z);
    }

    private void ScaleZ(float mouseX, float mouseY)
    {
        var currentScale = transformableObject.transform.localScale;
        transformableObject.transform.localScale = new Vector3(currentScale.x, currentScale.y, currentScale.z + mouseX + mouseY);
    }

    private void RotateX(float mouseX, float mouseY)
    {

    }

    private void RotateY(float mouseX, float mouseY)
    {

    }

    private void RotateZ(float mouseX, float mouseY)
    {

    }

    private void ResetPivot()
    {
        if (Input.GetMouseButtonUp(0))
        {
            _zClicked = false;
            _xClicked = false;
            _yClicked = false;
            _currentPivot.xAxis.SetSelected(false);
            _currentPivot.yAxis.SetSelected(false);
            _currentPivot.zAxis.SetSelected(false);
        }
    }

    private void RescalePivot()
    {
        var dst = Vector3.Distance(gameObject.transform.position, _camera.transform.position);
        gameObject.transform.localScale = initialScale;
        gameObject.transform.localScale *= Mathf.Sqrt(dst * 0.1f);
    }

    public void SetMovePivot()
    {
        center.gameObject.SetActive(false);
    }
    public void SetRotatePivot()
    {
        center.gameObject.SetActive(false);
    }

    public void SetScalePivot()
    {
        center.gameObject.SetActive(true);
    }

    public void SetMode(int pMode)
    {
        SetMode((Mode)pMode);
    }

    public void SetMode(Mode pMode)
    {
        mode = pMode;
        HidePivots();

        switch (mode)
        {
            case Mode.move:
                ShowAhdSetPivot(movementPivot);
                break;
            case Mode.scale:
                ShowAhdSetPivot(scalePivot);
                break;
            case Mode.rotate:
                ShowAhdSetPivot(rotationPivot);
                break;
        }
    }

    private void HidePivots()
    {
        movementPivot.gameObject.SetActive(false);
        rotationPivot.gameObject.SetActive(false);
        scalePivot.gameObject.SetActive(false);
    }

    private void ShowAhdSetPivot(Pivot pivot)
    {
        pivot.gameObject.SetActive(true);
        _currentPivot = pivot;
    }
}
