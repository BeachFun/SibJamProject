using UnityEngine;
using Zenject;

public class SpeechEntity : MonoBehaviour
{
    [SerializeField] private int speachID;
    [SerializeField] private bool isInteractive = true;
    [Inject] private SpeechManager dialogueManager;


    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isInteractive)
        {
            dialogueManager.CurrentSpeechID.Value = speachID;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isInteractive)
        {
            dialogueManager.CurrentSpeechID.Value = -1;
        }
    }

    public void ShowSpeech()
    {
        dialogueManager.CurrentSpeechID.Value = speachID;
        dialogueManager.ShowSpeech();
    }
}
