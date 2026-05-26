using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenUI : MonoBehaviour
{
    [SerializeField] GameObject sceneScreen, gameScreen;
    [SerializeField] Button sceneButton, gameButton, playButton, stopButton;
    private void OnEnable()
    {
        EditorSceneManager.Instance.onPlayModeToggle += OnPlayModeToggle;
        OnPlayModeToggle(EditorSceneManager.Instance.playMode);
        sceneButton.onClick.AddListener(OpenSceneScreen);
        gameButton.onClick.AddListener(OpenGameScreen);
        playButton.onClick.AddListener(EnterPlayMode);
        stopButton.onClick.AddListener(ExitPlayMode);
    }
    private void OnDisable()
    {
        sceneButton.onClick.RemoveListener(OpenSceneScreen);
        gameButton.onClick.RemoveListener(OpenGameScreen);
        playButton.onClick.RemoveListener(EnterPlayMode);
        stopButton.onClick.RemoveListener(ExitPlayMode);
    }

    private void OnPlayModeToggle(bool playMode)
    {
        playButton.gameObject.SetActive(!playMode);
        stopButton.gameObject.SetActive(playMode);
    }

    void OpenSceneScreen() => SetScreenMode(ScreenMode.Scene);
    void OpenGameScreen() => SetScreenMode(ScreenMode.Game);
    void EnterPlayMode() => EditorSceneManager.Instance.EnterPlayMode();
    void ExitPlayMode() => EditorSceneManager.Instance.ExitPlayMode();
    public void SetScreenMode(ScreenMode mode)
    {
        sceneScreen.SetActive(mode == ScreenMode.Scene);
        gameScreen.SetActive(mode == ScreenMode.Game);
    }

    [System.Serializable]
    public enum ScreenMode
    {
        Game,
        Scene
    }
}