using UnityEngine;
using UnityEngine.UI;

public class Thumbnail : MonoBehaviour
{
    public Image thumbnail;
    public GameObject model;

    private void Start()
    {
        var texture = RuntimePreviewGenerator.GenerateModelPreview(model.transform);
        SetTexture(texture);
    }

    public void SetTexture(Texture2D texture)
    {
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        thumbnail.sprite = sprite;
    }
}
