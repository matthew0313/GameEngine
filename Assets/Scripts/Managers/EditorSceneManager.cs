using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;





#if UNITY_EDITOR
using UnityEditor;
#endif

public class EditorSceneManager : MonoBehaviour
{
    public static EditorSceneManager Instance { get; private set; }
    public string projectName { get; private set; } = "DebugProject";

    [field:SerializeField] public MyGameObjectList myGameObjectList { get; private set; }
    [field:SerializeField] public CodeBlockList codeBlockList { get; private set; }
    [field:SerializeField] public Canvas canvas { get; private set; }
    [field:SerializeField] public MyScene myScene { get; private set; }

    [SerializeField] string savePath = "Saves";
    public string projectSavePath => Path.Combine(savePath, projectName);

    [Header("Screen")]
    [SerializeField] GraphicRaycaster raycaster;
    [field:SerializeField] public Vector2Int screenSize { get; private set; }
    [field:SerializeField] public RightClickMenu rightClickMenu { get; private set; }
    [field:SerializeField] public ScriptGrid scriptGrid { get; private set; }
    [field:SerializeField] public HierarchyUI hierarchy { get; private set; }
    [field:SerializeField] public SceneScreenController sceneScreen { get; private set; }
    [field:SerializeField] public ScreenUI screen { get; private set; }
    [field:SerializeField] public ColorMenu colorMenu { get; private set; }

    public readonly List<MyAsset> assets = new();
    public readonly List<SnapPoint> snapPoints = new();
    public event Action onAssetsChange;

    public ISelectable selected { get; private set; } = null;
    public event Action<ISelectable> onSelect;

    public readonly List<MyLog> logs = new();
    public event Action onLogsChange;

    public ControlMachine<bool> raycastControl { get; private set; }
    EventSystem eventSystem;

