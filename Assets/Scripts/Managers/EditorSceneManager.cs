using GLTFast.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class EditorSceneManager : MonoBehaviour
{
    public static EditorSceneManager Instance { get; private set; }
    public string projectName { get; private set; } = "DebugProject";

    [SerializeField] MyGameObjectList myGameObjectList;
    [SerializeField] CodeBlockList codeBlockList;
    [SerializeField] string savePath = "Saves";
    [field:SerializeField] public MyScene myScene { get; private set; }
    public string projectSavePath => Path.Combine(savePath, projectName);

    [Header("Screen")]
    [field:SerializeField] public Vector2Int screenSize { get; private set; }
    [field:SerializeField] public ScriptGrid scriptGrid { get; private set; }

    public readonly List<MyAsset> assets = new();
    public readonly List<SnapPoint> snapPoints = new();
    public event Action onAssetsReload;

    public ISelectable selected { get; private set; } = null;
    public event Action<ISelectable> onSelect;

    public readonly List<MyLog> logs = new();
    public event Action onLogsChange;
    public void Select(ISelectable selectable)
    {
        selected = selectable;
        onSelect?.Invoke(selectable);
    }
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        ReloadAssets();
        AddLog(new()
        {
            type = MyLogType.Info,
            message = "Test Log"
        });
    }
    public void ReloadAssets()
    {
        string assetsPath = Path.Combine(projectSavePath, "Assets");
        foreach(string filePath in Directory.GetFiles(assetsPath))
        {
            if(assets.Find(a => a is IFileAsset fileAsset && fileAsset.filePath == filePath) != null) continue;
            if (filePath.EndsWith(".png"))
            {
                var tmp = new ImageAsset()
                {
                    name = Path.GetFileNameWithoutExtension(filePath)
                };
                tmp.LoadFile(filePath);
                assets.Add(tmp);
            }
            else
            {
                AddLog(new()
                {
                    type = MyLogType.Warning,
                    message = $"Unsupported fileAsset type: {filePath}"
                });
                continue;
            }
        }
        onAssetsReload?.Invoke();
    }
    public T GetAsset<T>(ulong uid) where T : MyAsset
    {
        MyAsset asset = assets.Find(a => a.uid == uid);
        if(asset != null && asset is T t) return t;
        return null;
    }
    public void AddLog(MyLog log)
    {
        logs.Insert(0, log);
        onLogsChange?.Invoke();
    }
    public void ClearLog()
    {
        logs.Clear();
        onLogsChange?.Invoke();
    }
    public CodeBlock IDToBlock(string id) => codeBlockList.IDToBlock(id);
    public MyGameObject IDToGameObject(string id)
    {
        foreach(var obj in myGameObjectList.myGameObjects)
        {
            if(obj.id == id) return obj;
        }
        return null;
    }
    public ProjectSave Save()
    {
        ProjectSave save = new();
        save.projectName = projectName;
        save.scene = myScene.Save();
        foreach(var asset in assets) save.assets.Add(asset.Save());
        return save;
    }
    public void Load(ProjectSave save)
    {
        projectName = save.projectName;
        myScene.Load(save.scene);
        foreach(var assetSave in save.assets)
        {
            MyAsset added = null;
            if (assetSave.type == AssetType.Image) added = new ImageAsset();
            else if (assetSave.type == AssetType.Prefab) added = new PrefabAsset();
            else
            {
                AddLog(new()
                {
                    type = MyLogType.Warning,
                    message = $"Unsupported asset type for asset \'{assetSave.name}\'"
                });
                continue;
            }
            added.Load(assetSave);
            assets.Add(added);
        }
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
        this.time = DateTime.Now;
        this.message = message;
    }
    public MyLogType type;
    public DateTime time;
    public string message;
}
[System.Serializable]
public class ProjectSave
{
    public string projectName;
    public MySceneSave scene = new();
    public List<MyAssetSave> assets = new();
}