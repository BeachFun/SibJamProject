using UnityEngine;
using RGames.Core;
using UniRx;
using Zenject;

[RequireComponent(typeof(FirstPersonController))]
public class PlayerController : MonoBehaviour, ICharacter
{
    private FirstPersonController _fpsController;

    public ReactiveProperty<CharacterStatus> Status { get; } = new();


    private void Awake()
    {
        _fpsController = GetComponent<FirstPersonController>();

        GameManager.Instance?.CurrentGameState.Subscribe(OnGameStateChangedHandler).AddTo(this); // TODO: мб ошибки в событиях из-за такого
    }


    public void Kill()
    {
        Status.Value = CharacterStatus.Died;
    }


    private void OnGameStateChangedHandler(GameState state)
    {
        if (state == GameState.Paused || state == GameState.Dialogue)
        {
            _fpsController.enabled = false;
        }
        else
        {
            _fpsController.enabled = true;
        }
    }
}