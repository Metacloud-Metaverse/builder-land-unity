using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetPacks;

public class SearchAssetMenu : Menu
{
    public Thumbnail thumbnailPrefab;
    public RectTransform buttonsParent;
    private List<Thumbnail> _thumbnails = new();


    public void Search(string parameter)
    {
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


    private void DownloadCallback()
    {
        CreateButtons();
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
                    delegate { SceneManagement.instance.SetFloorTexture(texture); });

            }
            _thumbnails.Add(thumbnail);
        }
    }
}
