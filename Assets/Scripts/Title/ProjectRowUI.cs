using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>One project entry in the title screen list. The row lives in the scene as a
/// disabled template that <see cref="TitleUI"/> clones per project.</summary>
public class ProjectRowUI : MonoBehaviour
{
    [SerializeField] TMP_Text nameText;
    [SerializeField] TMP_Text dateText;
    [SerializeField] Button openButton;
    [SerializeField] Button renameButton;
    [SerializeField] Button folderButton;
    [SerializeField] Button deleteButton;

    public void Setup(string projectName, DateTime lastModified,
        Action open, Action rename, Action folder, Action delete)
    {
        nameText.text = projectName;
        dateText.text = "Last modified: " + lastModified.ToString("yyyy-MM-dd HH:mm");
        Bind(openButton, open);
        Bind(renameButton, rename);
        Bind(folderButton, folder);
        Bind(deleteButton, delete);
    }

    static void Bind(Button button, Action action)
    {
        button.onClick.RemoveAllListeners();
        if (action != null) button.onClick.AddListener(() => action());
    }
}
