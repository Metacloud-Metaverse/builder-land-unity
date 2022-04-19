using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoundBox : MonoBehaviour
{
    public void Initialize(Renderer[] renderers)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            var collider = gameObject.AddComponent<BoxCollider>();
            collider.center = renderers[i].bounds.center;
            collider.size = renderers[i].bounds.size;
        }
        gameObject.tag = "Bound box";
    }
}
