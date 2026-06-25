using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CodeBlock_FunctionParam : PropertyCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Starter;
    public override PropertyType propertyType => PropertyType.Wildcard;
    [SerializeField] RectTransform rectTransform;
    [SerializeField] TMP_Text parameterNameText;
    [SerializeField] TMP_InputField parameterRenameField;
    public CodeBlock_Function target { get; private set; }
    public string parameterName { get; private set; }
    public override bool IsAddable(ICodeable codeable) => false;
    public override float GetWidth() => rectTransform.rect.width;
    public void BindTarget(CodeBlock_Function target, string parameterName)
    {
        this.target = target;
        this.parameterName = parameterName;
        parameterNameText.text = parameterName;
        target.onParameterUpdate += OnParameterUpdate;
        target.onParameterRename += OnParameterRename;
    }
    private void OnEnable()
    {
        parameterRenameField.onEndEdit.AddListener(OnEndEdit);
    }
    private void OnDisable()
    {
        parameterRenameField.onEndEdit.RemoveListener(OnEndEdit);
    }
    void OnEndEdit(string val)
    {
        if (target == null) return;
        target.RenameParameter(parameterName, val);
        parameterRenameField.gameObject.SetActive(false);
    }
    private void OnParameterUpdate()
    {
        if (target == null || !target.parameters.Contains(parameterName)) Delete();
    }
    private void OnParameterRename(string prev, string newName)
    {
        if (parameterName == prev)
        {
            parameterName = newName;
            parameterNameText.text = newName;
            parameterRenameField.text = newName;
        }
    }

    public override float GetNumber(ulong hash) => target.GetParameter(hash, parameterName).number;
    public override bool GetCondition(ulong hash) => target.GetParameter(hash, parameterName).condition;
    public override string GetString(ulong hash) => target.GetParameter(hash, parameterName).str;
    public override MyGameObject GetObject(ulong hash) => target.GetParameter(hash, parameterName).obj;
    public override MyAsset GetAsset(ulong hash) => target.GetParameter(hash, parameterName).asset;
    public override Vector2 GetVector2(ulong hash) => target.GetParameter(hash, parameterName).vector2;
    public override Color GetColor(ulong hash) => target.GetParameter(hash, parameterName).color;

    public override CodeBlockSave Save()
    {
        var save = base.Save();
        save.data.ulongs["target"] = target != null ? target.uid : 0;
        save.data.strings["parameterName"] = parameterName;
        return save;
    }
    public override void Load(CodeBlockSave save)
    {
        base.Load(save);
        if (save.data.ulongs.TryGetValue("target", out ulong targetUID))
        {
            var targetBlock = owner.codeBlocks.Find(block => block.uid == targetUID);
            if (targetBlock is CodeBlock_Function forBlock)
            {
                target = forBlock;
            }
        }
        if (save.data.strings.TryGetValue("parameterName", out string parameterName))
        {
            this.parameterName = parameterName;
            parameterNameText.text = parameterName;
        }
    }
    protected override void Update()
    {
        base.Update();
        if (target == null) Delete();
    }
    public override void Delete()
    {
        if(target != null)
        {
            target.onParameterUpdate -= OnParameterUpdate;
            target.onParameterRename -= OnParameterRename;
        }
        base.Delete();
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
        yield return new RCMenuElement_Button(
            "Rename Parameter",
            ctx =>
            {
                if (target == null) return;
                parameterRenameField.gameObject.SetActive(true);
                parameterRenameField.Select();
            });
        yield return new RCMenuElement_Button(
            "Remove Parameter",
            ctx =>
            {
                if (target == null) return;
                target.RemoveParameter(parameterName);
            });
    }
}