using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PrefabAsset : MyAsset, ICodeable
{
    public MyGameObject prefabOrigin { get; private set; }
    public override AssetType type => AssetType.Prefab;
    public float lastZoom { get => prefabOrigin.lastZoom; set => prefabOrigin.lastZoom = value; }
    public Vector2 lastOffset { get => prefabOrigin.lastOffset; set => prefabOrigin.lastOffset = value; }
    public List<CodeBlock> codeBlocks => prefabOrigin.codeBlocks;

    public void Set(MyGameObject gameObject)
    {
        name = gameObject.name;
        prefabOrigin = gameObject;
        gameObject.gameObject.SetActive(false);
        if (prefabOrigin.parent != null) prefabOrigin.parent.RemoveChild(prefabOrigin);
    }
    public override MyAssetSave Save()
    {
        var save = base.Save();
        save.data.strings["prefabOrigin"] = JsonUtility.ToJson(prefabOrigin.Save());
        return save;
    }
    MyGameObjectSave prefabOriginSave;
    public override void EarlyLoad(MyAssetSave save, bool resetUID = false)
    {
        base.EarlyLoad(save, resetUID);
        prefabOriginSave = JsonUtility.FromJson<MyGameObjectSave>(save.data.strings["prefabOrigin"]);
        prefabOrigin = MonoBehaviour.Instantiate(EditorSceneManager.Instance.TypeToObjectPrefab(prefabOriginSave.type));
        prefabOrigin.EarlyLoad(prefabOriginSave, resetUID);
    }
    public override void Load(MyAssetSave save)
    {
        base.Load(save);
        prefabOrigin.Load(prefabOriginSave);
    }
    public MyGameObject Instantiate()
    {
        MyGameObjectSave save = prefabOrigin.Save();
        MyGameObject obj = MonoBehaviour.Instantiate(prefabOrigin);
        obj.Load(save);
        obj.gameObject.SetActive(true);
        return obj;
    }
}