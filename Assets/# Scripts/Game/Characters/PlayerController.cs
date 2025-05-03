using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RGames.Core;
using UniRx;

public class PlayerController : MonoBehaviour, ICharacter
{
    public ReactiveProperty<CharacterStatus> Status { get; private set; } = new();


    public void Kill()
    {
        Status.Value = CharacterStatus.Died;
    }
}