using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class TransformModal : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Transform _target;
    public Transform target { get { return _target; } }

    public InputField posX;
    public InputField posY;
    public InputField posZ;
    public InputField rotX;
    public InputField rotY;
    public InputField rotZ;
    public InputField sclX;
    public InputField sclY;
    public InputField sclZ;

    private bool _isMouseInside;
    public bool isMouseInside { get { return _isMouseInside; } }

    private static TransformModal _instance;
    public static TransformModal instance { get { return _instance; } }

    void Awake()
    {
        if (_instance == null) _instance = this;
        else
            Debug.LogError("There are two Transform modals in scene");
    }

    void Start()
    {
        posX.onValueChanged.AddListener(delegate {
            SetVector(posX, Axis.Type.X, PivotController.Mode.move);
        });
        posY.onValueChanged.AddListener(delegate {
            SetVector(posY, Axis.Type.Y, PivotController.Mode.move);
        });
        posZ.onValueChanged.AddListener(delegate {
            SetVector(posZ, Axis.Type.Z, PivotController.Mode.move);
        });

        rotX.onValueChanged.AddListener(delegate {
            SetVector(rotX, Axis.Type.X, PivotController.Mode.rotate);
        });
        rotY.onValueChanged.AddListener(delegate {
            SetVector(rotY, Axis.Type.Y, PivotController.Mode.rotate);
        });
        rotZ.onValueChanged.AddListener(delegate {
            SetVector(rotZ, Axis.Type.Z, PivotController.Mode.rotate);
        });

        sclX.onValueChanged.AddListener(delegate {
            SetVector(sclX, Axis.Type.X, PivotController.Mode.scale);
        });
        sclY.onValueChanged.AddListener(delegate {
            SetVector(sclY, Axis.Type.Y, PivotController.Mode.scale);
        });
        sclZ.onValueChanged.AddListener(delegate {
            SetVector(sclZ, Axis.Type.Z, PivotController.Mode.scale);
        });
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        _isMouseInside = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        _isMouseInside = false;
    }
    public void SetPositionX()
    {
        SetVector(posX, Axis.Type.X, PivotController.Mode.move);
    }

    public void SetVector(InputField input, Axis.Type axis, PivotController.Mode mode)
    {
        var text = input.text;
        if (string.IsNullOrEmpty(text)) return;

        var value = float.Parse(text);

        switch(axis)
        {
            case Axis.Type.X:
                if (mode == PivotController.Mode.move)
                    _target.position = new Vector3(value, _target.position.y, _target.position.z);
                else if (mode == PivotController.Mode.rotate)
                    _target.eulerAngles = new Vector3(value, _target.eulerAngles.y, _target.eulerAngles.z);
                else
                    _target.localScale = new Vector3(value, _target.localScale.y, _target.localScale.z);
                break;

            case Axis.Type.Y:
                if (mode == PivotController.Mode.move)
                    _target.position = new Vector3(_target.position.x, value, _target.position.z);
                else if (mode == PivotController.Mode.rotate)
                    _target.eulerAngles = new Vector3(_target.eulerAngles.x, value, _target.eulerAngles.z);
                else
                    _target.localScale = new Vector3(_target.localScale.x, value, _target.localScale.z);
                break;

            case Axis.Type.Z:
                if (mode == PivotController.Mode.move)
                    _target.position = new Vector3(_target.position.x, _target.position.y, value);
                else if (mode == PivotController.Mode.rotate)
                    _target.eulerAngles = new Vector3(_target.eulerAngles.x, _target.eulerAngles.y, value);
                else
                    _target.localScale = new Vector3(_target.localScale.x, _target.localScale.y, value);
                break;
        }
        
    }

    public void SetTarget(Transform target)
    {
        _target = target;
        posX.text = target.position.x.ToString();
        posY.text = target.position.y.ToString();
        posZ.text = target.position.z.ToString();

        rotX.text = target.eulerAngles.x.ToString();
        rotY.text = target.eulerAngles.y.ToString();
        rotZ.text = target.eulerAngles.z.ToString();

        sclX.text = target.localScale.x.ToString();
        sclY.text = target.localScale.y.ToString();
        sclZ.text = target.localScale.z.ToString();



    }
}
