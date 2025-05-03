using RGames.Core;
using System.Resources;
using UniRx;
using UnityEngine;
using Zenject;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [Inject] ResourceManager resourceManager;
    public ReactiveProperty<ManagerStatus> Status => throw new System.NotImplementedException();
    public void PlaySound(AudioClip sound)
    {
        audioSource.clip = sound;
        audioSource.Play();
    }
}
