using UniRx;
using UnityEngine;
using Zenject;

public class GameOverManager : MonoBehaviour
{
    [Inject] IGameManager gameManager;
    [Inject] PlayerManager playerManager;


    private void Awake()
    {
        playerManager.Health.Subscribe(OnHealthUpdatedHandler);
    }


    private void OnHealthUpdatedHandler(int health)
    {
        if (health > 0) return;

        
    }
}