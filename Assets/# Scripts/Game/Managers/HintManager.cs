using RGames.Core;
using UniRx;
using UnityEngine;
using Zenject;

public class HintManager : MonoBehaviour, IManager
{
    [Inject] private SpeechManager _speechManager;

    public ReactiveProperty<ManagerStatus> Status { get; } = new();
    public ReactiveProperty<string> Hint { get; } = new();

    private void Awake()
    {
        Status.Subscribe(OnManagerStatusChangedHandler);

        Status.Value = ManagerStatus.Initializing;
        _speechManager.CurrentSpeechID.Subscribe(OnDialogueUpdatedHandler).AddTo(this);
    }

    private void Start()
    {
        Status.Value = ManagerStatus.Started;
    }


    private void OnDialogueUpdatedHandler(int speechID)
    {
        if (Status.Value == ManagerStatus.Suspended) return;

        if (speechID == -1)
        {
            Hint.Value = "";
        }
        else
        {
            Hint.Value = "Нажмите 'E', чтобы начать диалог";
        }
    }

    private void OnManagerStatusChangedHandler(ManagerStatus status)
    {
        string info = $"{nameof(HintManager)} is {status.ToString()}";
    }
}