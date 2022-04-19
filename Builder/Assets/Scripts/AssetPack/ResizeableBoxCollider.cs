using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeableBoxCollider
{
    public BoxCollider collider;
    public SkinnedMeshRenderer renderer;

    public ResizeableBoxCollider(BoxCollider collider, SkinnedMeshRenderer renderer)
    {
        this.collider = collider;
        this.renderer = renderer;
    }

    public void Refresh()
    {
        collider.center = renderer.bounds.center;
        collider.size = renderer.bounds.size;
    }
}
