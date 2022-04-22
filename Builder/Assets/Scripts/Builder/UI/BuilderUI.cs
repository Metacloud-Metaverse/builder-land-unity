using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetPacks;
using UnityEngine.UI;

public class BuilderUI : MonoBehaviour
{
    private AssetPackManager _apm;
    private bool _sceneCountActive;

    public AssetPackMenu assetPackMenu;
    public SectionMenu sectionMenu;
    public SearchAssetMenu searchAssetMenu;

    public SceneCounts sceneCountsPopup;

    private Menu[] _menus;


    private void Start()
    {
        SetMenuArray(assetPackMenu, sectionMenu, searchAssetMenu);
    }


    private void SetMenuArray(params Menu[] menus)
    {
        _menus = new Menu[menus.Length];
        for (int i = 0; i < _menus.Length; i++)
        {
            _menus[i] = menus[i];
        }
    }


    private void HideMenus()
    {
        foreach (var menu in _menus)
        {
            menu.enabled = false;
            menu.gameObject.SetActive(false);
        }
    }


    public void ShowAssetPackMenu()
    {
        HideMenus();

        assetPackMenu.gameObject.SetActive(true);
        assetPackMenu.enabled = true;
    }


    public void ShowSectionMenu(int assetPack)
    {
        HideMenus();
        sectionMenu.gameObject.SetActive(true);
        sectionMenu.enabled = true;
    }


    public void ShowSearchAssetMenu()
    {
        HideMenus();

        searchAssetMenu.gameObject.SetActive(true);
        searchAssetMenu.enabled = true;
    }


    public void SwitchSceneCountPopUp()
    {
        _sceneCountActive = !_sceneCountActive;
        sceneCountsPopup.gameObject.SetActive(_sceneCountActive);
        if (_sceneCountActive)
            sceneCountsPopup.RefreshCounts();
    }


    public void Search(string parameter)
    {
        if(parameter != "")
        {
            ShowSearchAssetMenu();
            searchAssetMenu.Search(parameter);
        }
        else
        {
            ShowAssetPackMenu();
        }
    } 
}
