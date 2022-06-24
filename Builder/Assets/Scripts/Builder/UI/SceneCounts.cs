using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneCounts : MonoBehaviour
{
    [SerializeField] private Text _chunksCountText;
    [SerializeField] private Text _chunksSizeText;
    [SerializeField] private Text _trianglesCountText;
    [SerializeField] private Text _meshesCountText;
    [SerializeField] private Text _materialsCountText;
    [SerializeField] private Text _texturesCountText;


    public void RefreshCounts()
    {
        _chunksCountText.text = $"{SceneManagement.Instance.chunks.x} x {SceneManagement.Instance.chunks.y} LAND";
        _chunksSizeText.text = $"{SceneManagement.Instance.chunks.x * SceneManagement.Instance.chunkSize.x} x {SceneManagement.Instance.chunks.y * SceneManagement.Instance.chunkSize.z} m";
        _trianglesCountText.text = $"{SceneManagement.Instance.GetTrianglesCount()}/{SceneManagement.Instance.maxTriangles}";
        _meshesCountText.text = $"{SceneManagement.Instance.GetMeshesCount()}/{SceneManagement.Instance.maxMeshes}";
        _materialsCountText.text = $"{SceneManagement.Instance.GetMaterialsCount()}/{SceneManagement.Instance.maxMaterials}";
        _texturesCountText.text = $"{SceneManagement.Instance.GetTexturesCount()}/{SceneManagement.Instance.maxTextures}";
    }

    private void Update()
    {
        RefreshCounts();
    }
}
