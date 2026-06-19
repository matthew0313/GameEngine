using TMPro;
using UnityEngine;

public class CodeBlock_SetBodyType : RigidbodyExecutableCodeBlock
{
    [SerializeField] TMP_Dropdown bodyType; // 0 = Dynamic, 1 = Kinematic, 2 = Static
    protected override void Apply(MyGameObject_Rigidbody rb, ulong hash)
    {
        rb.rb.bodyType = (RigidbodyType2D)bodyType.value;
    }
    public override CodeBlockSave Save()
    {
        var save = base.Save();
        save.data.integers["bodyType"] = bodyType.value;
        return save;
    }
    public override void Load(CodeBlockSave save)
    {
        base.Load(save);
        if (save.data.integers.TryGetValue("bodyType", out int bodyTypeValue)) bodyType.value = bodyTypeValue;
    }
}
