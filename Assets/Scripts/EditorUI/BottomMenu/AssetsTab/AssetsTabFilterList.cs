using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AssetsTabFilterList : MonoBehaviour
{
    [SerializeField] AssetsTab assetsTab;
    [SerializeField] Transform filterElementAnchor;
    [SerializeField] AssetsTabFilterListElement filterElementPrefab;
    bool initialized = false;
    void Initialize()
    {
        foreach (AssetType i in Enum.GetValues(typeof(AssetType)))
        {
            var tmp = Instantiate(filterElementPrefab, filterElementAnchor);
            tmp.gameObject.SetActive(true);
            tmp.Set(assetsTab, i);
        }
    }
    bool skipFrame = false;
    private void OnEnable()
    {
        if (!initialized)
        {
            initialized = true;
            Initialize();
        }
        skipFrame = true;
    }
    private void Update()
    {
        if (skipFrame)
        {
            skipFrame = false; return;
        }
        if (Input.GetMouseButtonDown(0))
        {
            bool clicked = false;
            foreach(var hit in UIScanner.ScanUI(Input.mousePosition))
            {
                if (hit.gameObject == gameObject) clicked = true;
            }
            if (!clicked) gameObject.SetActive(false);
        }
    }
}