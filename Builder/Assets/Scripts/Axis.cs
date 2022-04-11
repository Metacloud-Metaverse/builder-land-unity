using UnityEngine;

public class Axis : MonoBehaviour
{
    private MeshRenderer _renderer;
    private PivotController _pivotController;

    public enum Type { X, Y, Z }
    public Type type;

    void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
        _pivotController = PivotController.instance;
    }

    public void SetSelected(bool selected)
    {
        if(selected)
        {
            print("selected");
            _renderer.material = _pivotController.selectedMaterial;
        }
        else
        {
            switch(type)
            {
                case Type.X:
                    _renderer.material = _pivotController.xAxisMaterial;
                    break;
                case Type.Y:
                    _renderer.material = _pivotController.yAxisMaterial;
                    break;
                case Type.Z:
                    _renderer.material = _pivotController.zAxisMaterial;
                    break;

            }
        }
    }

}
