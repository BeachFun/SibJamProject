using UnityEngine;
using Zenject;
using RGames.Core;

public class SpeechActivator : MonoBehaviour, IActivatable
{
    [SerializeField] private int dialogueID;
    [Inject] private SpeechManager dialogueManager;
    public void Activate()
    {
        dialogueManager.ShowSpeech(dialogueID);
    }
    public void OnTrigger()
    {
        Activate();
    }
}
