using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EditorSceneManager : MonoBehaviour
{
    public static EditorSceneManager Instance { get; private set; }
    public string projectName { get; private set; }

    [SerializeField] CodeBlockList codeBlockList;
    [SerializeField] string savePath = "Saves";

    [Header("Tabs")]
    [SerializeField] CodeTab m_codeTab;
    public CodeTab codeTab => m_codeTab;

    public readonly List<MyAsset> assets = new();
    public readonly List<MyLog> logs = new();
    public readonly List<SnapPoint> snapPoints = new();
    public event Action onAssetsReload, onLogsChange;
    private void Awake()
    {
        Instance = this;
    }
    public void LoadProject(string projectName)
    {
        this.projectName = projectName;
        RefreshAssets();
    }
    public void RefreshAssets()
    {
        string assetsPath = Path.Combine(savePath, projectName, "Assets");
        assets.Clear();
        foreach(string filePath in Directory.GetFiles(assetsPath))
        {
            if (filePath.EndsWith(".png"))
            {
                assets.Add(new ImageAsset(filePath));
            }
        }
        onAssetsReload?.Invoke();
    }
    public T GetAsset<T>(string filePath) where T : MyAsset
    {
        MyAsset asset = assets.Find(a => a.filePath == filePath);
        if(asset != null && asset is T t) return t;
        return null;
    }
    public void AddLog(MyLog log)
    {
        logs.Add(log);
        onLogsChange?.Invoke();
    }
    public CodeBlock IDToBlock(string id)
    {
        foreach(var block in codeBlockList.codeBlocks)
        {
            if(block.id == id) return block;
        }
        return null;
    }
}
public enum MyLogType
{
    Info,
    Warning,
    Error
}
public struct MyLog
{
    public MyLog(MyLogType type, string message)
    {
        this.type = type;
        this.message = message;
    }
    public MyLogType type;
    public string message;
}
