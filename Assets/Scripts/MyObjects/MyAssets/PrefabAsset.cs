using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PrefabAsset : MyAsset, ICodeable
{
    public MyGameObject prefabOrigin { get; private set; }
    public override AssetType type => AssetType.Prefab;

    public Vector2 lastOffset { get => prefabOrigin.lastOffset; set => prefabOrigin.lastOffset = value; }
    public List<CodeBlock> codeBlocks => prefabOrigin.codeBlocks;
    public IEnumerable<CodeBlock> GetAvailableBlocks() => prefabOrigin.GetAvailableBlocks();

    public void Set(MyGameObject gameObject)
    {
        if (prefabOrigin == gameObject) return;
        if (prefabOrigin != null) MonoBehaviour.Destroy(prefabOrigin);
        prefabOrigin = gameObject;
        if (prefabOrigin != null && EditorSceneManager.Instance.myScene.ContainsObject(prefabOrigin))
        {
            if(prefabOrigin.parent != null) prefabOrigin.parent.RemoveChild(prefabOrigin);
            else EditorSceneManager.Instance.myScene.RemoveTopObject(prefabOrigin);
        }
    }
}