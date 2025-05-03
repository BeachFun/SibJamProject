using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Threading;
using static SpeechData;

public class SpeechControllerUI : MonoBehaviour
{
    [Inject] private SpeechManager speechManager;
    [Inject] private InputService inputService;

    [SerializeField] private TextMeshProUGUI textSpeech;
    [SerializeField] private Transform contentPanel;
    [SerializeField] private Button buttonPrefab;

    private CancellationTokenSource _cts; // Для управления печатью

    private void Start()
    {
        speechManager.speechData.SkipLatestValueOnSubscribe().Subscribe(data => ShowSpeech(data));
    }

    private async UniTaskVoid ShowSpeech(SpeechData data)
    {
        var escapeDisposable = inputService.EscapeIsDown.Subscribe(_ => OnEscapePressed());

        foreach (SpeechTemplate speechTemplate in data.SpeechTemplates)
        {
            if (speechTemplate.IsResponse)
            {
                foreach (string response in speechTemplate.SpeechLines)
                {
                    Button newButton = Instantiate(buttonPrefab, contentPanel);
                    newButton.GetComponentInChildren<TextMeshProUGUI>().text = response;
                }
            }
            else
            {
                foreach (string replica in speechTemplate.SpeechLines)
                {
                    bool wasInterrupted = await TypeSpeech(replica, charsPerSecond: 10);

                    // Если прервали — продолжаем к следующей реплике
                    if (wasInterrupted)
                    {
                        Debug.Log("Реплика была прервана. Переходим к следующей.");
                    }
                }
            }
        }

        escapeDisposable.Dispose();
    }

    private async UniTask<bool> TypeSpeech(string speech, int charsPerSecond = 10)
    {
        textSpeech.text = "";
        float delay = 1f / charsPerSecond;

        _cts = new CancellationTokenSource();

        try
        {
            foreach (char c in speech)
            {
                textSpeech.text += c;
                await UniTask.Delay(TimeSpan.FromSeconds(1f / charsPerSecond), cancellationToken: _cts.Token);
            }

            await UniTask.Delay(TimeSpan.FromSeconds(2), cancellationToken: _cts.Token);
            _cts.Dispose();
            return false; // Не было прерывания
        }
        catch (OperationCanceledException)
        {
            textSpeech.text = speech; // Мгновенно показываем весь текст
            _cts.Dispose();
            return true;
        }
    }

    private void OnEscapePressed()
    {
        if (_cts != null && !_cts.IsCancellationRequested)
        {
            _cts.Cancel(); // Отменяем текущее печатание
        }
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }
}