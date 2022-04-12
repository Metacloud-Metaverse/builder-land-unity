using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollInput : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    private InputField _field;
    private TransformModal _transformModal;
    private bool _isClicking;
    public Axis.Type axis;
    public PivotController.Mode transformMode;

    void Awake()
    {
        _field = GetComponentInParent<InputField>();
        _transformModal = TransformModal.instance;
    }

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        _isClicking = true;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        CursorManager.instance.SetResizeCursor();
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        CursorManager.instance.SetDefaultCursor();
    }

    void Update()
    {
        if(_isClicking)
        {
            var mouseX = Input.GetAxis("Mouse X");
            Vector3 value = Vector3.zero;

            switch(axis)
            {
                case Axis.Type.X:
                    value = new Vector3(mouseX, 0, 0);
                    break;
                case Axis.Type.Y:
                    value = new Vector3(0, mouseX, 0);
                    break;
                case Axis.Type.Z:
                    value = new Vector3(0, 0, mouseX);
                    break;
            }

            switch(transformMode)
            {
                case PivotController.Mode.move:
                    _transformModal.target.position += value;
                    if(axis == Axis.Type.X)
                        _field.text = _transformModal.target.position.x.ToString();
                    else if(axis == Axis.Type.Y)
                        _field.text = _transformModal.target.position.y.ToString();
                    else
                        _field.text = _transformModal.target.position.z.ToString();
                    break;

                case PivotController.Mode.rotate:
                    _transformModal.target.eulerAngles += value * 10;
                    if (axis == Axis.Type.X)
                        _field.text = _transformModal.target.eulerAngles.x.ToString();
                    else if (axis == Axis.Type.Y)
                        _field.text = _transformModal.target.eulerAngles.y.ToString();
                    else
                        _field.text = _transformModal.target.eulerAngles.z.ToString();
                    break;

                case PivotController.Mode.scale:
                    _transformModal.target.localScale += value;
                    if (axis == Axis.Type.X)
                        _field.text = _transformModal.target.localScale.x.ToString();
                    else if (axis == Axis.Type.Y)
                        _field.text = _transformModal.target.localScale.y.ToString();
                    else
                        _field.text = _transformModal.target.localScale.z.ToString();
                    break;
            }
        }
        if(Input.GetMouseButtonUp(0))
        {
            _isClicking = false;
        }
    }
}
