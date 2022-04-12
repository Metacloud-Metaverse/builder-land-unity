using UnityEngine;
using UnityEngine.UI;
using AssetPacks;
using System.Collections.Generic;

public class SectionMenu : MonoBehaviour
{
    public Image image;
    public Text title;
    public SectionObject sectionPrefab;
    public Thumbnail thumbnailPrefab;
    public RectTransform sectionsContainer;
    private List<SectionObject> sectionObjects = new List<SectionObject>();
    private int _selectedAssetPack;
    private void Start()
    {
        CreateMenu(0);
    }


    public void CreateMenu(int assetPackIndex)
    {
        _selectedAssetPack = assetPackIndex;
 
        title.text = AssetPackManager.instance.assetPacks[assetPackIndex].name;

        AssetPackManager.instance.DownloadAssetPack(assetPackIndex, CreateButtons);
    }

    private void CreateButtons()
    {
        foreach (var section in AssetPackManager.instance.assetPacks[_selectedAssetPack].sections)
        {
            var sectionObject = Instantiate(sectionPrefab);
            sectionObject.transform.SetParent(sectionsContainer);
            sectionObject.transform.localScale = Vector3.one;
            sectionObject.title.text = section.name;

            foreach (var asset in section.assets)
            {
                var thumbnail = Instantiate(thumbnailPrefab);
                thumbnail.transform.SetParent(sectionObject.thumbnailsParent);
                thumbnail.transform.localScale = Vector3.one;
                print($"{asset.GetType()} {typeof(GameObject)} {typeof(Texture2D)}");
                if(asset.GetType() == typeof(GameObject))
                {
                    print("gameobject");
                    var go = (GameObject)asset;
                    thumbnail.SetTexture(go.transform);
                }
                else if(asset.GetType() == typeof(Texture2D))
                {
                    print("texture");
                    var texture = (Texture2D)asset;
                    thumbnail.SetTexture(texture);
                }
                sectionObject.thumbnails.Add(thumbnail);
            }
        }
    }
}
