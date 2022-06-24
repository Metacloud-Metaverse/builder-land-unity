using UnityEngine;

public class EditorCamera : MonoBehaviour
{
    private float _horizontal;
    private float _vertical;
    private float _scroll;
    private bool _isGrabbing;

    public float translationCoef = 5;
    public float zoomCoef = 10;
    public float rotationCoef = 200;

    void Update()
    {
        if (!CursorManager.instance.IsInEditorScreen()) return;
        if (!SceneManagement.Instance.activeEditor) return;
        
        _horizontal = Input.GetAxis("Mouse X");
        _vertical = Input.GetAxis("Mouse Y");
        _scroll = Input.GetAxis("Mouse ScrollWheel");

        Translate();
        Zoom();
        Rotate();
    }

    private void Translate()
    {
        if (Input.GetMouseButton(2) || Input.GetKey(KeyCode.LeftAlt) &&
            Input.GetKey(KeyCode.LeftCommand) && Input.GetMouseButton(0))
        {
            CursorManager.instance.SetGrabCursor();
            transform.Translate(new Vector3(-_horizontal, -_vertical,0) * translationCoef * Time.deltaTime);
            _isGrabbing = true;
        }

        if (_isGrabbing && (Input.GetMouseButtonUp(2) || Input.GetKeyUp(KeyCode.LeftAlt) ||
            Input.GetKeyUp(KeyCode.LeftCommand) || Input.GetMouseButtonUp(0)))
        {
            CursorManager.instance.SetDefaultCursor();
            _isGrabbing = false;
        }
    }

    private void Zoom()
    {
        transform.Translate(0, 0, _scroll * zoomCoef * Time.deltaTime);
    }

    private void Rotate()
    {
        if(Input.GetMouseButton(1))
        {
            CursorManager.instance.SetRotationCursor();
            Vector3 axis = Vector3.zero;
            float coef = 0;
            if (Mathf.Abs(_horizontal) > Mathf.Abs(_vertical))
            {
                axis = Vector3.up;
                coef = _horizontal;
            }
            if (Mathf.Abs(_vertical) > Mathf.Abs(_horizontal))
            {
                axis = transform.right;
                coef = _vertical;
            }

            if(TransformModal.instance != null && TransformModal.instance.isActiveAndEnabled)
            {
               transform.RotateAround(TransformModal.instance.target.transform.position,
               axis, rotationCoef * coef * Time.deltaTime);
               print(Vector3.Angle(transform.position, TransformModal.instance.target.transform.position));
            }
            else
            {
                transform.Rotate(axis, rotationCoef * coef * Time.deltaTime);
                var euler = transform.eulerAngles;
                transform.eulerAngles = new Vector3(euler.x, euler.y, 0);
            }
        }
        if (Input.GetMouseButtonUp(1))
            CursorManager.instance.SetDefaultCursor();
    }
}
