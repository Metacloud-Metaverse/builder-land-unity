using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundImageOffset : MonoBehaviour
{
    [SerializeField] private Camera _editorCamera;
    [SerializeField] private RawImage _image;

    void Start()
    {
        _image.uvRect = _editorCamera.rect;
    }

}
