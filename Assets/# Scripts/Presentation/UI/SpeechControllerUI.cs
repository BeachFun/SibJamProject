using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Threading.Tasks;
using static SpeechData;

public class SpeechControllerUI : MonoBehaviour
{
    [Inject] private SpeechManager speechManager;
    [Inject] private InputService inputService;
    [SerializeField] private TextMeshProUGUI textSpeech;
    [SerializeField] private Transform contentPanel;
    [SerializeField] private Button buttonPrefab;
    private void Start()
    {
        speechManager.speechData.Subscribe(speechData => ShowSpeech(speechData));
    }

    private async UniTaskVoid ShowSpeech(SpeechData data)
    {
        foreach (SpeechTemplate speechTemplate in data.SpeechTemplates)
        {
            if (speechTemplate.IsResponse)
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

                    await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: destroyCancellationToken);
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
