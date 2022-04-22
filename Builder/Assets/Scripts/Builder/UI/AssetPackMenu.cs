using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetPacks;

public class AssetPackMenu : Menu
{
    public AssetPackButton assetPackButton;
    private List<AssetPackButton> _buttons = new List<AssetPackButton>();
    //public delegate void ButtonListener(int assetPackIndex);
    //public ButtonListener buttonListener;
    public SectionMenu sectionMenu;
    public BuilderUI builderUI;

    private void Start()
    {
        CreateButtons();
        AssetPackManager.instance.DownloadAssetPacksImages(SetImages);
    }

    protected override void CreateButtons()
    {
        var assetPacks = AssetPackManager.instance.assetPacks;

        for(int i = 0; i < assetPacks.Length; i++)
        {
            var btn = Instantiate(assetPackButton);
            btn.transform.SetParent(transform);
            btn.transform.localScale = Vector3.one;
            btn.text.text = assetPacks[i].name;
            int assetPackIndex = i;
            btn.button.onClick.AddListener(delegate { ButtonListener(assetPackIndex); } );
            _buttons.Add(btn);
        }
    }

    public void ButtonListener(int assetPackIndex)
    {
        sectionMenu.selectedAssetPack = assetPackIndex;
        builderUI.ShowSectionMenu(assetPackIndex);
        //sectionMenu.CreateMenu(assetPackIndex);
    }
    //public void Listener(int assetPackIndex)
    //{
    //    print("Listener " + assetPackIndex);
    //    buttonListener(assetPackIndex);
    //}

    private void SetImages()
    {
        for (int i = 0; i < _buttons.Count; i++)
        {
            _buttons[i].image.sprite = AssetPackManager.instance.assetPacks[i].image;
        }
    }

}
