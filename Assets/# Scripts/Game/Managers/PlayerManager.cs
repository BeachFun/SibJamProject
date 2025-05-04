using UnityEngine;
using Zenject;
using UniRx;
using RGames.Core;
using System;

public class PlayerManager : MonoBehaviour, IManager
{
    [Header("Settings")]
    [SerializeField] private GameObject _playerPrefab;

    [Inject] private CheckpointManager _checkpointManager;

    public PlayerController Player { get; private set; }
    public ReactiveProperty<ManagerStatus> Status { get; } = new();
    public ReactiveProperty<int> Health { get; } = new(7);


    public event Action OnKill;


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

        Player.Status.Subscribe(OnKillHandler);

        // Поворот игрока
        Vector3 pos = point.transform.position;
        pos.y += 2f;
        Player.transform.position = pos;
        Player.transform.rotation = point.transform.rotation;
    }

    public void RespawnPlayer()
    {
        // Выбор места респавна
        Checkpoint checkPoint = _checkpointManager.CurrentCheckpoint.Value;
        Transform point;
        if (checkPoint != null)
            point = checkPoint.transform;
        else
            point = _checkpointManager.SpawnPoint;

        // Поворот игрока
        Vector3 pos = point.transform.position;
        pos.y += 2f;
        Player.transform.position = pos;
        Player.transform.rotation = point.transform.rotation;
    }


    private void OnKillHandler(CharacterStatus status)
    {
        if (status != CharacterStatus.Died) return;

        Health.Value -= 1;
        if (Health.Value > 0) RespawnPlayer();

        OnKill?.Invoke();
    }

    private void OnManagerStatusChangedHandler(ManagerStatus status)
    {
        string info = $"{nameof(HintManager)} is {status.ToString()}";
    }

    #region Unity API

    [ContextMenu("Убить игрока")]
    public void KillPlayer() => Player?.Kill();

    #endregion
}
