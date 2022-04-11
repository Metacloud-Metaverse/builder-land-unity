using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagement : MonoBehaviour
{
    private Outline _selectedObjectOutline;
    public PivotController pivot;
    public TransformModal transformModal;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "Selectable")
                {
                    if (TransformModal.instance != null && TransformModal.instance.isMouseInside) return;

                    if (_selectedObjectOutline != null) _selectedObjectOutline.enabled = false;
                    _selectedObjectOutline = hit.transform.GetComponent<Outline>();
                    _selectedObjectOutline.enabled = true;
                    //pivot.gameObject.SetActive(true);
                    //pivot.transform.position = _selectedObjectOutline.transform.position;
                    //pivot.transformableObject = _selectedObjectOutline.gameObject;
                    transformModal.gameObject.SetActive(true);
                    transformModal.SetTarget(_selectedObjectOutline.transform);
                    print(transformModal.target);
                }
                else
                    UnselectObject();
            }
            else
            {
                UnselectObject();
            }
            
        }
    }

    private void UnselectObject()
    {
        if (TransformModal.instance != null && !TransformModal.instance.isMouseInside)
        {
            if (_selectedObjectOutline != null)
                _selectedObjectOutline.enabled = false;
            //pivot.gameObject.SetActive(false);
            transformModal.gameObject.SetActive(false);
        }
    }
}
