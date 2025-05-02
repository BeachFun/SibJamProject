using UnityEngine;
using Zenject;

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
