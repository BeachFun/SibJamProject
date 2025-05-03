using UnityEngine;
using UniRx;

public class CheckpointManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private ReactiveProperty<Checkpoint> _currentCheckpoint;

    public Transform SpawnPoint => _spawnPoint;
    public ReactiveProperty<Checkpoint> CurrentCheckpoint => _currentCheckpoint;


    public void SwapCheckpoint(Checkpoint point)
    {
        _currentCheckpoint.Value = point;
    }
}
