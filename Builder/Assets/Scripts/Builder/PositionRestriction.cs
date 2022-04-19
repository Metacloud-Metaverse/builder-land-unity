using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionRestriction : MonoBehaviour
{
    public bool isProhibited { get; private set; }
    private Renderer[] _renderers;
    private List<Wall> _wallsCollisioned = new List<Wall>();

    private void Awake()
    {
        _renderers = GetComponentsInChildren<Renderer>();
    }
    

    public void SetErrorMaterial()
    {
        foreach (var renderer in _renderers)
        {
            renderer.material.SetColor("_BaseColor", Color.red);
        }
    }

    public void SetMainMaterial()
    {
        foreach (var renderer in _renderers)
        {
            renderer.material.SetColor("_BaseColor", Color.white);
        }

    }

    public void AddCollisionedWall(Wall wall)
    {
        _wallsCollisioned.Add(wall);
        print("count " + _wallsCollisioned.Count);
        isProhibited = true;
        SetErrorMaterial();
    }

    public void RemoveCollisionedWall(Wall wall)
    {
        _wallsCollisioned.Remove(wall);
        print("count " + _wallsCollisioned.Count);
        if (_wallsCollisioned.Count == 0)
        {
            isProhibited = false;
            SetMainMaterial();
        }
    }
    
}
