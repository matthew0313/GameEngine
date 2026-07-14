using System.Collections.Generic;
using UnityEngine;

public class CodeBlock_InstantiatedObject : ObjectCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Other;
    [SerializeField] RectTransform rectTransform;
    public CodeBlock_Instantiate target { get; private set; }
    public override bool IsAddable(ICodeable codeable) => false;
    public override MyGameObject GetObject(ulong hash) => target != null ? target.GetInstantiatedObject(hash) : null;
    public override float GetWidth() => rectTransform.rect.width;
    public void BindTarget(CodeBlock_Instantiate target)
    {
        this.target = target;
    }
    public override CodeBlockSave Save()
    {
        var save = base.Save();
        save.data.ulongs["target"] = target != null ? target.uid : 0;
        return save;
    }
    public override void Load(CodeBlockSave save)
    {
        base.Load(save);
        if (save.data.ulongs.TryGetValue("target", out ulong targetUID))
        {
            var targetBlock = owner.codeBlocks.Find(block => block.uid == targetUID);
            if (targetBlock is CodeBlock_Instantiate instantiateBlock)
            {
                target = instantiateBlock;
            }
        }
    }
    protected override void Update()
    {
        base.Update();
        if (target == null) Delete();
    }
    protected override IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        foreach (var i in base.MakeRightClickMenu()) yield return i;
        yield return new RCMenuElement_Button(
            "Move to Origin",
            ctx =>
            {
                if (target == null) return;
                EditorSceneManager.Instance.scriptGrid.MoveTo(target.transform.position, true);
            });
    }
}
