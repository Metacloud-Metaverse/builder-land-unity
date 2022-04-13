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
    private List<SectionObject> _sectionObjects = new List<SectionObject>();
    public int selectedAssetPack;

    private void Start()
    {
        //CreateMenu(0);
        //_assetPackManager = AssetPackManager.instance;
    }


    public void CreateMenu(int assetPackIndex)
    {
        //_selectedAssetPack = assetPackIndex;

        title.text = AssetPackManager.instance.assetPacks[assetPackIndex].name;
        if (!AssetPackManager.instance.assetPacksDownloaded[assetPackIndex])
        {
            AssetPackManager.instance.DownloadAssetPack(assetPackIndex, CreateButtons);
        }
        else
        {
            CreateButtons();
        }
    }

    private void CreateButtons()
    {
        var sections = AssetPackManager.instance.assetPacks[selectedAssetPack].sections;
        for (int i = 0; i < sections.Count; i++)
        {
            var sectionObject = Instantiate(sectionPrefab);
            sectionObject.transform.SetParent(sectionsContainer);
            sectionObject.transform.localScale = Vector3.one;
            sectionObject.title.text = sections[i].name;

            var assets = sections[i].assets;

            for (int j = 0; j < assets.Count; j++)
            {
                var thumbnail = Instantiate(thumbnailPrefab);
                thumbnail.transform.SetParent(sectionObject.thumbnailsParent);
                thumbnail.transform.localScale = Vector3.one;
                var sectionIndex = i;
                var assetIndex = j;

                if (assets[j].GetType() == typeof(GameObject))
                {
                    var go = (GameObject)assets[j];
                    thumbnail.SetTexture(go.transform);
                    thumbnail.button.onClick.AddListener(
                        delegate { AssetPackManager.instance.SpawnAsset(selectedAssetPack, sectionIndex, assetIndex); });

                }
                else if (assets[j].GetType() == typeof(Texture2D))
                {
                    var texture = (Texture2D)assets[j];
                    thumbnail.SetTexture(texture);
                    print(SceneManagement.instance);
                    thumbnail.button.onClick.AddListener(
                        delegate { SceneManagement.instance.SetFloorTexture(texture); });

                }
                sectionObject.thumbnails.Add(thumbnail);

            }
            _sectionObjects.Add(sectionObject);
        }
    }

    private void DeleteSections()
    {
        print("delete");
        foreach (var section in _sectionObjects)
        {
            Destroy(section.gameObject);
        }
        _sectionObjects.Clear();
    }

    private void OnDisable()
    {
        DeleteSections();
    }

    private void OnEnable()
    {
        image.sprite = AssetPackManager.instance.assetPacks[selectedAssetPack].image;
        CreateMenu(selectedAssetPack);
    }
}
