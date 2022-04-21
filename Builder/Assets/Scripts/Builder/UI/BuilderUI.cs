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
    public SceneCounts sceneCountsPopup;

    public void ShowAssetPackMenu()
    {
        sectionMenu.enabled = false;
        sectionMenu.gameObject.SetActive(false);

        assetPackMenu.gameObject.SetActive(true);
        assetPackMenu.enabled = true;
    }

    public void ShowSectionMenu(int assetPack)
    {
        assetPackMenu.enabled = false;
        assetPackMenu.gameObject.SetActive(false);

        sectionMenu.gameObject.SetActive(true);
        sectionMenu.enabled = true;
    }

    public void SwitchSceneCountPopUp()
    {
        _sceneCountActive = !_sceneCountActive;
        sceneCountsPopup.gameObject.SetActive(_sceneCountActive);
        if (_sceneCountActive)
            sceneCountsPopup.RefreshCounts();
    }
}
