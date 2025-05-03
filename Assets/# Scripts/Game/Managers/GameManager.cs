using UnityEngine;
using Zenject;
using UniRx;
using UnityEngine.SceneManagement;
using RGames.Core;

public class GameManager : MonoBehaviour, IGameManager
{
    [Inject] private InputService _inputService;

    public static GameManager Instance;

    public ReactiveProperty<GameState> CurrentGameState { get; } = new();
    public ReactiveProperty<ManagerStatus> Status { get; } = new();


    private void Awake()
    {
        Status.Subscribe(OnManagerStatusChangedHandler);

        Status.Value = ManagerStatus.Initializing;
        Instance = this;
        _inputService.EscapeIsDown.Subscribe(OnEscapeDownHandler).AddTo(this);

        print("Game Manager is initialized");
    }

    private void Start()
    {
        ChangeGameState(GameState.Played);

        print("Game Manager is Started");
        Status.Value = ManagerStatus.Started;
    }

    private void OnDestroy()
    {
        Instance = null;
    }

    public void ChangeGameState(GameState state)
    {
        if (state == GameState.Paused)
        {
            Cursor.lockState = CursorLockMode.None;
            print("Игра приостановлена");
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            print("Игра возобновлена");
        }

        CurrentGameState.Value = state;
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
            ChangeGameState(GameState.Played);
        }
        else
        {
            ChangeGameState(GameState.Paused);
        }
    }

    private void OnManagerStatusChangedHandler(ManagerStatus status)
    {
        string info = $"{nameof(HintManager)} is {status.ToString()}";
    }
}
