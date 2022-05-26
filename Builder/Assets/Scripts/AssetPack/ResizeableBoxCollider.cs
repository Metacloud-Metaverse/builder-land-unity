using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResizeableBoxCollider: MonoBehaviour
{
    public BoxCollider[] colliders;
    public SkinnedMeshRenderer[] renderers;
    public float boundsCoef = 1;

    private void Start()
    {
        //for (var i = 0; i < colliders.Length; i++)
        //{
        //    colliders[i].center = renderers[i].bounds.center;
        //}
        //Invoke("Resize", 3f);
    }

    private void Resize()
    {
        
    }

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
