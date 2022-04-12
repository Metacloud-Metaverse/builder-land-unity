using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pivot : MonoBehaviour
{
    [HideInInspector] public Axis xAxis;
    [HideInInspector] public Axis yAxis;
    [HideInInspector] public Axis zAxis;

    private void Awake()
    {
        var axises = GetComponentsInChildren<Axis>();

        foreach (var axis in axises)
        {
            switch(axis.type)
            {
                case Axis.Type.X:
                    xAxis = axis;
                    break;

                case Axis.Type.Y:
                    yAxis = axis;
                    break;

                case Axis.Type.Z:
                    zAxis = axis;
                    break;
            }
        }
    }
}
