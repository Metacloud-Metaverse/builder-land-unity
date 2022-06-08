using UnityEngine;
using UnityEngine.UI;
using AssetPacks;
using System.Collections.Generic;

public class SectionMenu : Menu
{
    public Image image;
    public Text title;
    public SectionObject sectionPrefab;
    public Thumbnail thumbnailPrefab;
    public RectTransform sectionsContainer;
    private List<SectionObject> _sectionObjects = new List<SectionObject>();
    public int selectedAssetPack;
    public ContentSizeFitter contentSizeFitter;
    public GameObject loading;


    public void CreateMenu(int assetPackIndex)
    {
        title.text = AssetPackManager.instance.assetPacks[assetPackIndex].name;
        if (!AssetPackManager.instance.assetPacksDownloaded[assetPackIndex])
        {
            ShowLoading(true);
            AssetPackManager.instance.DownloadAssetPack(assetPackIndex, DownloadCallback);
        }
        else
        {
            CreateButtons();
        }
    }


    private void ShowLoading(bool show)
    {
        loading.SetActive(show);
    }


    private void DownloadCallback()
    {
        ShowLoading(false);
        CreateButtons();
    }


    protected override void CreateButtons()
    {
        var sections = AssetPackManager.instance.assetPacks[selectedAssetPack].sections;
        for (int i = 0; i < sections.Count; i++)
        {
            var sectionObject = Instantiate(sectionPrefab);
            sectionObject.transform.SetParent(sectionsContainer);
            sectionObject.transform.localScale = Vector3.one;
            sectionObject.title.text = sections[i].name.ToUpper();

            var assets = sections[i].assets;

            for (int j = 0; j < assets.Count; j++)
            {
                var thumbnail = Instantiate(thumbnailPrefab);
                thumbnail.transform.SetParent(sectionObject.thumbnailsParent);
                thumbnail.transform.localScale = Vector3.one;
                var sectionIndex = i;
                var assetIndex = j;

                if (assets[j].asset.GetType() == typeof(GameObject))
                {
                    var go = (GameObject)assets[j].asset;
                    thumbnail.SetTexture(go.transform);
                    thumbnail.button.onClick.AddListener(
                        delegate { SceneManagement.instance.SpawnAsset(selectedAssetPack, sectionIndex, assetIndex); });

                }
                else if (assets[j].asset.GetType() == typeof(Texture2D))
                {
                    var texture = (Texture2D)assets[j].asset;
                    thumbnail.SetTexture(texture);
                    int id = assets[j].id;

                    thumbnail.button.onClick.AddListener(
                        delegate { SceneManagement.instance.SetFloorTexture(id); });

                }
                sectionObject.thumbnails.Add(thumbnail);

            }
            var transforms = sectionObject.thumbnailGrid.GetComponentsInChildren<RectTransform>();
            int rowCount = 0;
            if (transforms.Length > 1)
            {
                var firstPosition = transforms[1].anchoredPosition;
                for (int j = 1; j < transforms.Length; j++)
                {
                    if (transforms[j].anchoredPosition.x == firstPosition.x)
                        rowCount++;
                }
            }
            var rt = sectionObject.GetComponent<RectTransform>();
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, sectionObject.thumbnailGrid.cellSize.y * rowCount + sectionObject.titleTransform.sizeDelta.y);
            _sectionObjects.Add(sectionObject);
        }

        //workaround. El componente content size fitter hace que se solapen los
        //dos ultimos elementos del vertical layout group

        var fixSection = Instantiate(sectionPrefab);
        fixSection.transform.SetParent(sectionsContainer);
        fixSection.transform.localScale = Vector3.one;
        fixSection.gameObject.SetActive(false);
        _sectionObjects.Add(fixSection);
    }


    private void DeleteSections()
    {
        foreach (var section in _sectionObjects)
        {
            Destroy(section.gameObject);
        }
        _sectionObjects.Clear();
    }


    private void OnDisable()
    {
        DeleteSections();
        contentSizeFitter.enabled = false;
    }


    private void OnEnable()
    {
        image.sprite = AssetPackManager.instance.assetPacks[selectedAssetPack].image;
        CreateMenu(selectedAssetPack);
        contentSizeFitter.enabled = true;
    }
}
