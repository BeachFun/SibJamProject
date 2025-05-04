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

    [Header("Wolf settings")]
    [SerializeField] private Transform _spawnPointDefault;
    [SerializeField] private WolfController _wolfPrefab;

    [Header("Debug")]
    [SerializeField] private ReactiveProperty<PursuitState> _pursuitState = new();


    [Inject] IGameManager _gameManager;
    [Inject] PlayerManager _playerManager;

    private bool _ManagerIsOn = true;
    private bool _isSudoSpawn;
    private WolfController _currentWolf;

    public Vector3? SpawnPos { get; set; }
    public ReactiveProperty<ManagerStatus> Status { get; } = new();
    public ReactiveProperty<float> Timer { get; } = new();
    public ReactiveProperty<PursuitState> PursState => _pursuitState;



    private void Awake()
    {
        _gameManager.CurrentGameState.Subscribe(OnGameStateChangedHandler).AddTo(this);

        this.Timer.Subscribe(OnTimerUpdatedHandler).AddTo(this);
        Timer.Value = _intervalNone;
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
        if (state == GameState.Paused || state == GameState.Dialogue)
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
                DestroyWolf();
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
                SpawnWolf();
                break;
        }
    }

    public void SudoSpawn()
    {
        _isSudoSpawn = true;
        _pursuitState.Value = PursuitState.Transition;
    }

    private void SpawnWolf()
    {
        _currentWolf = Instantiate(_wolfPrefab.gameObject).GetComponent<WolfController>();
        _currentWolf.gameObject.transform.position = SpawnPos ?? _spawnPointDefault.transform.position;
        _currentWolf.Target.Value = _playerManager.Player;

        if (!_isSudoSpawn)
            _currentWolf.DoubleSpeed();
    }

    public void DestroyWolf()
    {
        SpawnPos = null;

        if (_currentWolf is not null)
            Destroy(_currentWolf.gameObject);
    }
}

public enum PursuitState
{
    None,
    Suspense,
    Transition,
    Pursuit
}
