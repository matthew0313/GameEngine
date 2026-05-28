using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class InspectorUIAsset : InspectorUIElement, IAssetDraggable, IPointerDownHandler
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
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.used) return;
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            EditorSceneManager.Instance.rightClickMenu.Open(Input.mousePosition, MakeRightClickMenu());
        }
    }
    IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Clear",
            ctx =>
            {
                if (element == null) return;
                element.setter(null);
            });
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
