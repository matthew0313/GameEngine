using System;
using TMPro;
using UnityEngine;

public class InspectorUIAsset : InspectorUIElement, IAssetDraggable
{
    [SerializeField] TMP_Text label;
    [SerializeField] TMP_Text assetName;

    ExposedAsset element;
    MyAsset asset;

    public void OnAssetDrag(MyAsset asset)
    {
        if (element == null) return;
        if ((asset.type | element.assetType) == 0) return;
        element.setter(asset);
    }
    public void Set(ExposedAsset element)
    {
        this.element = element;
        label.text = element.name + $" ({Enum.GetName(typeof(AssetType), element.assetType)})";
        asset = element.getter();
        assetName.text = asset != null ? asset.name : "None";
    }
    private void Update()
    {
        if (element == null) return;
        MyAsset tmp = element.getter();
        if (tmp != asset)
        {
            asset = tmp;
            assetName.text = asset != null ? asset.name : "None";
        }
    }
}
