using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public Texture2D resizeCursor;
    public Vector2 resizeCursorHotspot = new Vector2(10, 15f);
    public Texture2D grabCursor;
    public Vector2 grabCursorHotspot = new Vector2(0, 0);
    public Texture2D rotationCursor;
    public Vector2 rotationCursorHotspot = new Vector2(0, 0);
    private CursorMode cursorMode = CursorMode.Auto;
    private static CursorManager _instance;
    public static CursorManager instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else
            Debug.LogError("There are more than one Cursor Manager in scene");
    }

    public void SetResizeCursor()
    {
        Cursor.SetCursor(resizeCursor, resizeCursorHotspot, cursorMode);
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }

    public void SetGrabCursor()
    {
        Cursor.SetCursor(grabCursor, grabCursorHotspot, cursorMode);
    }

    public void SetRotationCursor()
    {
        Cursor.SetCursor(rotationCursor, rotationCursorHotspot, cursorMode);
    }

}
