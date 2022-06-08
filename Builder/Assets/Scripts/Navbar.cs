using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navbar : MonoBehaviour
{
    private TransformModalPointerHandler[] _pointerHandlers;
    public static Navbar Instance { get; private set; }
    public bool isMouseInside
    {
        get
        {
            foreach (var pointerHandler in _pointerHandlers)
            {
                if (pointerHandler.isMouseInside) return true;
            }
            return false;
        }
    }

    private void Awake()
    {
        _pointerHandlers = GetComponentsInChildren<TransformModalPointerHandler>();
        if (Instance == null) Instance = this;
        else
            throw new System.Exception("There are more than one navbars in scene");
    }

    public void ResetPointerHandlers()
    {
        foreach (var pointerHandler in _pointerHandlers)
        {
            pointerHandler.Reset();
        }
    }

}
