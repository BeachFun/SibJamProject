using UnityEngine;
using UniRx;

public class CheckpointManager : MonoBehaviour
{
    public ReactiveProperty<Checkpoint> CurrentCheckpoint { get; private set; } = new();

    public void SwapCheckpoint(Checkpoint point)
    {
        CurrentCheckpoint.Value = point;
    }
}
