using UnityEngine;
using Zenject;
using RGames.Core;

public class Checkpoint : MonoBehaviour, IActivatable
{
    [Header("Settings")]
    [SerializeField] private bool _isUseItOnes = true;

    [Inject] private CheckpointManager _manager;

    private bool _isActivated;

    public void Activate()
    {
        if (_isActivated) return;

        if (_isUseItOnes) _isActivated = true;
        _manager.SwapCheckpoint(this);
    }
}
