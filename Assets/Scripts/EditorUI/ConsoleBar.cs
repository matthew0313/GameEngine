using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class ConsoleBar : MonoBehaviour
{
    [SerializeField] Image icon;
    [SerializeField] TMP_Text text;
    [SerializeField] Color infoColor, warningColor, errorColor;
    Sprite infoIcon, warningIcon, errorIcon;
    private void Awake()
    {
        infoIcon = Resources.Load<Sprite>("Images/LogInfoIcon");
        warningIcon = Resources.Load<Sprite>("Images/LogWarningIcon");
        errorIcon = Resources.Load<Sprite>("Images/LogErrorIcon");
    }
    private void OnEnable()
    {
        EditorSceneManager.Instance.onLogsChange += OnLogsChange;
        OnLogsChange();
    }

    private void OnDisable()
    {
        EditorSceneManager.Instance.onLogsChange -= OnLogsChange;
    }
    private void OnLogsChange()
    {
        if(EditorSceneManager.Instance.logs.Count == 0)
        {
            icon.gameObject.SetActive(false);
            text.gameObject.SetActive(false);
            return;
        }

        MyLog log = EditorSceneManager.Instance.logs[0];
        text.color = log.type switch
        {
            MyLogType.Info => infoColor,
            MyLogType.Warning => warningColor,
            MyLogType.Error => errorColor,
            _ => text.color
        };
        icon.sprite = log.type switch
        {
            MyLogType.Info => infoIcon,
            MyLogType.Warning => warningIcon,
            MyLogType.Error => errorIcon,
            _ => icon.sprite
        };
        text.text = log.message;
        icon.gameObject.SetActive(true);
        text.gameObject.SetActive(true);
    }
}