using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeableBoxCollider: MonoBehaviour
{
    public BoxCollider[] colliders;
    public SkinnedMeshRenderer[] renderers;

    public void Update()
    {
        if (colliders == null || renderers == null) return;

        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].center = renderers[i].bounds.center;
            colliders[i].size = renderers[i].bounds.size;
        }
    }
}
