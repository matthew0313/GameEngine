using System.IO;
using UnityEngine;

public class PrefabAsset : MyAsset
{
    public override AssetType type => AssetType.Prefab;
    public void Set(MyGameObject gameObject)
    {
        if (gameObject != null)
        {
            gameObject.isPrefab = true;
        }
        else
        {

        }
    }
}