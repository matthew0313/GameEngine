using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssetsTab : Tab
{
    [SerializeField] Button openButton;
    [SerializeField] Color openColor, closeColor;
    [SerializeField] TMP_InputField searchInput;
    [SerializeField] Transform elementAnchor;
    [SerializeField] AssetsTabElement elementPrefab;
    [SerializeField] Button reloadButton;
    readonly List<AssetsTabElement> elements = new();

    [field:SerializeField] public AssetType filter { get; private set; }
    public event Action<AssetType> onFilterChange;
    public override void Open()
    {
        base.Open();
        openButton.image.color = openColor;
        gameObject.SetActive(true);
    }
    public override void Close()
    {
        base.Close();
        openButton.image.color = closeColor;
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        EditorSceneManager.Instance.onAssetsChange += Refresh;
        searchInput.onEndEdit.AddListener(OnSearchInputEndEdit);
        reloadButton.onClick.AddListener(Reload);
        Refresh();
    }
    private void OnDisable()
    {
        EditorSceneManager.Instance.onAssetsChange -= Refresh;
        searchInput.onEndEdit.RemoveListener(OnSearchInputEndEdit);
        reloadButton.onClick.RemoveListener(Reload);
    }
    public void SetFilter(AssetType filter)
    {
        this.filter = filter;
        onFilterChange?.Invoke(filter);
        Refresh();
    }
    void OnSearchInputEndEdit(string value) => Refresh();
    void Refresh()
    {
        int i = 0;
        foreach (var asset in Filter())
        {
            if (elements.Count <= i) elements.Add(Instantiate(elementPrefab, elementAnchor));
            elements[i].gameObject.SetActive(true);
            elements[i++].Set(asset);
        }
        for (; i < elements.Count; i++) elements[i].gameObject.SetActive(false);
    }
    void Reload() => EditorSceneManager.Instance.ReloadAssets();
    IEnumerable<MyAsset> Filter()
    {
        List<MyAsset> assets = EditorSceneManager.Instance.assets;
        foreach(var asset in assets)
        {
            if (asset.name.StartsWith(searchInput.text, StringComparison.OrdinalIgnoreCase) && (asset.type & filter) > 0)
            {
                yield return asset;
            }
        }
    }
}