using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetPacks;
using UnityEngine.UI;

public class AssetPackMenu : Menu
{
    public AssetPackButton assetPackButton;
    private List<AssetPackButton> _buttons = new List<AssetPackButton>();
    //public delegate void ButtonListener(int assetPackIndex);
    //public ButtonListener buttonListener;
    public SectionMenu sectionMenu;
    public BuilderUI builderUI;
    public Sprite defaultAssetPackImage;
    private void Start()
    {
        SceneManagement.instance.loadedSceneCallback = LoadedSceneCallback;
        CreateButtons();
        AssetPackManager.instance.DownloadAssetPacksImages(SetImages);
    }

    protected override void CreateButtons()
    {
        var assetPacks = AssetPackManager.instance.assetPacks;

        for(int i = 0; i < assetPacks.Count; i++)
        {
            var btn = Instantiate(assetPackButton);
            btn.transform.SetParent(transform);
            btn.transform.localScale = Vector3.one;
            btn.text.text = assetPacks[i].name;
            btn.image.sprite = defaultAssetPackImage;
            int assetPackIndex = i;
            btn.button.onClick.AddListener(delegate { ButtonListener(assetPackIndex); } );
            _buttons.Add(btn);
        }
    }

    public void ButtonListener(int assetPackIndex)
    {
        sectionMenu.selectedAssetPack = assetPackIndex;
        builderUI.ShowSectionMenu(assetPackIndex);
    }

    private void LoadedSceneCallback()
    {
        DeleteButtons();
        CreateButtons();
        SetImages();
    }

    private void SetImages()
    {
        for (int i = 0; i < _buttons.Count; i++)
        {
            var image = AssetPackManager.instance.assetPacks[i].image;
            if(image != null)
            _buttons[i].image.sprite = image;
        }
    }

    private void DeleteButtons()
    {
        for (int i = 0; i < _buttons.Count; i++)
        {
            DestroyImmediate(_buttons[i].gameObject);
        }
        _buttons.Clear();
    }

}
