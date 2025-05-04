using UnityEngine;
using UnityEngine.Audio;
using Zenject;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;


    public void PlaySound(AudioClip sound)
    {
        if (audioSource == null) return;

        audioSource.clip = sound;
        audioSource.Play();
    }
}
