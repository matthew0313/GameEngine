using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class AudioAsset : FileAsset
{
    public override AssetType type => AssetType.Audio;
    public override Sprite assetImage => EditorSceneManager.Instance.assetsSettings.audioAssetIcon;

    public AudioClip clip { get; private set; }
    public event Action<AudioClip> onClipChange;

    CancellationTokenSource loadCts;

    public override void LoadFile(string filePath)
    {
        if (!File.Exists(filePath) || !filePath.EndsWith(".mp3"))
        {
            EditorSceneManager.Instance.AddLog(new()
            {
                type = MyLogType.Warning,
                message = $"Failed to load audio asset: {filePath}"
            });
            SetClip(null);
            return;
        }
        this.filePath = filePath;
        loadCts?.Cancel();
        loadCts?.Dispose();
        loadCts = new CancellationTokenSource();
        Load(filePath, loadCts.Token).Forget();
    }

    async UniTask Load(string filePath, CancellationToken token)
    {
        string url = new Uri(Path.GetFullPath(filePath)).AbsoluteUri;
        using UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);
        try
        {
            await request.SendWebRequest().WithCancellation(token);
        }
        catch (OperationCanceledException) { return; }
        if (request.result != UnityWebRequest.Result.Success)
        {
            EditorSceneManager.Instance.AddLog(new()
            {
                type = MyLogType.Warning,
                message = $"Failed to load audio asset: {filePath} ({request.error})"
            });
            SetClip(null);
            return;
        }
        AudioClip loaded = DownloadHandlerAudioClip.GetContent(request);
        loaded.name = Path.GetFileNameWithoutExtension(filePath);
        SetClip(loaded);
    }

    void SetClip(AudioClip newClip)
    {
        clip = newClip;
        onClipChange?.Invoke(clip);
        OnDisplayUpdate();
    }

    public override void OnRemove()
    {
        base.OnRemove();
        loadCts?.Cancel();
        loadCts?.Dispose();
        loadCts = null;
    }
}
