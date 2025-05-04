using UnityEngine;
using RGames.Core;
using UniRx;
using Zenject;

public class PursuitManager : MonoBehaviour, IManager
{
    [Header("Settings")]
    [SerializeField] private float _intervalNone = 160;
    [SerializeField] private float _intervalSuspense = 60f;
    [SerializeField] private float _intervalTransition = 10f;
    [SerializeField] private float _intervalPursuit = 130f;

    [Header("Debug")]
    [SerializeField] private ReactiveProperty<PursuitState> _pursuitState = new();

    [Inject] IGameManager _gameManager;
    [Inject] PlayerManager _playerManager;

    private bool _ManagerIsOn = true;

    public ReactiveProperty<ManagerStatus> Status { get; } = new();
    public ReactiveProperty<float> Timer { get; } = new();


    private void Awake()
    {
        _gameManager.CurrentGameState.Subscribe(OnGameStateChangedHandler).AddTo(this);

        this.Timer.Subscribe(OnTimerUpdatedHandler).AddTo(this);
        this._pursuitState.Subscribe(OnPursuitStateUpdatedHandler).AddTo(this);

        _playerManager.OnKill += OnPlayerKilledHandler;
    }

    private void Start()
    {
        _pursuitState.Value = PursuitState.None;
    }

    private void OnDestroy()
    {
        if (_playerManager is not null)
            _playerManager.OnKill -= OnPlayerKilledHandler;
    }


    private void FixedUpdate()
    {
        if (_ManagerIsOn) Timer.Value -= Time.fixedDeltaTime;
    }



    private void OnPlayerKilledHandler()
    {
        _pursuitState.Value = PursuitState.None;
    }

    private void OnGameStateChangedHandler(GameState state)
    {
        if (state == GameState.Paused)
        {
            _ManagerIsOn = false;
            Status.Value = ManagerStatus.Suspended;
        }
        else
        {
            _ManagerIsOn = true;
            Status.Value = ManagerStatus.Started;
        }
    }

    private void OnTimerUpdatedHandler(float timer)
    {
        if (timer > 0) return;

        _pursuitState.Value = _pursuitState.Value switch
        {
            PursuitState.None => PursuitState.Suspense,
            PursuitState.Suspense => PursuitState.Transition,
            PursuitState.Transition => PursuitState.Pursuit,
            _ => PursuitState.None
        };
    }

    private void OnPursuitStateUpdatedHandler(PursuitState pursuitState)
    {
        Timer.Value = _pursuitState.Value switch
        {
            PursuitState.None => _intervalNone,
            PursuitState.Suspense => _intervalSuspense,
            PursuitState.Transition => _intervalTransition,
            _ => _intervalPursuit
        };

        switch (pursuitState)
        {
            case PursuitState.None:
                _gameManager.CurrentGameState.SetValueAndForceNotify(GameState.Played);
                break;
            case PursuitState.Suspense:
                _gameManager.CurrentGameState.SetValueAndForceNotify(GameState.Suspense);
                break;
            case PursuitState.Transition:
                _gameManager.CurrentGameState.SetValueAndForceNotify(GameState.PursuitTransition);
                break;
            case PursuitState.Pursuit:
                _gameManager.CurrentGameState.SetValueAndForceNotify(GameState.Pursuit);
                break;
        }
    }
}

public enum PursuitState
{
    None,
    Suspense,
    Transition,
    Pursuit
}