using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AssetPacks;
using Console;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class BuilderUI : MonoBehaviour
{
    public static BuilderUI Instance; 
    
    private void Awake()
    {
        if (BuilderUI.Instance == null) Instance = this;
        else throw new Exception("There are more than one BuilderUI instance.");
    }

    private AssetPackManager _apm;
    private bool _sceneCountActive;

    public AssetPackMenu assetPackMenu;
    public SectionMenu sectionMenu;
    public SearchAssetMenu searchAssetMenu;
    
    public SceneCounts sceneCountsPopup;
    public GameObject loadingSceneLabel;
    [FormerlySerializedAs("console")] public Terminal terminal;
    private Menu[] _menus;
    private bool _consoleActivated;


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

    private int _savedParameterLength;
    public float timeUntilSearch = 1;
    public void Search(string parameter)
    {
        _savedParameterLength = parameter.Length;
        searchAssetMenu.CleanButtons();
        searchAssetMenu.notFoundLabel.gameObject.SetActive(false);
        searchAssetMenu.searchLabel.gameObject.SetActive(true);
        searchAssetMenu.searchLabel.text = $"Search \"{parameter}\"";
        ShowSearchAssetMenu();

        if (PassSearchValidations(parameter))
        {
            StartCoroutine(MakeSearch(parameter));
        }
        else if(parameter.Length == 0)
        {
            ShowAssetPackMenu();
        }
    }

    private bool PassSearchValidations(string parameter)
    {
        if (parameter == "")
            return false;
        if (parameter.Length < 3)
            return false;


        return true;
    }

    private IEnumerator MakeSearch(string parameter)
    {
        yield return new WaitForSeconds(timeUntilSearch);
        if(parameter.Length == _savedParameterLength)
        {
            searchAssetMenu.searchLabel.gameObject.SetActive(false);
            searchAssetMenu.searchingLabel.SetActive(true);

            searchAssetMenu.Search(parameter);
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            _consoleActivated = !_consoleActivated;
            terminal.gameObject.SetActive(_consoleActivated);
        }
    }
}
