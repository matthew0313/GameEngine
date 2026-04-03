using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleTab : Tab
{
    [SerializeField] Button openButton, clearButton;
    [SerializeField] Color openColor, closeColor;
    [SerializeField] Transform elementAnchor;
    [SerializeField] ConsoleTabElement elementPrefab;
    readonly List<ConsoleTabElement> elements = new List<ConsoleTabElement>();
    public override void Open()
    {
        base.Open();
        openButton.image.color = openColor;
        clearButton.onClick.AddListener(Clear);
        gameObject.SetActive(true);
    }
    public override void Close()
    {
        base.Close();
        openButton.image.color = closeColor;
        clearButton.onClick.RemoveListener(Clear);
        gameObject.SetActive(false);
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
        int i = 0;
        for(; i < EditorSceneManager.Instance.logs.Count; i++)
        {
            if (elements.Count <= i) elements.Add(Instantiate(elementPrefab, elementAnchor));
            elements[i].gameObject.SetActive(true);
            elements[i].Set(EditorSceneManager.Instance.logs[i]);
        }
        for(; i < elements.Count; i++)
        {
            elements[i].gameObject.SetActive(false);
        }
    }
    void Clear() => EditorSceneManager.Instance.ClearLog();
}