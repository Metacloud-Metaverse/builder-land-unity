using UnityEngine;

public class Data : MonoBehaviour
{
    public int meshId;
    public string url;
    public int chunkX { get { return (int)((transform.position.x + _chunkSize.x / 2) / _chunkSize.x); } } // El "_chunkSize.x / 2 es porque el pivot de la parcela está en el centro. Si estuviera en la esquina superior izquierda, se sacaría esa parte."
    public int chunkY { get { return (int)((transform.position.z + _chunkSize.z / 2) / _chunkSize.z); } }
    private Vector3 _chunkSize;

    private void Start()
    {
        _chunkSize = SceneManagement.Instance.chunkSize;
    }


    public void SetChunkParent()
    {
        var parent = SceneManagement.Instance.GetChunk(chunkX, chunkY);
        transform.SetParent(parent);
    }
}
