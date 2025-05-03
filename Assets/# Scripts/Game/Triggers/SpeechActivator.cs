using UnityEngine;
using UniRx;
using Zenject;
using RGames.Core;
using System;

public class SpeechActivator : MonoBehaviour, IActivatable
{
    [SerializeField] private int dialogueID;
    [Inject] private SpeechManager dialogueManager;
    [Inject] private InputService inputService;

    private IDisposable interactionSubscription;


    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && interactionSubscription == null)
        {
            interactionSubscription = inputService.Intaraction.Subscribe(OnInteract);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            interactionSubscription?.Dispose();
        }
    }


    public void Activate()
    {
        dialogueManager.ShowSpeech(dialogueID);
    }

    private void OnInteract(bool keyState)
    {
        if (keyState)
        {
            Activate();
        }
    }
}
