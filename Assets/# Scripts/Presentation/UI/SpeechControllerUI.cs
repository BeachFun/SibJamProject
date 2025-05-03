using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Threading;
using static SpeechData;
using System.Collections.Generic;

public class SpeechControllerUI : MonoBehaviour
{
    [Inject] private SpeechManager speechManager;
    [Inject] private InputService inputService;
    [Inject] private SoundManager soundManager;

    [SerializeField] private TextMeshProUGUI textSpeech;
    [SerializeField] private Transform contentPanel;
    [SerializeField] private Button buttonPrefab;

    private CancellationTokenSource _cts; // ��� ���������� �������
    private List<Button> responseButtons;

    private void Start()
    {
        try
        {
            speechManager.speechData.SkipLatestValueOnSubscribe().Subscribe(data => ShowSpeech(data));
        }
        catch { }
    }

    private async UniTaskVoid ShowSpeech(SpeechData data)
    {
        var escapeDisposable = inputService.EscapeIsDown.Subscribe(_ => OnEscapePressed());

        foreach (SpeechTemplate speechTemplate in data.SpeechTemplates)
        {
            if (speechTemplate.IsResponse)
            {
                for (int i = 0; i < speechTemplate.SpeechLines.Count; i++)
                {
                    textSpeech.text = "";
                    Button newButton = Instantiate(buttonPrefab, contentPanel);
                    newButton.GetComponentInChildren<TextMeshProUGUI>().text = speechTemplate.SpeechLines[i];
                    newButton.onClick.AddListener(() => OnResponseChosen(speechTemplate.ResponsesEffect[i]));
                    responseButtons.Add(newButton);
                }
            }
            else
            {
                foreach (string replica in speechTemplate.SpeechLines)
                {
                    bool wasInterrupted = await TypeSpeech(replica, speechTemplate.SpeakerData.Sound);

                    // ���� �������� � ���������� � ��������� �������
                    if (wasInterrupted)
                    {
                        Debug.Log("������� ���� ��������. ��������� � ���������.");
                    }
                }
            }
        }

        escapeDisposable.Dispose();
    }

    private async UniTask<bool> TypeSpeech(string speech, AudioClip sound, int charsPerSecond = 10)
    {
        textSpeech.text = "";
        float delay = 1f / charsPerSecond;

        _cts = new CancellationTokenSource();

        try
        {
            foreach (char c in speech)
            {
                textSpeech.text += c;
                soundManager.PlaySound(sound);
                await UniTask.Delay(TimeSpan.FromSeconds(1f / charsPerSecond), cancellationToken: _cts.Token);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: _cts.Token);
            _cts.Dispose();
            return false; // �� ���� ����������
        }
        catch (OperationCanceledException)
        {
            textSpeech.text = speech; // ��������� ���������� ���� �����
            _cts.Dispose();
            return true;
        }
    }
    private void OnResponseChosen(string responseEffect)
    {
        foreach(Button button in responseButtons)
            button.onClick.RemoveAllListeners();
        responseButtons.Clear();
        speechManager.HandleResponse(responseEffect);
    }
    private void OnEscapePressed()
    {
        if (_cts != null && !_cts.IsCancellationRequested)
        {
            _cts.Cancel(); // �������� ������� ���������
        }
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}