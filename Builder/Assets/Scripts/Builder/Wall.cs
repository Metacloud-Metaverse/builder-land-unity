using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Selectable")
        {
            var restriction = other.GetComponent<PositionRestriction>();
            restriction.AddCollisionedWall(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Selectable")
        {
            var restriction = other.GetComponent<PositionRestriction>();
            restriction.RemoveCollisionedWall(this);
        }
    }

}
