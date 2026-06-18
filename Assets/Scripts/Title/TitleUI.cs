using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Unity Hub-like title screen. Lists projects on disk (via <see cref="GlobalManager"/>) and lets
/// the user create, open, rename, delete, or reveal a project. All UI is authored in the scene;
/// this controller only populates the list (cloning <see cref="rowTemplate"/>) and drives dialogs.
/// </summary>
public class TitleUI : MonoBehaviour
{
    [Header("List")]
    [SerializeField] Transform content;
    [SerializeField] ProjectRowUI rowTemplate;
    [SerializeField] GameObject emptyState;

    [Header("Header")]
    [SerializeField] Button newProjectButton;

    [Header("Name Dialog")]
    [SerializeField] GameObject nameDialog;
    [SerializeField] TMP_Text nameDialogTitle;
    [SerializeField] TMP_InputField nameDialogInput;
    [SerializeField] TMP_Text nameDialogError;
    [SerializeField] TMP_Text nameDialogConfirmLabel;
    [SerializeField] Button nameDialogConfirm;
    [SerializeField] Button nameDialogCancel;

    [Header("Confirm Dialog")]
    [SerializeField] GameObject confirmDialog;
    [SerializeField] TMP_Text confirmDialogMessage;
    [SerializeField] TMP_Text confirmDialogConfirmLabel;
    [SerializeField] Button confirmDialogConfirm;
    [SerializeField] Button confirmDialogCancel;

    Func<string, string> onNameConfirm;
    Action onConfirm;

    void Start()
    {
        rowTemplate.gameObject.SetActive(false);
        nameDialog.SetActive(false);
        confirmDialog.SetActive(false);

        newProjectButton.onClick.AddListener(ShowCreateDialog);
        nameDialogCancel.onClick.AddListener(() => nameDialog.SetActive(false));
        nameDialogConfirm.onClick.AddListener(OnNameConfirmClicked);
        confirmDialogCancel.onClick.AddListener(() => confirmDialog.SetActive(false));
        confirmDialogConfirm.onClick.AddListener(OnConfirmClicked);

        Refresh();
    }

    public void Refresh()
    {
        foreach (Transform child in content)
            if (child != rowTemplate.transform) Destroy(child.gameObject);

        var projects = GlobalManager.Instance != null
            ? GlobalManager.Instance.GetProjects()
            : new System.Collections.Generic.List<ProjectInfo>();

        emptyState.SetActive(projects.Count == 0);
        foreach (var info in projects)
        {
            var row = Instantiate(rowTemplate, content);
            row.gameObject.SetActive(true);
            row.Setup(info.name, info.lastModified,
                open: () => GlobalManager.Instance.OpenProject(info.name),
                rename: () => ShowRenameDialog(info.name),
                folder: () => GlobalManager.Instance.ShowInExplorer(info.name),
                delete: () => ShowDeleteDialog(info.name));
        }
        rowTemplate.transform.SetAsLastSibling();
    }

    // ---- dialog entry points ----
    void ShowCreateDialog() => ShowNameDialog("Create New Project", "", "Create", name =>
    {
        if (!GlobalManager.Instance.IsValidProjectName(name, out string error)) return error;
        var info = GlobalManager.Instance.CreateProject(name);
        GlobalManager.Instance.OpenProject(info.name); // open immediately, like Unity Hub
        return null;
    });

    void ShowRenameDialog(string oldName) => ShowNameDialog($"Rename \"{oldName}\"", oldName, "Rename", newName =>
    {
        if (newName != null && newName.Trim() == oldName) return null; // no change
        if (!GlobalManager.Instance.RenameProject(oldName, newName, out string error)) return error;
        return null;
    });

    void ShowDeleteDialog(string name) => ShowConfirmDialog(
        $"Delete project \"{name}\"?\nThis permanently removes its files and cannot be undone.",
        "Delete", () => GlobalManager.Instance.DeleteProject(name));

    // ---- name dialog ----
    void ShowNameDialog(string title, string initial, string confirmLabel, Func<string, string> handler)
    {
        onNameConfirm = handler;
        nameDialogTitle.text = title;
        nameDialogInput.text = initial ?? "";
        nameDialogError.text = "";
        nameDialogConfirmLabel.text = confirmLabel;
        nameDialog.SetActive(true);
        nameDialogInput.Select();
    }

    void OnNameConfirmClicked()
    {
        string error = onNameConfirm?.Invoke(nameDialogInput.text);
        if (string.IsNullOrEmpty(error)) { nameDialog.SetActive(false); Refresh(); }
        else nameDialogError.text = error;
    }

    // ---- confirm dialog ----
    void ShowConfirmDialog(string message, string confirmLabel, Action handler)
    {
        onConfirm = handler;
        confirmDialogMessage.text = message;
        confirmDialogConfirmLabel.text = confirmLabel;
        confirmDialog.SetActive(true);
    }

    void OnConfirmClicked()
    {
        confirmDialog.SetActive(false);
        onConfirm?.Invoke();
        Refresh();
    }
}
