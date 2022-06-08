using UnityEngine;
using UnityEngine.EventSystems;

public class TransformModalPointerHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool isMouseInside { get; private set; }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        isMouseInside = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        isMouseInside = false;
    }

    public void Reset()
    {
        isMouseInside = false;
    }
}
