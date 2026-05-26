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
        EditorSceneManager.Instance.onPlayModeEnter += OnPlayModeEnter;
    }

    private void OnPlayModeEnter()
    {
        throw new NotImplementedException();
    }

    void OpenSceneScreen() => SetScreenMode(ScreenMode.Scene);
    void OpenGameScreen() => SetScreenMode(ScreenMode.Game);
    void EnterPlayMode() => EditorSceneManager.Instance.EnterPlayMode();
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