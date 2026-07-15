using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MyGameObject_AudioSource : MyGameObject
{
    AudioSource audioSource;
    bool playOnAwake;
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
        audioSource.Play();
    }
    public override IEnumerable<ExposedElement> GetElements()
    {
        foreach(var i in base.GetElements()) yield return i;
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
    }
}