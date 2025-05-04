using UnityEngine;
using Zenject;
using UniRx;
using UnityEngine.SceneManagement;
using RGames.Core;

public class GameManager : MonoBehaviour, IGameManager
{
    [Inject] private InputService _inputService;
    private GameState _lastGameState;


    public static GameManager Instance;

    public ReactiveProperty<GameState> CurrentGameState { get; } = new();
    public ReactiveProperty<ManagerStatus> Status { get; } = new();


    private void Awake()
    {
        CurrentGameState.Subscribe(OnChangeGameStateHandler);

        Status.Value = ManagerStatus.Initializing;
        Instance = this;
        _inputService.EscapeIsDown.Subscribe(OnEscapeDownHandler).AddTo(this);
    }

    private void Start()
    {
        CurrentGameState.Value = GameState.Played;

        Status.Value = ManagerStatus.Started;
    }

    private void OnDestroy()
    {
        Instance = null;
    }


    public void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("MainMenu");
    }


    private void OnEscapeDownHandler(bool isEscapeDown)
    {
        if (!isEscapeDown) return;

        if (CurrentGameState.Value == GameState.Paused)
        {
            CurrentGameState.Value = _lastGameState;
        }
        else
        {
            _lastGameState = CurrentGameState.Value;
            CurrentGameState.Value = GameState.Paused;
        }
    }

    public void OnChangeGameStateHandler(GameState state)
    {
        if (state == GameState.Paused || state == GameState.Dialogue)
        {
            Cursor.lockState = CursorLockMode.Confined;
            print("Игра приостановлена");
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            print("Игра возобновлена");
        }
    }
}
