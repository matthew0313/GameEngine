using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ScriptGridBlockMenuElement : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] Button button;
    ScriptGridBlockMenu menu;
    CodeBlock target;
    private void OnEnable()
    {
        button.onClick.AddListener(OnClick);
    }
    private void OnDisable()
    {
        button.onClick.RemoveListener(OnClick);
    }
    public void Init(ScriptGridBlockMenu menu)
    {
        this.menu = menu;
    }
    public void Set(CodeBlock codeBlock)
    {
        target = codeBlock;
        text.text = codeBlock.blockID;
        text.color = codeBlock.blockColor;
    }
    void OnClick() => menu.AddBlock(target);
}