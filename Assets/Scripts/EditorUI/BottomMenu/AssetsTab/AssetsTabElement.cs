using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AssetsTabElement : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text text;
    [SerializeField] GameObject selected;
    [SerializeField] int characterLimit = 20;
    MyAsset asset;
    public void Set(MyAsset asset)
    {
        if (this.asset != null) this.asset.onDisplayUpdate -= OnDisplayUpdate;
        this.asset = asset;
        this.asset.onDisplayUpdate += OnDisplayUpdate;
        OnDisplayUpdate();
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
        if(asset != null) asset.onDisplayUpdate -= OnDisplayUpdate;
    }
    private void OnDisplayUpdate()
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
    bool selectQueued = false;
    public void OnPointerDown(PointerEventData eventData)
    {
        selectQueued = true;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (!selectQueued || eventData.button != PointerEventData.InputButton.Left) return;
        if (eventData.used) return;
        EditorSceneManager.Instance.Select(asset);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        selectQueued = false;
    }

    static AssetDragIcon dragIcon;
    public void OnDrag(PointerEventData eventData)
    {
        dragIcon.transform.position = eventData.position;
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        dragIcon ??= Instantiate(Resources.Load<AssetDragIcon>("AssetDragIcon"), EditorSceneManager.Instance.canvas.transform);
        dragIcon.Show(asset);
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        foreach (var hit in UIScanner.ScanUI(eventData.position))
        {
            if (hit.gameObject.TryGetComponentInParents(out IAssetDraggable target))
            {
                target.OnAssetDrag(asset);
                break;
            }
        }
        dragIcon.Hide();
    }
}