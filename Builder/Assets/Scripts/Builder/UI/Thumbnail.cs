using UnityEngine;
using UnityEngine.UI;

public class Thumbnail : MonoBehaviour
{
    public Image thumbnail;
    public Button button;

    public void SetTexture(Texture2D texture)
    {
        var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
        thumbnail.sprite = sprite;
    }

    public void SetTexture(Transform transform)
    {
        var texture = RuntimePreviewGenerator.GenerateModelPreview(transform);
        SetTexture(texture);
    }
}
