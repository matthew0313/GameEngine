using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ObjectDragIcon : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text text;
    public void Show(MyGameObject obj)
    {
        gameObject.SetActive(true);
        icon.sprite = obj.icon;
        text.text = obj.name;
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}