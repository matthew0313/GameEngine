using System;
using UnityEngine;

/// <summary>
/// Facade that exposes GameEngine authoring operations to an external MCP server.
///
/// Design notes (up for review):
/// - Static and stateless on purpose. Everything reaches through EditorSceneManager.Instance
///   rather than introducing a second singleton.
/// - MAIN THREAD ONLY. Whatever transport calls into this (socket thread, relay) must marshal
///   through a queue drained in Update(). Calling these off the main thread will throw.
/// - Addressing: objects and blocks are addressed by uid. Snap points have no identity of their
///   own, so they are addressed as (ownerBlockUid, index) where index is the ordinal position in
///   CodeBlock.GetSnapPoints() -- the same ordering Save/EarlyLoad/Load already rely on.
/// - Every method returns McpResult so failures cross the boundary as actionable messages
///   instead of exceptions.
/// </summary>
public static class MCPManager
{
    #region Discovery
    // These come first deliberately: without them an external caller has to guess blockIDs
    // out of 250+ prefabs, and guesses will be wrong.

    /// <summary>Block types that can currently be added to the given owner, honouring CodeBlock.IsAddable.
    /// Returns blockID, category and display colour for each.</summary>
    public static McpResult ListBlockTypes(ulong ownerUid) => throw new NotImplementedException();

    /// <summary>Schema for a single block type: its snap point count, what each snap point accepts,
    /// and the DataUnit keys the block reads in Load. This is the piece that lets a caller set
    /// "setKey" or "value" without reverse-engineering a save file.</summary>
    public static McpResult DescribeBlockType(string blockID) => throw new NotImplementedException();

    /// <summary>Object types available from MyGameObjectList (Rigidbody, Sprite, BoxCollider, Empty, ...).</summary>
    public static McpResult ListObjectTypes() => throw new NotImplementedException();
    #endregion

    #region Scene read

    /// <summary>Full hierarchy of the open scene or prefab: uid, id, name, transform, child nesting.
    /// Does not include block programs -- use GetProgram for those.</summary>
    public static McpResult GetSceneTree() => throw new NotImplementedException();

    /// <summary>Single object with its inspector data expanded.</summary>
    public static McpResult GetObject(ulong objectUid) => throw new NotImplementedException();
    #endregion

    #region Object mutation

    /// <summary>Creates a MyGameObject of the given type id. parentUid of 0 means scene root
    /// (or prefab root when in prefab mode).</summary>
    public static McpResult CreateObject(string typeId, string name, ulong parentUid = 0) => throw new NotImplementedException();

    public static McpResult DeleteObject(ulong objectUid) => throw new NotImplementedException();

    public static McpResult RenameObject(ulong objectUid, string name) => throw new NotImplementedException();

    /// <summary>Null components are left unchanged.</summary>
    public static McpResult SetObjectTransform(ulong objectUid, Vector2? position, float? rotation, Vector2? scale) => throw new NotImplementedException();

    /// <summary>Patches the object's inspectorData. Only keys present in the patch are written.</summary>
    public static McpResult SetObjectData(ulong objectUid, DataUnit patch) => throw new NotImplementedException();

    /// <summary>newParentUid of 0 reparents to scene root. Must reject cycles.</summary>
    public static McpResult ReparentObject(ulong objectUid, ulong newParentUid) => throw new NotImplementedException();
    #endregion

    #region Block program read

    /// <summary>The owner's full block program as CodeBlockSave JSON -- the same shape Save() emits,
    /// so it round-trips directly into AuthorProgram.</summary>
    public static McpResult GetProgram(ulong ownerUid) => throw new NotImplementedException();

    /// <summary>Snap points on a block: index, what is currently snapped in (if anything), and
    /// whether a given candidate block would be accepted.</summary>
    public static McpResult ListSnapPoints(ulong blockUid) => throw new NotImplementedException();
    #endregion

    #region Block authoring

    /// <summary>Adds a single unsnapped block to the owner's grid. Mirrors BlockAddMenu.AddBlock
    /// but takes the owner explicitly instead of reading ScriptGrid.editing.</summary>
    public static McpResult AddBlock(ulong ownerUid, string blockID, Vector2 position) => throw new NotImplementedException();

