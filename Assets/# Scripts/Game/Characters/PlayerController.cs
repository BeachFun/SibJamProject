using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGames.Core;
using UniRx;

[RequireComponent(typeof(FirstPersonController))]
public class PlayerController : MonoBehaviour, ICharacter
{
    private FirstPersonController _fpsController;

    public ReactiveProperty<CharacterStatus> Status { get; private set; } = new();


    private void Awake()
    {
        _fpsController = GetComponent<FirstPersonController>();
    }


    public void Kill()
    {
        Status.Value = CharacterStatus.Died;
    }
}