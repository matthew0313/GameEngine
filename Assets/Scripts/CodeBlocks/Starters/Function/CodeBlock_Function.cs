using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class CodeBlock_Function : CodeBlock, IOnFinish
{
    public override CodeBlockCategory category => CodeBlockCategory.Starter;

    [SerializeField] Transform parameterElementAnchor;
    [SerializeField] SnapPoint_FunctionParam parameterElementPrefab;
    readonly List<SnapPoint_FunctionParam> parameterElements = new();

    [SerializeField] TMP_InputField functionNameField;
    [SerializeField] ExecutableSnapPoint onExecute;
    public ExecutableSnapPoint onFinish => onExecute;
    public readonly List<string> parameters = new();
    public event Action onParameterUpdate;
    public event Action<string, string> onParameterRename;
    public readonly Dictionary<ulong, Dictionary<string, Wildcard>> executionParams = new();
    ulong hash = 0;
    public string functionName => functionNameField.text;
    public override bool IsAddable(ICodeable codeable)
    {
        return base.IsAddable(codeable) && (codeable is MyGameObject || codeable is PrefabAsset);
    }
    public override void Set(ICodeable owner)
    {
        base.Set(owner);
        if (owner is MyGameObject obj) obj.functions.Add(this);
    }
    public void AddParameter()
    {
        int index = 1;
        while (parameters.Contains($"param{index}")) index++;
        parameters.Add($"param{index}");
        OnParameterUpdate();
    }
    public void RemoveParameter(string paramName)
    {
        if (!parameters.Contains(paramName)) return;
        parameters.Remove(paramName);
        OnParameterUpdate();
    }
    public void RenameParameter(string paramName, string newName)
    {
        if (!parameters.Contains(paramName)) return;
        int index = parameters.IndexOf(paramName);
        parameters[index] = newName;
        onParameterRename?.Invoke(paramName, newName);
        OnParameterUpdate();
    }
    public void SwapParameter(string param1, string param2)
    {
        if(!parameters.Contains(param1) || !parameters.Contains(param2)) return;
        int index1 = parameters.IndexOf(param1);
        int index2 = parameters.IndexOf(param2);
        parameters[index1] = param2;
        parameters[index2] = param1;
        OnParameterUpdate();
    }
    void OnParameterUpdate()
    {
        int i = 0;
        for(; i < parameters.Count; i++)
        {
            if (parameterElements.Count <= i) parameterElements.Add(Instantiate(parameterElementPrefab, parameterElementAnchor));
            parameterElements[i].gameObject.SetActive(true);
            parameterElements[i].parameterName = parameters[i];
            parameterElements[i].Clear();
        }
        for(; i < parameterElements.Count; i++)
        {
            parameterElements[i].gameObject.SetActive(false);
        }
        onParameterUpdate?.Invoke();
    }
    public async UniTask Execute(Dictionary<string, Wildcard> parameters, CancellationToken token)
    {
        executionParams[hash] = parameters;
        var info = await onExecute.Execute(hash, token);
        executionParams.Remove(hash);
        hash++;
    }
    public Wildcard GetParameter(ulong hash, string name)
    {
        if (!executionParams.ContainsKey(hash)) return new();
        if (!executionParams[hash].ContainsKey(name)) return new();
        return executionParams[hash][name];
    }
    protected override IEnumerable<SnapPoint> GetSnapPoints()
    {
        yield return onExecute;
    }
    public override CodeBlockSave Save()
    {
        var save = base.Save();
        save.data.strings["parameters"] = SaveSerializer.Serialize(new ParametersWrapper() { parameters = parameters });
        save.data.strings["functionName"] = functionName;
        return save;
    }
    public override void Load(CodeBlockSave save)
    {
        base.Load(save);
        parameters.Clear();
        if (save.data.strings.TryGetValue("parameters", out string parametersJson))
            foreach (var i in SaveSerializer.Deserialize<ParametersWrapper>(parametersJson).parameters) parameters.Add(i);
        if (save.data.strings.TryGetValue("functionName", out string fnName)) functionNameField.text = fnName;
        OnParameterUpdate();
    }
    public override void Delete()
    {
        foreach (var i in parameterElements) i.enabled = false;
        if (owner is MyGameObject obj) obj.functions.Remove(this);
        base.Delete();
    }

    ulong testHash = 0;
    protected override IEnumerable<RCMenuElement> MakeRightClickMenu()
    {
        yield return new RCMenuElement_Button(
            "Add Parameter",
            ctx =>
            {
                AddParameter();
            });
        yield return new RCMenuElement_Button(
            "Execute with default params",
            ctx =>
            {
                onExecute.Execute(testHash++, CancellationToken.None);
            });
        foreach (var i in base.MakeRightClickMenu()) yield return i;
    }
    [System.Serializable]
    struct ParametersWrapper
    {
        public List<string> parameters;
    }
}