using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetPacks;
using UnityEngine.UI;

public class SearchAssetMenu : Menu
{
    public Thumbnail thumbnailPrefab;
    public RectTransform buttonsParent;
    public InputField searchInput;
    public GameObject searchingLabel;
    public Text searchLabel;
    public Text notFoundLabel;
    private List<Thumbnail> _thumbnails = new();

    public void Search(string parameter)
    {
        searchInput.interactable = false;
        DestroyThumbnails();

        parameter = parameter.ToUpper();

        var assetPacksData = AssetPackManager.instance.assetPacksData;
        var matches = new List<AssetData>();
        var assetPacksIndexes = new List<int>();

        for (int i = 0; i < assetPacksData.Length; i++)
        {
            foreach (var asset in assetPacksData[i].assets)
            {
                var assetName = asset.name.ToUpper();
                if (assetName.Contains(parameter))
                {
                    matches.Add(asset);
                    assetPacksIndexes.Add(i);
                    continue;
                }
                foreach (var tag in asset.tags)
                {
                    var tagName = tag.ToUpper();
                    if (tagName.Contains(parameter))
                    {
                        matches.Add(asset);
                        assetPacksIndexes.Add(i);
                        break;
                    }
                }
            }
        }

        AssetPackManager.instance.DownloadAssets(assetPacksIndexes, matches, DownloadCallback);

        foreach (var match in matches)
        {
            print(match.name);
        }
    }

    private Color _selectionColor = new Color();
    private Color _transparent = new Color(0, 0, 0, 0);
    private void DownloadCallback()
    {
        CreateButtons();
        searchingLabel.SetActive(false);
        searchInput.interactable = true;
        searchInput.ActivateInputField();
        StartCoroutine(MoveTextEndNextFrame());
        _selectionColor = searchInput.selectionColor;
        searchInput.selectionColor = _transparent;
    }

    IEnumerator MoveTextEndNextFrame()
    {
        yield return 0; 
        searchInput.MoveTextEnd(false);
        searchInput.selectionColor = _selectionColor;
    }


    private void DestroyThumbnails()
    {
        foreach (var thumbnail in _thumbnails)
        {
            DestroyImmediate(thumbnail.gameObject);
        }
        _thumbnails.Clear();
    }

    protected override void CreateButtons()
    {
        var assets = AssetPackManager.instance.temporalAssets;
        if (assets.Count == 0) notFoundLabel.gameObject.SetActive(true);
        for (int j = 0; j < assets.Count; j++)
        {
            var thumbnail = Instantiate(thumbnailPrefab);
            thumbnail.transform.SetParent(buttonsParent);
            thumbnail.transform.localScale = Vector3.one;
            var assetIndex = j;

            if (assets[j].asset.GetType() == typeof(GameObject))
            {
                var go = (GameObject)assets[j].asset;
                thumbnail.SetTexture(go.transform);
                thumbnail.button.onClick.AddListener(
                    delegate { AssetPackManager.instance.SpawnAsset(go); });

            }
            else if (assets[j].asset.GetType() == typeof(Texture2D))
            {
                var texture = (Texture2D)assets[j].asset;
                thumbnail.SetTexture(texture);

                thumbnail.button.onClick.AddListener(
                    delegate { SceneManagement.Instance.SetFloorTexture(texture); });

            }
            _thumbnails.Add(thumbnail);
        }
    }

    public void CleanButtons()
    {
        if (_thumbnails.Count == 0) return;

        foreach (var button in _thumbnails)
        {
            DestroyImmediate(button.gameObject);
        }
        _thumbnails.Clear();
    }
}
