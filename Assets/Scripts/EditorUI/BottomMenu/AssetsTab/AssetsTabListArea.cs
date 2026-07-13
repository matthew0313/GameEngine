using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AssetsTabListArea : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.used) return;
        if(eventData.button == PointerEventData.InputButton.Right)
        {
            eventData.Use();
            EditorSceneManager.Instance.rightClickMenu.Open(Input.mousePosition, MakeRightClickMenu());
        }
    }
    IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Foldout("Create Asset", MakeRightClickMenu_Create());
    }
    IEnumerable<RCMenuElement> MakeRightClickMenu_Create()
    {
        yield return new RCMenuElement_Button("Scene", ctx =>
        {
            EditorSceneManager.Instance.AddAsset(EditorSceneManager.Instance.CreateAsset(AssetType.Scene));
        });
    }
}