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
    [SerializeField] private SpeachButtonUI buttonPrefab;

    private bool m_isStopInRespnoce;
    private SpeechData _speechData;
    private CancellationTokenSource _ctsTyped;
    private CancellationTokenSource _ctsDelay;
    private List<SpeachButtonUI> responseButtons = new();

    public ReactiveProperty<ManagerStatus> Status { get; } = new();


    private void Awake()
    {
        speechManager.speechData.Subscribe(ShowSpeech);
        gameManager.CurrentGameState.Subscribe(OnGameStateUpdatedHandler);
    }

    private void Start()
    {
        ShowSpeech(false);
        Status.Value = ManagerStatus.Started;
    }


    private void ShowSpeech(bool isShow)
    {
        if (isShow)
        {
            if (_speechData != null && _speechData.LockPlayer)
                gameManager.CurrentGameState.Value = GameState.Dialogue;
        }
        else
        {
            gameManager.CurrentGameState.Value = GameState.Played;
        }

        gameObject.SetActive(isShow);
    }


    private async void ShowSpeech(SpeechData data)
    {
        if (Status.Value != ManagerStatus.Started || data is null) return;

        HelperClass.ClearChildren(contentPanel);
        _speechData = data;
        ShowSpeech(true);

        IDisposable typedDisposable = null, delayDisposable = null;

        if (_speechData.IsMonologue)
        {
            typedDisposable = inputService.DialogueSkip.Subscribe(OnSkipTypedPressed);
        }
        else
        {
            delayDisposable = inputService.Mouse0Press.Subscribe(OnSkipDelay);
        }

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
                    SpeachButtonUI newButton = Instantiate<SpeachButtonUI>(buttonPrefab, contentPanel);
                    newButton.Content = speechTemplate.SpeechLines[i];
                    newButton.ResponceEffect = speechTemplate.ResponsesEffect[i];
                    newButton.OnClick += OnResponseChosed;

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

                    _ctsDelay = new CancellationTokenSource();

                    try
                    {
                        if (speechTemplate.IsDelayInEnd)
                            await UniTask.Delay((int)speechTemplate.IntervalBetweenSpeechLines * 1000, cancellationToken: _ctsDelay.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        _ctsDelay.Dispose();
                    }
                }
            }
        }

        typedDisposable?.Dispose();
        delayDisposable?.Dispose();

        ShowSpeech(false);
    }

    private async UniTask<bool> TypeSpeech(string speech, AudioClip sound, int charsPerSecond = 10)
    {
        textSpeech.text = "";
        float charDelay = 1f / charsPerSecond;

        _ctsTyped = new CancellationTokenSource();

        try
        {
            foreach (char c in speech)
            {
                textSpeech.text += c;
                soundManager.PlaySound(sound);
                await UniTask.Delay((int)charDelay*1000, cancellationToken: _ctsTyped.Token);
            }

            _ctsTyped.Dispose();
            return false;
        }
        catch (OperationCanceledException)
        {
            textSpeech.text = speech;
            _ctsTyped.Dispose();
            return true;
        }
    }


    private void OnResponseChosed(SpeachButtonUI buttonUI)
    {
        // Очистка перед закрытием
        m_isStopInRespnoce = false;
        HelperClass.ClearChildren(contentPanel);

        // Отправка ответа
        speechManager.HandleResponse(buttonUI.ResponceEffect);
    }

    private void OnSkipTypedPressed(bool keyState)
    {
        if (!keyState) return;

        if (_ctsTyped != null && !_ctsTyped.IsCancellationRequested)
        {
            try
            {
                _ctsTyped.Cancel(); // Пропуск диалоговой фразы
            }
            catch { }
        }
    }

    private void OnSkipDelay(bool keyState)
    {
        if (!keyState) return;

        if (_ctsDelay != null && !_ctsDelay.IsCancellationRequested)
        {
            try
            {
                _ctsDelay.Cancel(); // Пропуск паузы между диалогами
            }
            catch { }
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