using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "CodeBlockList", menuName = "Scriptables/CodeBlockList")]
public class CodeBlockList : ScriptableObject
{
    public List<CodeBlockListElement> elements;
    public IEnumerable<CodeBlock> GetBlocks()
    {
        foreach (var i in elements)
        {
            if (i.type == CodeBlockListElementType.Item)
            {
                yield return i.item;
            }
            else if (i.type == CodeBlockListElementType.List)
            {
                foreach (var j in i.list.GetBlocks())
                {
                    yield return j;
                }
            }
        }
    }
    public CodeBlock IDToBlock(string id)
    {
        foreach (var block in GetBlocks())
        {
            if (block.blockID == id) return block;
        }
        return null;
    }
}
[System.Serializable]
public struct CodeBlockListElement
{
    public CodeBlockListElementType type;
    public CodeBlock item;
    public CodeBlockList list;
}
[System.Serializable]
public enum CodeBlockListElementType
{
    Item = 0,
    List = 1
}
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(CodeBlockListElement))]
public class CodeBlockListElement_Drawer : PropertyDrawer
{
    const int typeWidth = 80;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField(new Rect(position) { width = typeWidth }, property.FindPropertyRelative("type"), GUIContent.none);

        if (property.FindPropertyRelative("type").enumValueIndex == 0)
        {
            EditorGUI.PropertyField(new Rect(position) { x = position.x + typeWidth + 3, width = position.width - typeWidth - 3 }, property.FindPropertyRelative("item"), GUIContent.none);
        }
        else if (property.FindPropertyRelative("type").enumValueIndex == 1)
        {
            EditorGUI.PropertyField(new Rect(position) { x = position.x + typeWidth + 3, width = position.width - typeWidth - 3 }, property.FindPropertyRelative("list"), GUIContent.none);
        }
    }
}
#endif