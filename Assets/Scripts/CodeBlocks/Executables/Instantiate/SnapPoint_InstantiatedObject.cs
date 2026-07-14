using UnityEngine;

public class SnapPoint_InstantiatedObject : SnapPoint
{
    [SerializeField] CodeBlock_InstantiatedObject blockPrefab;
    public override bool IsSnappable(CodeBlock codeBlock) => codeBlock is CodeBlock_InstantiatedObject block && block.target == ownerBlock;
    public override void Snap(CodeBlock codeBlock)
    {
        codeBlock.Delete();
    }
    protected override void OnSnappedChange()
    {
        base.OnSnappedChange();
        if (!enabled) return;
        if (snapped == null)
        {
            var tmp = Instantiate(blockPrefab);
            tmp.BindTarget(ownerBlock as CodeBlock_Instantiate);
            tmp.Set(ownerBlock.owner);
            ownerBlock.owner.codeBlocks.Add(tmp);
            base.Snap(tmp);
        }
    }
}
