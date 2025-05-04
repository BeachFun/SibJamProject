using System;
using RGames.Core;
using UniRx;
using UnityEngine;
using Zenject;

public class SpeechManager : MonoBehaviour, IManager
{
    [Inject] private ResourceService resourceService;
    [Inject] private InputService inputService;
    [Inject] private IGameManager gameManager;

    public ReactiveProperty<SpeechData> speechData = new();

    public ReactiveProperty<ManagerStatus> Status { get; } = new();
    public ReactiveProperty<int> CurrentSpeechID { get; } = new(-1);


    private void Awake()
    {
        Status.Subscribe(OnManagerStatusChangedHandler);

        Status.Value = ManagerStatus.Initializing;
        inputService.Intaraction.Subscribe(OnInteractionE).AddTo(this);
        gameManager.CurrentGameState.Subscribe(OnGameStateUpdatedHandler);
    }

    private void Start()
    {
        Status.Value = ManagerStatus.Started;
    }


    public void ShowSpeech()
    {
        if (Status.Value != ManagerStatus.Started) return;

        SpeechData dialogueData = resourceService.GetSpeechDataByID(CurrentSpeechID.Value);
        CurrentSpeechID.Value = -1;

        speechData.SetValueAndForceNotify(dialogueData);
    }

    public void HandleResponse(string responseEffect)
    {
        //if...
    }


    // Callbacks
    private void OnInteractionE(bool keyState)
    {
        if (!keyState || CurrentSpeechID.Value == -1) return;

        ShowSpeech();
    }

    private void OnManagerStatusChangedHandler(ManagerStatus status)
    {
        string info = $"{nameof(HintManager)} is {status.ToString()}";
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
