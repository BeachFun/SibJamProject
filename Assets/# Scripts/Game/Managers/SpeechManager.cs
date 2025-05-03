using System;
using RGames.Core;
using UniRx;
using UnityEngine;
using Zenject;
public class SpeechManager : MonoBehaviour, IManager
{
    [Inject] private ResourceService resourceService;
    [Inject] private InputService inputService;

    public ReactiveProperty<SpeechData> speechData = new();

    public ReactiveProperty<ManagerStatus> Status { get; } = new();
    public ReactiveProperty<int> CurrentSpeechID { get; } = new(-1);


    private void Awake()
    {
        Status.Value = ManagerStatus.Initializing;
        inputService.Intaraction.Subscribe(OnInteract).AddTo(this);
    }

    private void Start()
    {
        Status.Value = ManagerStatus.Started;
    }


    public void ShowSpeech() => ShowSpeech(CurrentSpeechID.Value);
    public void ShowSpeech(int ID)
    {
        speechData.SetValueAndForceNotify(resourceService.GetSpeechDataByID(ID));
    }

    public void HandleResponse(string responseEffect)
    {
        //if...
    }


    private void OnInteract(bool keyState)
    {
        if (!keyState || CurrentSpeechID.Value == -1) return;

        ShowSpeech(this.CurrentSpeechID.Value);
    }
}
