using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SnapPoint_InvokeFunctionArg : WildcardSnapPoint, IPointerDownHandler
{
    Codeblock_InvokeFunction target;
    public int index;
    public void BindTarget(Codeblock_InvokeFunction target)
    {
        this.target = target;
    }
    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        if (eventData.used) return;
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            EditorSceneManager.Instance.rightClickMenu.Open(eventData.position, MakeRightClickMenu());
            eventData.Use();
        }
    }
    IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Remove Parameter",
            ctx =>
            {
                if (target == null) return;
                target.RemoveArgument(index);
            });
    }
}