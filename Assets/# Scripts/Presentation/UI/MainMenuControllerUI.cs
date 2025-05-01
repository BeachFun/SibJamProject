using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class MainMenuControllerUI : ScreenBase
{
    [Inject] UIService _uiService;


    #region Привязка методов к событиям через инспектор | Serialized Event Binding

    public void ContinuGame()
    {

    }

    public void NewGame()
    {
        SceneManager.LoadScene("Loading", LoadSceneMode.Additive);
        SceneManager.LoadSceneAsync("Game");
    }

    public void LoadGame()
    {

    }

    public void OpenSettings()
    {
        _uiService?.OpenScreen("Settings");
    }

    public void OpenCredits()
    {
        _uiService?.OpenScreen("Credits");
    }

    public void Exit()
    {
        Application.Quit();
    }

    #endregion

}