    /// <summary>Builds an entire nested program in one call by feeding CodeBlockSave JSON through the
    /// existing EarlyLoad/Load path. Highest-leverage entry point -- no new snapping logic needed.
    /// When replace is false the authored blocks are added alongside what is already there.
    /// Should always assign fresh uids (resetUID: true) so authored blocks never collide with existing ones.</summary>
    public static McpResult AuthorProgram(ulong ownerUid, string programJson, bool replace = false) => throw new NotImplementedException();

    /// <summary>Snaps an existing loose block into (targetBlockUid, snapPointIndex).
    /// Must reject the snap if SnapPoint.IsSnappable is false rather than silently doing nothing.</summary>
    public static McpResult SnapBlock(ulong blockUid, ulong targetBlockUid, int snapPointIndex) => throw new NotImplementedException();

    /// <summary>Detaches a block from whatever it is snapped into, leaving it loose on the grid.</summary>
    public static McpResult DetachBlock(ulong blockUid) => throw new NotImplementedException();

    /// <summary>Deletes a block and everything snapped beneath it.</summary>
    public static McpResult DeleteBlock(ulong blockUid) => throw new NotImplementedException();

    /// <summary>Patches a block's DataUnit -- literal values, key codes, dropdown selections.
    /// Only keys present in the patch are written.</summary>
    public static McpResult SetBlockData(ulong blockUid, DataUnit patch) => throw new NotImplementedException();

    /// <summary>Reports structural problems without mutating: empty snap points that need filling,
    /// blocks left unsnapped, starters that lead nowhere. Lets a caller check its own work.</summary>
    public static McpResult ValidateProgram(ulong ownerUid) => throw new NotImplementedException();
    #endregion

    #region Assets and prefabs

    /// <summary>Lists project assets. Pass 0 for all types.</summary>
    public static McpResult ListAssets(AssetType filter = 0) => throw new NotImplementedException();

    /// <summary>Creates a PrefabAsset from an existing scene object, via PrefabAsset.Set.</summary>
    public static McpResult CreatePrefabFromObject(ulong objectUid) => throw new NotImplementedException();

    /// <summary>Instantiates a PrefabAsset into the open scene. Wraps PrefabAsset.Instantiate.</summary>
    public static McpResult InstantiatePrefab(ulong prefabAssetUid) => throw new NotImplementedException();

    public static McpResult DeleteAsset(ulong assetUid) => throw new NotImplementedException();

    /// <summary>Switches the editor to a scene asset. Changes what GetSceneTree returns.</summary>
    public static McpResult OpenScene(ulong sceneAssetUid) => throw new NotImplementedException();

    /// <summary>Enters prefab mode for editing a prefab's origin object.</summary>
    public static McpResult OpenPrefab(ulong prefabAssetUid) => throw new NotImplementedException();
    #endregion

    #region Lifecycle and feedback

    public static McpResult SaveProject() => throw new NotImplementedException();

    public static McpResult EnterPlayMode() => throw new NotImplementedException();

    public static McpResult ExitPlayMode() => throw new NotImplementedException();

    /// <summary>Reads EditorSceneManager.logs. This is how an external caller finds out whether the
    /// program it just authored actually runs -- without it, authoring is write-only.</summary>
    public static McpResult GetLogs(int count = 20, MyLogType? filter = null) => throw new NotImplementedException();

    public static McpResult ClearLogs() => throw new NotImplementedException();
    #endregion

    #region Internal helpers
    // Neither of these exists yet. EditorSceneManager has FindObjectWithUID for objects, but there
    // is no equivalent for blocks, and blocks are nested arbitrarily deep through snap points.

    /// <summary>Depth-first search for a block by uid across the owner's whole program.</summary>
    static CodeBlock FindBlock(ulong blockUid) => throw new NotImplementedException();

    /// <summary>Resolves (ownerBlockUid, index) against CodeBlock.GetSnapPoints() ordering.</summary>
    static SnapPoint FindSnapPoint(ulong ownerBlockUid, int snapPointIndex) => throw new NotImplementedException();

    /// <summary>Walks up from a block to the ICodeable that owns it.</summary>
    static ICodeable FindOwner(ulong blockUid) => throw new NotImplementedException();
    #endregion
}

/// <summary>
/// Uniform return shape for every MCPManager call. payload carries JSON when there is something
/// to return; error carries a message written for the caller to act on, not a stack trace.
/// </summary>
public class McpResult
{
    public bool success;
    public string error;
    public string payload;

    public static McpResult Ok(string payload = null) => new() { success = true, payload = payload };
    public static McpResult Fail(string error) => new() { success = false, error = error };
}
