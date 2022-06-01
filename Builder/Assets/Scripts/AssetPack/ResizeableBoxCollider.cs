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
            var bounds = renderers[i].bounds;

            // In world-space!
            var size = bounds.size;
            var center = bounds.center;

            // converted to local space of the collider
            size = colliders[i].transform.InverseTransformVector(size);
            center = colliders[i].transform.InverseTransformPoint(center);

            colliders[i].size = size;
            colliders[i].center = center;

        }
    }

}
