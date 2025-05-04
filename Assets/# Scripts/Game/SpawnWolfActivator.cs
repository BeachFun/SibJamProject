using UnityEngine;
using RGames.Core;
using Zenject;

public class SpawnWolfActivator : MonoBehaviour, IActivatable
{
    [Inject] private PursuitManager pursuit;

    public void Activate()
    {
        if (pursuit is null) return;

        pursuit.SpawnPos = this.gameObject.transform.position;
        pursuit.SudoSpawn();
    }
}