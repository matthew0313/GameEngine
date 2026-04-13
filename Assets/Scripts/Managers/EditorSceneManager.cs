using GLTFast.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Threading;



#if UNITY_EDITOR
using UnityEditor;
#endif

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
    [SerializeField] GraphicRaycaster raycaster;
    [field:SerializeField] public Vector2Int screenSize { get; private set; }
    [field:SerializeField] public RightClickMenu rightClickMenu { get; private set; }
    [field:SerializeField] public ScriptGrid scriptGrid { get; private set; }
    [field:SerializeField] public HierarchyUI hierarchy { get; private set; }

    public readonly List<MyAsset> assets = new();
    public readonly List<SnapPoint> snapPoints = new();
    public event Action onAssetsChange;

    public ISelectable selected { get; private set; } = null;
    public event Action<ISelectable> onSelect;

    public readonly List<MyLog> logs = new();
    public event Action onLogsChange;

    public ControlMachine<bool> raycastControl { get; private set; }
    EventSystem eventSystem;
    public void Select(ISelectable selectable)
    {
        selected = selectable;
        onSelect?.Invoke(selectable);
    }
    private void Awake()
    {
        Instance = this;
        eventSystem = EventSystem.current;
        raycastControl = new(value => raycaster.enabled = value, true);
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
    private void Update()
    {
        if (InputManager.GetKeyDown(KeyCode.Escape)) Select(null);
    }
    readonly List<RaycastResult> raycastResults = new();
    public List<RaycastResult> RaycastUI(Vector2 position)
    {
        PointerEventData pointerData = new PointerEventData(eventSystem) { position = position };
        raycastResults.Clear();
        raycaster.Raycast(pointerData, raycastResults);
        return raycastResults;
    }
    public void AddAsset(MyAsset asset)
    {
        assets.Add(asset);
        onAssetsChange?.Invoke();
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
        onAssetsChange?.Invoke();
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
        Dictionary<MyAsset, MyAssetSave> saves = new();
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
            added.EarlyLoad(assetSave);
            assets.Add(added);
            saves.Add(added, assetSave);
        }
        foreach(var i in saves) i.Key.Load(i.Value);
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
#if UNITY_EDITOR
[CustomEditor(typeof(EditorSceneManager))]
public class EditorSceneManager_Editor : Editor
{
    int count = 0;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorSceneManager target = (this.target as EditorSceneManager);
        GUILayout.Space(10);
        GUILayout.Label("Debug");
        if (GUILayout.Button("Test Prefab"))
        {
            var tmp = new PrefabAsset() { name = "Test Prefab" };
            tmp.Set(Instantiate(target.IDToGameObject("Sprite")));
            target.AddAsset(tmp);
        }
        if(GUILayout.Button("Test Top GO"))
        {
            var tmp = Instantiate(target.IDToGameObject("Sprite"));
            tmp.name = $"Sprite{count++}";
            target.myScene.AddChild(tmp);
        }
        if(GUILayout.Button("Test Child GO"))
        {
            var tmp = Instantiate(target.IDToGameObject("Sprite"));
            var tmp2 = Instantiate(target.IDToGameObject("Sprite"));
            tmp.AddChild(tmp2);
            target.myScene.AddChild(tmp);
        }
    }
}
#endif