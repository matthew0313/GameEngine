using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class FileAsset : MyAsset
{
    public string filePath { get; protected set; }
    public abstract void LoadFile(string filePath);
    public override IEnumerable<ExposedElement> GetElements()
    {
        foreach(var i in base.GetElements()) yield return i;
        yield return new ExposedString(
            "FilePath",
            () => filePath,
            (value) => LoadFile(value));
    }
    public override MyAssetSave Save()
    {
        var save = base.Save();
        save.data.strings["filePath"] = filePath;
        return save;
    }
    public override void Load(MyAssetSave save)
    {
        base.Load(save);
        if (save.data.strings.TryGetValue("filePath", out string path)) LoadFile(path);
    }
}