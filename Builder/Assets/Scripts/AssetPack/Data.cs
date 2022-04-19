using UnityEngine;

public class Data : MonoBehaviour
{
    public string url;
    public int chunkX;
    public int chunkY;
    private Vector3 _chunkSize;

    private void Start()
    {
        _chunkSize = SceneManagement.instance.chunkSize;
    }

    private void Update()
    {
        chunkX = (int)((transform.position.x + _chunkSize.x / 2) / _chunkSize.x); // El "_chunkSize.x / 2 es porque el pivot de la parcela está en el centro. Si estuviera en la esquina superior izquierda, se sacaría esa parte."
        chunkY = (int)((transform.position.z + _chunkSize.z / 2) / _chunkSize.z);
    }

    public void SetChunkParent()
    {
        var parent = SceneManagement.instance.GetChunk(chunkX, chunkY);
        transform.SetParent(parent);
    }
}
