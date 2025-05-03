using RGames.Core;
using UniRx;
using UnityEngine;
using Zenject;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [Inject] ResourceService resourceService;

    public ReactiveProperty<ManagerStatus> Status => throw new System.NotImplementedException();

    public void PlaySound(AudioClip sound)
    {
        audioSource.clip = sound;
        audioSource.Play();
    }
}
