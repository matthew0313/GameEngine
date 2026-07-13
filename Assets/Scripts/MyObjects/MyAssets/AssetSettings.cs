using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AssetsSettings", menuName = "Scriptables/AssetsSettings")]
public class AssetsSettings : ScriptableObject
{
    [field: SerializeField] public Sprite prefabAssetIcon { get; private set; }
    [field: SerializeField] public Sprite sceneAssetIcon { get; private set; }
}
