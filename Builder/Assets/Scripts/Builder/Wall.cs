using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Bound box")
        {
            var restriction = other.GetComponentInParent<PositionRestriction>();
            restriction.AddCollisionedWall(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Bound box")
        {
            var restriction = other.GetComponentInParent<PositionRestriction>();
            restriction.RemoveCollisionedWall(this);
        }
    }

}
