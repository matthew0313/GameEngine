using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AssetsTabElement : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text text;
    [SerializeField] GameObject selected;
    [SerializeField] int characterLimit = 20;
    MyAsset asset;
    public void Set(MyAsset asset)
    {
        if (this.asset != null) this.asset.onUpdate -= OnAssetUpdate;
        this.asset = asset;
        this.asset.onUpdate += OnAssetUpdate;
        OnAssetUpdate();
    }
    private void OnEnable()
    {
        EditorSceneManager.Instance.onSelect += OnSelect;
        OnSelect(EditorSceneManager.Instance.selected);
    }

    private void OnDisable()
    {
        EditorSceneManager.Instance.onSelect -= OnSelect;
    }
    private void OnDestroy()
    {
        if(asset != null) asset.onUpdate -= OnAssetUpdate;
    }
    private void OnAssetUpdate()
    {
        if (asset is ImageAsset imageAsset)
        {
            icon.sprite = imageAsset.sprite;
        }
        if(asset.name.Length > characterLimit) text.text = asset.name.Substring(0, characterLimit) + "..";
        else text.text = asset.name;
    }
    private void OnSelect(ISelectable obj)
    {
        selected.SetActive(asset != null && asset == obj);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.button != PointerEventData.InputButton.Left) return;
        if (eventData.used) return;
        EditorSceneManager.Instance.Select(asset);
    }
}