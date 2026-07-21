using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MyGameObject_AudioSource : MyGameObject
{
    AudioSource audioSource;
    bool playOnAwake;
    public AudioAsset audio { get; private set; }
    public override string id => "AudioSource";
    private void OnEnable()
    {
        audioSource ??= GetComponent<AudioSource>();
    }
    public override void OnAwake()
    {
        if (playOnAwake) Play();
        base.OnAwake();
    }
    public void Play()
    {
        if (audioSource.clip != null) audioSource.Play();
        else EditorSceneManager.Instance.AddLog(new(MyLogType.Error, "AudioSource does not have a clip"));
    }
    public void SetAudio(AudioAsset audio)
    {
        if(this.audio != null) this.audio.onClipChange -= OnClipChange;
        this.audio = audio;
        if (audio != null)
        {
            audio.onClipChange += OnClipChange;
            OnClipChange(audio.clip);
        }
        else OnClipChange(null);
    }

    private void OnClipChange(AudioClip clip)
    {
        audioSource.clip = clip;
    }

    public override IEnumerable<ExposedElement> GetElements()
    {
        foreach (var i in base.GetElements()) yield return i;
        yield return new ExposedAsset(
            "Clip",
            () => audio,
            (value) => SetAudio(value as AudioAsset), 
            AssetType.Audio);
        yield return new ExposedBool(
            "Mute",
            () => audioSource.mute,
            (value) => audioSource.mute = value);
        yield return new ExposedBool(
            "Play On Awake",
            () => playOnAwake,
            (value) => playOnAwake = value);
        yield return new ExposedBool(
            "Loop",
            () => audioSource.loop,
            (value) => audioSource.loop = value);
        yield return new ExposedSlider(
            "Volume",
            () => audioSource.volume,
            (value) => audioSource.volume = value,
            0.0f, 1.0f);
        yield return new ExposedSlider(
            "Pitch",
            () => audioSource.pitch,
            (value) => audioSource.pitch = value,
            -3.0f, 3.0f);
        yield return new ExposedSlider(
            "Stereo Pan",
            () => audioSource.panStereo,
            (value) => audioSource.panStereo = value,
            -1.0f, 1.0f);
        yield return new ExposedSlider(
            "Spatial Blend",
            () => audioSource.spatialBlend,
            (value) => { audioSource.spatialBlend = value; onInspectorChange?.Invoke(); },
            0.0f, 1.0f);
        yield return new ExposedFoldout(
            "3D Sound Settings",
            new ExposedElement[]
            {
                new ExposedSlider(
                    "Doppler Level",
                    () => audioSource.dopplerLevel,
                    (value) => audioSource.dopplerLevel = value,
                    0.0f, 5.0f),
                new ExposedSlider(
                    "Spread",
                    () => audioSource.spread,
                    (value) => audioSource.spread = value,
                    0.0f, 360.0f),
                new ExposedNumber(
                    "Min Distance",
                    () => audioSource.minDistance,
                    (value) => audioSource.minDistance = value),
                new ExposedNumber(
                    "Max Distance",
                    () => audioSource.maxDistance,
                    (value) => audioSource.maxDistance = value)
            })
        { visible = audioSource.spatialBlend > 0.0f };
        yield return new ExposedButton(
            "Play Audio",
            () => Play());
    }
    public override MyGameObjectSave Save(bool prettyPrint = true)
    {
        var save = base.Save(prettyPrint);
        save.data.ulongs["audio"] = audio != null ? audio.uid : 0;
        save.data.bools["mute"] = audioSource.mute;
        save.data.bools["playOnAwake"] = playOnAwake;
        save.data.bools["loop"] = audioSource.loop;
        save.data.floats["volume"] = audioSource.volume;
        save.data.floats["pitch"] = audioSource.pitch;
        save.data.floats["panStereo"] = audioSource.panStereo;
        save.data.floats["spatialBlend"] = audioSource.spatialBlend;
        save.data.floats["dopplerLevel"] = audioSource.dopplerLevel;
        save.data.floats["spread"] = audioSource.spread;
        save.data.floats["minDistance"] = audioSource.minDistance;
        save.data.floats["maxDistance"] = audioSource.maxDistance;
        return save;
    }
    public override void Load(MyGameObjectSave save)
    {
        base.Load(save);
        if (save.data.ulongs.TryGetValue("audio", out ulong audioId))
            SetAudio(EditorSceneManager.Instance.GetAsset<AudioAsset>(audioId));
        if (save.data.bools.TryGetValue("mute", out bool mute)) audioSource.mute = mute;
        if (save.data.bools.TryGetValue("playOnAwake", out bool playOnAwakeValue)) playOnAwake = playOnAwakeValue;
        if (save.data.bools.TryGetValue("loop", out bool loop)) audioSource.loop = loop;
        if (save.data.floats.TryGetValue("volume", out float volume)) audioSource.volume = volume;
        if (save.data.floats.TryGetValue("pitch", out float pitch)) audioSource.pitch = pitch;
        if (save.data.floats.TryGetValue("panStereo", out float panStereo)) audioSource.panStereo = panStereo;
        if (save.data.floats.TryGetValue("spatialBlend", out float spatialBlend)) audioSource.spatialBlend = spatialBlend;
        if (save.data.floats.TryGetValue("dopplerLevel", out float dopplerLevel)) audioSource.dopplerLevel = dopplerLevel;
        if (save.data.floats.TryGetValue("spread", out float spread)) audioSource.spread = spread;
        if (save.data.floats.TryGetValue("minDistance", out float minDistance)) audioSource.minDistance = minDistance;
        if (save.data.floats.TryGetValue("maxDistance", out float maxDistance)) audioSource.maxDistance = maxDistance;
    }
}