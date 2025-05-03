using UnityEngine;
using UniRx;
using Zenject;

public class SpeechEntity : MonoBehaviour
{
    [SerializeField] private int dialogueID;
    [Inject] private SpeechManager dialogueManager;


    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            dialogueManager.CurrentSpeechID.Value = dialogueID;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            dialogueManager.CurrentSpeechID.Value = -1;
        }
    }
}
