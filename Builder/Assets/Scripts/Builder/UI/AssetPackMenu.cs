using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetPacks;

public class AssetPackMenu : MonoBehaviour
{
    public AssetPackButton assetPackButton;

    private void Start()
    {
        CreateButtons();
    }

    private void CreateButtons()
    {
        foreach (var assetPack in AssetPackManager.instance.assetPacks)
        {
            var btn = Instantiate(assetPackButton);
            btn.transform.SetParent(transform);
            btn.transform.localScale = Vector3.one;
            btn.text.text = assetPack.name;
        }
    }

}
