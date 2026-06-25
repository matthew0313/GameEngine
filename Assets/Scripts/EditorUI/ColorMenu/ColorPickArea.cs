using UnityEngine;
using UnityEngine.EventSystems;

// Reports normalized (0..1) click/drag coordinates within its own RectTransform.
[RequireComponent(typeof(RectTransform))]
public class ColorPickArea : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    public System.Action<Vector2> onPick;
    RectTransform rt;

    void Awake() => rt = (RectTransform)transform;
    public void OnPointerDown(PointerEventData eventData) => Pick(eventData);
    public void OnDrag(PointerEventData eventData) => Pick(eventData);

    void Pick(PointerEventData eventData)
    {
        if (onPick == null) return;
        if (rt == null) rt = (RectTransform)transform;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out Vector2 local)) return;
        Rect r = rt.rect;
        float x = Mathf.Clamp01((local.x - r.x) / r.width);
        float y = Mathf.Clamp01((local.y - r.y) / r.height);
        onPick(new Vector2(x, y));
    }
}
