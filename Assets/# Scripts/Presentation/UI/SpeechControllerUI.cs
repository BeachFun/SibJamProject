using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static SpeechData;

public class SpeechControllerUI : MonoBehaviour
{
    [Inject] private SpeechManager speechManager;
    [Inject] private InputService inputService;
    [SerializeField] private TextMeshPro textSpeech;
    [SerializeField] private Transform contentPanel;
    [SerializeField] private Button buttonPrefab;
    private void Start()
    {
        speechManager.dialogueData.Subscribe(ShowDialogue);
    }

    private void ShowDialogue(SpeechData data)
    {
        foreach (SpeechTemplate speechTemplate in data.SpeechTemplates)
        {
            if(speechTemplate.IsResponse)
            {
                foreach (string response in speechTemplate.SpeechLines)
                {
                    Button newButton = Instantiate(buttonPrefab, contentPanel);
                    newButton.GetComponentInChildren<Text>().text = response;
                }
            }
            else
            {
                foreach (string replica in speechTemplate.SpeechLines)
                {
                    TypeSpeech(replica);
                }
            }
        }
    }
    private void TypeSpeech(string speech, float speed = 1f)
    {

    }
    private void OnResponseChosen()
    {
        
    }
}
