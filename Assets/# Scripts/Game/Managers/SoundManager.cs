using RGames.Core;
using UniRx;
using UnityEngine;
using Zenject;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    public void PlaySound(AudioClip sound)
    {
        audioSource.clip = sound;
        audioSource.Play();
    }
}
