using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Persistent, cross-scene singleton. Owns on-disk project persistence and the
/// hand-off of the "current project" between the title scene and the editor scene.
/// Initialized BeforeSceneLoad, mirroring <see cref="DeviceManager"/>.
/// </summary>
public class GlobalManager : MonoBehaviour
{
    public static GlobalManager Instance { get; private set; }

    [Header("Storage (relative to project root)")]
    [SerializeField] string _saveRoot = "Saves";
    [SerializeField] string _saveFileName = "Save.json";
    [SerializeField] string _assetsFolderName = "Assets";

    [Header("Scenes")]
    [SerializeField] string _titleSceneName = "TitleScene";
    [SerializeField] string _editorSceneName = "EditorScene";

    public string saveRoot => _saveRoot;
    public string saveFileName => _saveFileName;
    public string assetsFolderName => _assetsFolderName;
    public string titleSceneName => _titleSceneName;
    public string editorSceneName => _editorSceneName;

    /// <summary>Project chosen in the title scene, read by the editor scene on load.</summary>
    public string currentProjectName { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        Instance = Instantiate(Resources.Load<GlobalManager>(nameof(GlobalManager)));
        DontDestroyOnLoad(Instance);
    }

    // ---- Path helpers (relative to the project/working-directory root) ----
    public string ProjectFolder(string name) => Path.Combine(saveRoot, name);
    public string ProjectSaveFile(string name) => Path.Combine(ProjectFolder(name), saveFileName);
    public string ProjectAssetsFolder(string name) => Path.Combine(ProjectFolder(name), assetsFolderName);
    public bool ProjectExists(string name) => !string.IsNullOrEmpty(name) && Directory.Exists(ProjectFolder(name));

    // ---- Listing ----
    public List<ProjectInfo> GetProjects()
    {
        var list = new List<ProjectInfo>();
        if (!Directory.Exists(saveRoot)) Directory.CreateDirectory(saveRoot);
        foreach (var dir in Directory.GetDirectories(saveRoot))
        {
            string name = Path.GetFileName(dir);
            string saveFile = Path.Combine(dir, saveFileName);
            DateTime modified = File.Exists(saveFile)
                ? File.GetLastWriteTime(saveFile)
                : Directory.GetLastWriteTime(dir);
            list.Add(new ProjectInfo { name = name, path = dir, lastModified = modified });
        }
        list.Sort((a, b) => b.lastModified.CompareTo(a.lastModified));
        return list;
    }

    // ---- Validation ----
    public bool IsValidProjectName(string name, out string error)
    {
        error = null;
        if (string.IsNullOrWhiteSpace(name)) { error = "Name cannot be empty."; return false; }
        name = name.Trim();
        if (name.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0) { error = "Name contains invalid characters."; return false; }
        if (ProjectExists(name)) { error = "A project with that name already exists."; return false; }
        return true;
    }

    // ---- Create / Delete / Rename ----
    public ProjectInfo CreateProject(string name)
    {
        name = name.Trim();
        Directory.CreateDirectory(ProjectFolder(name));
        Directory.CreateDirectory(ProjectAssetsFolder(name));
        SaveProjectToFile(new ProjectSave { projectName = name });
        return new ProjectInfo { name = name, path = ProjectFolder(name), lastModified = DateTime.Now };
    }

    public void DeleteProject(string name)
    {
        var folder = ProjectFolder(name);
        if (Directory.Exists(folder)) Directory.Delete(folder, true);
    }

    public bool RenameProject(string oldName, string newName, out string error)
    {
        error = null;
        newName = (newName ?? string.Empty).Trim();
        if (!IsValidProjectName(newName, out error)) return false;
        if (!ProjectExists(oldName)) { error = "Original project no longer exists."; return false; }
        try
        {
            Directory.Move(ProjectFolder(oldName), ProjectFolder(newName));
            var save = LoadProject(newName);
            save.projectName = newName;
            SaveProjectToFile(save);
            return true;
        }
        catch (Exception e) { error = e.Message; return false; }
    }

    public void ShowInExplorer(string name)
    {
        if (!ProjectExists(name)) return;
        string full = Path.GetFullPath(ProjectFolder(name));
        try
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            System.Diagnostics.Process.Start("explorer.exe", full.Replace('/', '\\'));
#else
            Application.OpenURL("file://" + full);
#endif
        }
        catch (Exception e) { Debug.LogWarning($"Could not open explorer: {e.Message}"); }
    }

    // ---- Load / Save ----
    public ProjectSave LoadProject(string name)
    {
        var save = LoadProjectFromFile(ProjectSaveFile(name));
        return save ?? new ProjectSave { projectName = name };
    }

    ProjectSave LoadProjectFromFile(string file)
    {
        if (!File.Exists(file)) return null;
        string json = File.ReadAllText(file);
        if (string.IsNullOrWhiteSpace(json)) return null;
        try { return SaveSerializer.Deserialize<ProjectSave>(json); }
        catch (Exception e) { Debug.LogWarning($"Failed to parse '{file}': {e.Message}"); return null; }
    }

    public void SaveProject(ProjectSave save) => SaveProjectToFile(save);

    void SaveProjectToFile(ProjectSave save)
    {
        if (save == null || string.IsNullOrEmpty(save.projectName)) return;
        Directory.CreateDirectory(ProjectFolder(save.projectName));
        Directory.CreateDirectory(ProjectAssetsFolder(save.projectName));
        File.WriteAllText(ProjectSaveFile(save.projectName), SaveSerializer.Serialize(save, true));
    }

    // ---- Scene transitions ----
    public void OpenProject(string name)
    {
        if (!ProjectExists(name)) return;
        currentProjectName = name;
        SceneManager.LoadScene(editorSceneName);
    }

    public void ReturnToTitle()
    {
        currentProjectName = null;
        SceneManager.LoadScene(titleSceneName);
    }
}

/// <summary>Lightweight runtime descriptor for a project on disk (not serialized to JSON).</summary>
public class ProjectInfo
{
    public string name;
    public string path;
    public DateTime lastModified;
}
