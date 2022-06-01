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
        _chunksCountText.text = $"{SceneManagement.instance.chunks.x} x {SceneManagement.instance.chunks.y} LAND";
        _chunksSizeText.text = $"{SceneManagement.instance.chunks.x * SceneManagement.instance.chunkSize.x} x {SceneManagement.instance.chunks.y * SceneManagement.instance.chunkSize.z} m";
        _trianglesCountText.text = $"{SceneManagement.instance.GetTrianglesCount()}/{SceneManagement.instance.maxTriangles}";
        _meshesCountText.text = $"{SceneManagement.instance.GetMeshesCount()}/{SceneManagement.instance.maxMeshes}";
        _materialsCountText.text = $"{SceneManagement.instance.GetMaterialsCount()}/{SceneManagement.instance.maxMaterials}";
        _texturesCountText.text = $"{SceneManagement.instance.GetTexturesCount()}/{SceneManagement.instance.maxTextures}";
    }

    private void Update()
    {
        RefreshCounts();
    }
}
