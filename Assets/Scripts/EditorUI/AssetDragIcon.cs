using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AssetDragIcon : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text text;
    public void Show(MyAsset asset)
    {
        gameObject.SetActive(true);
        icon.sprite = asset is ImageAsset imageAsset ? imageAsset.sprite : null;
        text.text = asset.name;
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
