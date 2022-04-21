using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneCounts : MonoBehaviour
{
    public Text trianglesCountText;
    public Text meshesCountText;
    public Text materialsCountText;
    public Text texturesCountText;

    private SceneManagement _sceneManagement;

    public void RefreshCounts()
    {
        trianglesCountText.text = $"Triangles: {SceneManagement.instance.GetTrianglesCount()}/{SceneManagement.instance.maxTriangles}";
        meshesCountText.text = $"Meshes: {SceneManagement.instance.GetMeshesCount()}/{SceneManagement.instance.maxMeshes}";
        materialsCountText.text = $"Materials: {SceneManagement.instance.GetMaterialsCount()}/{SceneManagement.instance.maxMaterials}";
        texturesCountText.text = $"Textures: {SceneManagement.instance.GetTexturesCount()}/{SceneManagement.instance.maxTextures}";
    }

    private void Update()
    {
        RefreshCounts();
    }
}
