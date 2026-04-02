using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleTabElement : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text text;
    static Sprite infoIcon, warningIcon, errorIcon;
    public void Set(MyLog log)
    {
        infoIcon ??= Resources.Load<Sprite>("Images/LogInfoIcon");
        warningIcon ??= Resources.Load<Sprite>("Images/LogWarningIcon");
        errorIcon ??= Resources.Load<Sprite>("Images/LogErrorIcon");

        icon.sprite = log.type switch
        {
            MyLogType.Info => infoIcon,
            MyLogType.Warning => warningIcon,
            MyLogType.Error => errorIcon,
            _ => icon.sprite
        };
        text.text = $"{log.message}";
    }
}