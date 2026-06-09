using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodeBlock_KeyDown : ConditionCodeBlock
{
    public override CodeBlockCategory category => CodeBlockCategory.Condition;

    [SerializeField] RectTransform rectTransform;
    [SerializeField] TMP_InputField key;
    KeyCode setKey = KeyCode.None;
    private void OnEnable()
    {
        key.onEndEdit.AddListener(OnKeySet);
    }
    private void OnDisable()
    {
        key.onEndEdit.RemoveListener(OnKeySet);
    }
    void OnKeySet(string str)
    {
        if (char.TryParse(str, out char c) && InputManager.Instance.CharToKeycode(c, out KeyCode keyCode))
        {
            setKey = keyCode;
        }
        else key.text = "";
    }
    public override bool GetCondition(ulong hash)
    {
        return InputManager.GetKeyDown(setKey);
    }
    public override float GetWidth() => rectTransform.rect.width;
    public override CodeBlockSave Save()
    {
        var save = base.Save();
        save.data.integers["setKey"] = (int)setKey;
        return save;
    }
    public override void Load(CodeBlockSave save)
    {
        base.Load(save);
        setKey = (KeyCode)save.data.integers["setKey"];
        key.text = InputManager.Instance.KeycodeToString(setKey);
    }
}