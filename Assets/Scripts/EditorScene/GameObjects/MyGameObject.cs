using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class MyGameObject : MonoBehaviour, ICodeable
{
    [HideInInspector] public bool dirty = false;
    public abstract string id { get; }
    public List<CodeBlock> codeBlocks { get; } = new();

    [SerializeField] List<CodeBlockList> availableCodeBlockLists;
    [SerializeField] List<CodeBlock> availableCodeBlocks;
    public virtual IEnumerable<CodeBlock> GetAvailableBlocks()
    {
        foreach(var list in availableCodeBlockLists)
        {
            foreach(var block in list.codeBlocks)
            {
                yield return block;
            }
        }
        foreach(var block in availableCodeBlocks)
        {
            yield return block;
        }
    }
    public readonly Dictionary<string, float> numericVariables = new();
    public virtual MyGameObjectSave Save(bool prettyPrint = true)
    {
        MyGameObjectSave save = new();
        save.id = id;
        List<CodeBlockSave> codeBlockSaves = new();
        foreach(var block in codeBlocks)
        {
            if(block.snappedPoint == null)
            {
                var tmp = block.Save();
                tmp.position = block.transform.position;
                codeBlockSaves.Add(tmp);
            }
        }
        save.data.strings["CodeBlocks"] = JsonUtility.ToJson(codeBlockSaves, prettyPrint);
        return save;
    }
    public virtual void Load(MyGameObjectSave save)
    {
        codeBlocks.Clear();
        if (save.data.strings.ContainsKey("CodeBlocks"))
        {
            List<CodeBlockSave> codeBlockSaves = JsonUtility.FromJson<List<CodeBlockSave>>(save.data.strings["CodeBlocks"]);
            foreach (var blockSave in codeBlockSaves)
            {
                CodeBlock blockPrefab = EditorSceneManager.Instance.IDToBlock(blockSave.id);
                if (blockPrefab != null)
                {
                    CodeBlock block = Instantiate(blockPrefab, transform);
                    block.Load(blockSave);
                    block.transform.position = blockSave.position;
                    block.gameObject.SetActive(false);
                    codeBlocks.Add(block);
                }
            }
        }
    }
[System.Serializable]
public class MyGameObjectSave
{
    public string id;
    public DataUnit data = new();
}