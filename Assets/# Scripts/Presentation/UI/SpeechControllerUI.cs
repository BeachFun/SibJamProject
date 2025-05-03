using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using System.Threading;
using System.Collections.Generic;
using RGames.Core;

public class SpeechControllerUI : MonoBehaviour, IManager
{
    [Inject] private SpeechManager speechManager;
    [Inject] private InputService inputService;
    [Inject] private SoundManager soundManager;
    [Inject] private IGameManager gameManager;

    [SerializeField] private TextMeshProUGUI textSpeackerName;
    [SerializeField] private TextMeshProUGUI textSpeech;
    [SerializeField] private Transform contentPanel;
    [SerializeField] private Button buttonPrefab;

    private bool m_isStopInRespnoce;
    private SpeechData _speechData;
    private CancellationTokenSource _cts;
    private List<Button> responseButtons = new();

    public ReactiveProperty<ManagerStatus> Status { get; } = new();


    private void Awake()
    {
        speechManager.speechData.Subscribe(ShowSpeech);
        gameManager.CurrentGameState.Subscribe(OnGameStateUpdatedHandler);
    }

    private void Start()
    {
        HelperClass.ClearChildren(contentPanel);
        ShowSpeech(false);
        Status.Value = ManagerStatus.Started;
    }


    private void ShowSpeech(bool isShow)
    {
        gameObject.SetActive(isShow);
    }


    private async void ShowSpeech(SpeechData data)
    {
        if (Status.Value != ManagerStatus.Started || data is null) return;

        ShowSpeech(true);
        _speechData = data;
        var escapeDisposable = inputService.DialogueSkip.Subscribe(OnSpacePressed);

        foreach (SpeechData.SpeechTemplate speechTemplate in data.SpeechTemplates)
        {
            textSpeackerName.text = speechTemplate.SpeakerData.Name;

            // Варианты ответов
            if (speechTemplate.IsResponse)
            {
                textSpeech.text = "";
                responseButtons.Clear();

                m_isStopInRespnoce = true;
                for (int i = 0; i < speechTemplate.SpeechLines.Count; i++)
                {
                    Button newButton = Instantiate(buttonPrefab, contentPanel);
                    newButton.GetComponentInChildren<TextMeshProUGUI>().text = speechTemplate.SpeechLines[i];
                    newButton.onClick.AddListener(() =>
                    {
                        m_isStopInRespnoce = false;
                        OnResponseChosed(speechTemplate.ResponsesEffect[i]);
                    });

                    responseButtons.Add(newButton);
                }

                // Задержка до тех пор, пока игрок не нажмет кнопку
                while(m_isStopInRespnoce) await UniTask.Delay(200);
            }
            // Вывод текста
            else
            {
                for (int i = 0; i < speechTemplate.SpeechLines.Count; i++)
                {
                    await TypeSpeech(speechTemplate.SpeechLines[i], speechTemplate?.SpeakerData.Sound, speechTemplate.charPerSecond);

                    if (speechTemplate.IsDelayInEnd)
                        await UniTask.Delay((int)speechTemplate.IntervalBetweenSpeechLines*1000);
                }
            }
        }

        escapeDisposable.Dispose();

        ShowSpeech(false);
    }

    private async UniTask<bool> TypeSpeech(string speech, AudioClip sound, int charsPerSecond = 10)
    {
        textSpeech.text = "";
        float charDelay = 1f / charsPerSecond;

        _cts = new CancellationTokenSource();

        try
        {
            foreach (char c in speech)
            {
                textSpeech.text += c;
                soundManager.PlaySound(sound);
                await UniTask.Delay((int)charDelay*1000, cancellationToken: _cts.Token);
            }

            _cts.Dispose();
            return false;
        }
        catch (OperationCanceledException)
        {
            textSpeech.text = speech;
            _cts.Dispose();
            return true;
        }
    }


    private void OnResponseChosed(string responseEffect)
    {
        // Очистка перед закрытием
        HelperClass.ClearChildren(contentPanel);

        // Отправка ответа
        speechManager.HandleResponse(responseEffect);
    }

    private void OnSpacePressed(bool keyState)
    {
        if (!keyState || _speechData.IsMonologue) return;

        if (_cts != null && !_cts.IsCancellationRequested)
        {
            _cts.Cancel(); // Пропуск диалоговой фразы
        }
    }


    private void OnGameStateUpdatedHandler(GameState gameState)
    {
        if (gameState == GameState.Paused)
        {
            Status.Value = ManagerStatus.Suspended;
        }
        else
        {
            Status.Value = ManagerStatus.Started;
        }
    }
}