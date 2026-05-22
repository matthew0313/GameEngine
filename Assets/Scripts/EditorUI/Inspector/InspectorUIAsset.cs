using TMPro;
using UnityEngine;

public class InspectorUIAsset : InspectorUIElement, IAssetDraggable
{
    [SerializeField] TMP_Text assetName;

    ExposedAsset element;
    MyAsset asset;

    public void OnAssetDrag(MyAsset asset)
    {
        if (element == null) return;
        if (element.condition != null && !element.condition(asset)) return;
        element.setter(asset);
    }
    public void Set(ExposedAsset element)
    {
        this.element = element;
        this.asset = element.getter();
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
