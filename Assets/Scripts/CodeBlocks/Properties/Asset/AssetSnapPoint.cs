using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AssetSnapPoint : SnapPoint, IAssetDraggable, IPointerDownHandler
{
    [SerializeField] LayoutElement layoutElement;
    [SerializeField] float defaultWidth = 50.0f;
    [SerializeField] GameObject unSnapped;
    [SerializeField] TMP_Text setAssetText;
    MyAsset setAsset = null;
    public override bool IsSnappable(CodeBlock codeBlock)
    {
        return base.IsSnappable(codeBlock) &&
            codeBlock is PropertyCodeBlock propertyBlock &&
            (propertyBlock.propertyType & PropertyType.Asset) > 0;
    }
    private void Update()
    {
        if (layoutElement != null) layoutElement.minWidth = GetWidth();
    }
    protected override void OnSnappedChange()
    {
        unSnapped.SetActive(snapped == null);
        base.OnSnappedChange();
    }
    public MyAsset GetAsset(ulong hash)
    {
        if (snapped == null)
        {
            return setAsset;
        }
        else if (snapped is PropertyCodeBlock propertyBlock)
        {
            return propertyBlock.GetAsset(hash);
        }
        return null;
    }
    public void SetAsset(MyAsset asset)
    {
        if (setAsset != null)
        {
            setAsset.onRemove -= OnSetAssetRemove;
            setAsset.onDisplayUpdate -= OnSetAssetDisplayUpdate;
        }
        setAsset = asset;
        OnSetAssetDisplayUpdate();
        if (setAsset != null)
        {
            setAsset.onRemove += OnSetAssetRemove;
            setAsset.onDisplayUpdate += OnSetAssetDisplayUpdate;
        }
    }
    public void OnAssetDrag(MyAsset asset)
    {
        if (snapped != null) return;
        SetAsset(asset);
    }
    void OnSetAssetRemove() => SetAsset(null);
    void OnSetAssetDisplayUpdate() => setAssetText.text = setAsset != null ? setAsset.name : "None";
    public float GetWidth()
    {
        if (snapped is PropertyCodeBlock propertyBlock)
        {
            return propertyBlock.GetWidth();
        }
        else return defaultWidth;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.used) return;
        if (eventData.button == PointerEventData.InputButton.Middle && snapped == null)
        {
            eventData.Use();
            SetAsset(null);
        }
    }
    public override void Clear()
    {
        base.Clear();
        SetAsset(null);
    }
    public override SnapPointSave Save()
    {
        var save = base.Save();
        save.data.ulongs["setAsset"] = setAsset != null ? setAsset.uid : 0;
        return save;
    }
    public override void Load(SnapPointSave save)
    {
        base.Load(save);
        if (save.data.ulongs.TryGetValue("setAsset", out ulong assetID))
            SetAsset(EditorSceneManager.Instance.GetAsset<MyAsset>(assetID));
    }
}
