using System.Collections.Generic;
using UnityEngine;

public class SceneAsset : MyAsset
{
    public override AssetType type => AssetType.Scene;
    public override Sprite assetImage => EditorSceneManager.Instance.assetsSettings.sceneAssetIcon;

    public MySceneSave sceneSave = new();

    public override IEnumerable<ExposedElement> GetElements()
    {
        foreach (var i in base.GetElements()) yield return i;
        yield return new ExposedButton(
            "Open Scene",
            () => EditorSceneManager.Instance.OpenSceneAsset(this));
    }
    public override MyAssetSave Save()
    {
        var save = base.Save();
        // If this scene is being edited right now, capture the live state.
        if (EditorSceneManager.Instance.openSceneAsset == this)
            sceneSave = EditorSceneManager.Instance.myScene.Save();
        save.data.strings["scene"] = SaveSerializer.Serialize(sceneSave);
        return save;
    }
    public override void EarlyLoad(MyAssetSave save, bool resetUID = false)
    {
        base.EarlyLoad(save, resetUID);
        if (save.data.strings.TryGetValue("scene", out string sceneJson))
            sceneSave = SaveSerializer.Deserialize<MySceneSave>(sceneJson) ?? new();
    }
    public override void OnRemove()
    {
        base.OnRemove();
        if(EditorSceneManager.Instance.openSceneAsset == this)
        {

        }
    }
}
