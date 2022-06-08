using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public float rotationCoef = 30;
    private float _mouseHorizontal;
    private float _mouseVertical;
    private float _horizontal;
    private float _vertical;
    public float speed = 5;
    private Camera _cam;
    private float _xRot;
    private const float _RECT_ANGLE = 90;
    public float timeCursorShown = 5;

    private void Awake()
    {
        _cam = GetComponentInChildren<Camera>();
    }

    void Update()
    {
        GetAxis();
        Move();
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManagement.instance.ExitPreviewMode();
        }
    }


    private void GetAxis()
    {
        _mouseHorizontal = Input.GetAxis("Mouse X");
        _mouseVertical = Input.GetAxis("Mouse Y");

        _horizontal = Input.GetAxis("Horizontal");
        _vertical = Input.GetAxis("Vertical");
    }

    private void LateUpdate()
    {
        Rotate();
    }

    private void Move()
    {
        transform.position += transform.forward * _vertical * speed * Time.deltaTime;
        transform.position += transform.right * _horizontal * speed * Time.deltaTime;

    }

    private void Rotate()
    {
        transform.Rotate(Vector3.up, _mouseHorizontal * rotationCoef * Time.deltaTime);
        _xRot -= _mouseVertical;
        _xRot = Mathf.Clamp(_xRot, -_RECT_ANGLE, _RECT_ANGLE);
        _cam.transform.localRotation = Quaternion.Euler(_xRot, 0, 0);
        
    }
}
