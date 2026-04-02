using UnityEngine;
using UnityEngine.UI;
using PrimeTween;

public class BottomMenu : MonoBehaviour
{
    [SerializeField] Tab openTab;
    [SerializeField] Button closeButton;

    [Header("Tween")]
    [SerializeField] RectTransform rectTransform;
    [SerializeField] RectTransform up;
    [SerializeField] float moveTweenDuration = 0.5f;
    [SerializeField] Ease moveTweenEase = Ease.OutCirc;
    [SerializeField] Transform closeIcon;
    private void OnEnable()
    {
        closeButton.onClick.AddListener(Toggle);
    }
    private void OnDisable()
    {
        closeButton.onClick.RemoveListener(Toggle);
    }
    public void OpenTab(Tab tab)
    {
        if (openTab == tab) return;
        if (openTab != null) openTab.Close();
        openTab = tab;
        if(openTab != null) openTab.Open();
    }
    bool open = true;
    void Toggle()
    {
        if (!open)
        {
            open = true;
            Tween.UIAnchoredPositionY(rectTransform, 0.0f, moveTweenDuration, moveTweenEase);
            Tween.UIOffsetMinY(up, rectTransform.rect.height, moveTweenDuration, moveTweenEase);
        }
        else
        {
            open = false;
            Tween.UIAnchoredPositionY(rectTransform, -rectTransform.rect.height, moveTweenDuration, moveTweenEase);
            Tween.UIOffsetMinY(up, 0.0f, moveTweenDuration, moveTweenEase);
        }
    }
}