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
}