    public CopyBufferItemType copyBufferItemType = CopyBufferItemType.None;
    public string copyBuffer;
    public void Select(ISelectable selectable)
    {
        if (selected != null) selected.OnDeselect();
        selected = selectable;
        if (selected != null) selected.OnSelect();
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
        if (!string.IsNullOrEmpty(GlobalManager.Instance.currentProjectName))
        {
            projectName = GlobalManager.Instance.currentProjectName;
            Load(GlobalManager.Instance.LoadProject(projectName));
        }
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
        if (InputManager.GetKeyDown(KeyCode.Delete))
        {
            if(selected != null)
            {
                if(selected is MyGameObject myGameObject) myGameObject.Delete();
                else if(selected is MyAsset myAsset) RemoveAsset(myAsset);
            }
        }
        if (Input.GetKey(KeyCode.LeftControl) && InputManager.GetKeyDown(KeyCode.C))
        {
            if (selected is MyGameObject myGameObject)
            {
                copyBufferItemType = CopyBufferItemType.MyGameObject;
                copyBuffer = SaveSerializer.Serialize(myGameObject.Save());
            }
            else if (selected is MyAsset myAsset)
            {
                copyBufferItemType = CopyBufferItemType.MyAsset;
                copyBuffer = SaveSerializer.Serialize(myAsset.Save());
            }
        }
        if (Input.GetKey(KeyCode.LeftControl) && InputManager.GetKeyDown(KeyCode.V)) Paste();
        if (Input.GetKey(KeyCode.LeftControl) && InputManager.GetKeyDown(KeyCode.S)) SaveProject();
        if (playMode)
        {
            foreach (var i in myScene.GetObjects())
            {
                i.OnUpdate();
            }
        }
    }
    public void CopyBlock(CodeBlock block)
    {
        copyBufferItemType = CopyBufferItemType.CodeBlock;
        copyBuffer = SaveSerializer.Serialize(block.Save(), true);
    }
    public void Paste()
    {
        if (copyBufferItemType == CopyBufferItemType.MyGameObject)
        {
            MyGameObjectSave save = SaveSerializer.Deserialize<MyGameObjectSave>(copyBuffer);
            MyGameObject obj = Instantiate(IDToObjectPrefab(save.id));
            obj.EarlyLoad(save, true);
            myScene.AddChild(obj);
            obj.Load(save);
        }
        else if (copyBufferItemType == CopyBufferItemType.MyAsset)
        {
            MyAssetSave save = SaveSerializer.Deserialize<MyAssetSave>(copyBuffer);
            MyAsset asset = MyAsset.TypeToAsset(save.type);
            if (asset != null)
            {
                asset.EarlyLoad(save, true);
                AddAsset(asset);
                asset.Load(save);
            }
        }
        else if (copyBufferItemType == CopyBufferItemType.CodeBlock && scriptGrid.editing != null)
        {
            CodeBlockSave save = SaveSerializer.Deserialize<CodeBlockSave>(copyBuffer);
            CodeBlock blockPrefab = IDToBlockPrefab(save.id);
            if (blockPrefab != null)
            {
                CodeBlock block = Instantiate(blockPrefab, scriptGrid.transform);
                block.Set(scriptGrid.editing);
                scriptGrid.editing.codeBlocks.Add(block);
                scriptGrid.BindToGrid(block);
                block.EarlyLoad(save, true);
                block.Load(save);
            }
        }
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
        if (assets.Contains(asset)) return;
        assets.Add(asset);
        onAssetsChange?.Invoke();
    }
    public void RemoveAsset(MyAsset asset)
    {
        if (!assets.Contains(asset)) return;
        assets.Remove(asset);
        asset.OnRemove();
        onAssetsChange?.Invoke();
    }
    public void ReloadAssets()
    {
        string assetsPath = Path.Combine(projectSavePath, "Assets");
        foreach(string filePath in Directory.GetFiles(assetsPath))
        {
            if(assets.Find(a => a is FileAsset fileAsset && fileAsset.filePath == filePath) != null) continue;
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
                    message = $"Unsupported file extension found while loading Assets: {Path.GetExtension(filePath)}"
                });
                continue;
            }
        }
        onAssetsChange?.Invoke();
    }
    public MyAsset FindAsset(Func<MyAsset, bool> predicate) => assets.Find(predicate);
    public T GetAsset<T>(ulong uid) where T : MyAsset
    {
        MyAsset asset = FindAsset(item => item.uid == uid);
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
    public CodeBlock IDToBlockPrefab(string id) => codeBlockList.IDToBlockPrefab(id);
    public MyGameObject IDToObjectPrefab(string id)
    {
        foreach(var obj in myGameObjectList.myGameObjects)
        {
            if(obj.id == id) return obj;
        }
        return null;
    }
    public MyGameObject FindObjectWithUID(ulong uid)
    {
        foreach(var obj in myScene.GetObjects())
        {
            if (obj.uid == uid) return obj;
        }
        return null;
    }

    public event Action<bool> onPlayModeToggle;
    MySceneSave sceneSave;
    public bool playMode { get; private set; } = false;
    CancellationTokenSource playCts;
    public CancellationToken playToken => playCts != null ? playCts.Token : CancellationToken.None;
    public void EnterPlayMode()
    {
        if (playMode) return;
        playCts?.Dispose();
        playCts = new CancellationTokenSource();
        sceneSave = myScene.Save();
        playMode = true;
        foreach (var i in myScene.GetObjects())
        {
            i.OnAwake();
        }
        foreach (var i in myScene.GetObjects())
        {
            i.OnStart();
        }
        onPlayModeToggle?.Invoke(true);
    }
    public void ExitPlayMode()
    {
        if (!playMode) return;
        playMode = false;
        playCts?.Cancel();
        ulong selectedUID = selected is MyGameObject myGameObject ? myGameObject.uid : 0;
        myScene.Load(sceneSave);
        if (selectedUID != 0) Select(FindObjectWithUID(selectedUID));
        onPlayModeToggle?.Invoke(false);
    }
    public void SaveProject()
    {
        if (GlobalManager.Instance == null) return;
        GlobalManager.Instance.SaveProject(Save());
        AddLog(new(MyLogType.Info, $"Project '{projectName}' saved."));
    }
    public void ProjectToTitle()
    {
        if (GlobalManager.Instance != null) GlobalManager.Instance.ReturnToTitle();
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
        Dictionary<MyAsset, MyAssetSave> saves = new();
        foreach(var assetSave in save.assets)
        {
            MyAsset added = MyAsset.TypeToAsset(assetSave.type);
            if (added == null)
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
        myScene.Load(save.scene);
    }
}
public enum MyLogType
{
    Info = 0,
    Warning = 1,
    Error = 2
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
public class ProjectSave
{
    public string projectName;
    public MySceneSave scene = new();
    public List<MyAssetSave> assets = new();
}
public enum CopyBufferItemType
{
    None,
    CodeBlock,
    MyGameObject,
    MyAsset
}