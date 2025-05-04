using Zenject;
using UniRx;

public class PauseMenuControllerUI : ScreenBase
{
    [Inject] IGameManager _gameManager;
    [Inject] UIService _uiService;


    private void Awake()
    {
        if (_gameManager is null) return;

        _gameManager.CurrentGameState.Subscribe(OnGameStateChangedHandler);
    }


    #region Привязка методов к событиям через инспектор | Serialized Event Binding

    public void Resume()
    {
        _gameManager?.Resume();
    }

    public void Restart()
    {
        _gameManager?.RestartGame();
    }

    public void OpenSettings()
    {
        _uiService?.OpenScreen("Settings");
    }

    public void Exit()
    {
        _gameManager?.ExitGame();
    }

    #endregion


    private void OnGameStateChangedHandler(GameState state)
    {
        if (state == GameState.Paused)
        {
            Show(true);
        }
        else
        {
            Show(false);
        }
    }
}
