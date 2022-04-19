using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionRestrictionData : MonoBehaviour
{
    private static PositionRestrictionData _instance;
    public static PositionRestrictionData instance { get { return _instance; } }
    public Material prohibitedMaterial;

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else
            Debug.LogError("There are more than one Position restriction data instances");
    }


}
