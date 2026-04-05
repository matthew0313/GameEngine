using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AssetsTabFilterListElement : MonoBehaviour
{
    [SerializeField] Button button;
    [SerializeField] GameObject active;
    [SerializeField] TMP_Text text;
    AssetsTab tab;
    AssetType filter;

    bool filterEnabled
    {
        get => (tab.filter & filter) > 0;
        set
        {
            if (value) tab.SetFilter(tab.filter | filter);
            else tab.SetFilter(tab.filter & ~filter);
        }
    }
    public void Set(AssetsTab tab, AssetType filter)
    {
        this.tab = tab;
        this.filter = filter;
        text.text = Enum.GetName(typeof(AssetType), filter);
        tab.onFilterChange += OnFilterChange;
    }
    private void OnEnable()
    {
        button.onClick.AddListener(OnClick);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
    }
    void OnClick() => filterEnabled = !filterEnabled;
    private void OnDestroy()
    {
        if (tab != null) tab.onFilterChange -= OnFilterChange;
    }
    private void OnFilterChange(AssetType filter) => active.SetActive(filterEnabled);
}