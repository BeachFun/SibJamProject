using UnityEngine;
using Zenject;
using UniRx;
using RGames.Core;

public class PlayerManager : MonoBehaviour, IManager
{
    [Header("Settings")]
    [SerializeField] private GameObject _playerPrefab;

    [Inject] private CheckpointManager _checkpointManager;

    public PlayerController Player { get; private set; }
    public ReactiveProperty<ManagerStatus> Status { get; } = new();


    private void Awake()
    {
        Status.Subscribe(OnManagerStatusChangedHandler);
        Status.Value = ManagerStatus.Initializing;
    }

    private void Start()
    {
        SpawnPlayer();
        Status.Value = ManagerStatus.Started;
    }

    /// <summary>
    /// Спавнит игрока в указанной точке спавна
    /// </summary>
    public void SpawnPlayer()
    {
        if (Player != null)
        {
            Destroy(Player);
        }

        if (_playerPrefab == null) return;

        // Выбор места спавна
        Checkpoint checkPoint = _checkpointManager.CurrentCheckpoint.Value;
        Transform point;
        if (checkPoint != null)
            point = checkPoint.transform;
        else
            point = _checkpointManager.SpawnPoint;

        Player = Instantiate(_playerPrefab, point.position, point.rotation)
                .GetComponent<PlayerController>();

        Player.Status.Subscribe(OnKill);

        // Поворот игрока
        Vector3 pos = point.transform.position;
        pos.y += 2f;
        Player.transform.position = pos;
        Player.transform.rotation = point.transform.rotation;
    }

    public void OnKill(CharacterStatus status)
    {
        if (status != CharacterStatus.Died) return;

        Checkpoint point = _checkpointManager.CurrentCheckpoint.Value;

        if (point is null) SpawnPlayer();
        else
        {
            //Player._fpsController.m_CharacterController.enabled = false;
            Vector3 pos = point.transform.position;
            pos.y += 2f;
            Player.transform.position = pos;
            Player.transform.rotation = point.transform.rotation;
            //Player._fpsController.m_CharacterController.enabled = true;
        }
    }

    private void OnManagerStatusChangedHandler(ManagerStatus status)
    {
        string info = $"{nameof(HintManager)} is {status.ToString()}";
    }

    #region Unity API

    public void KillPlayer() => Player?.Kill();

    #endregion
}
