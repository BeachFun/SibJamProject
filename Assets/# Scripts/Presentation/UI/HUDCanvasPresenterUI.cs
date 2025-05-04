using UnityEngine;
using UniRx;
using TMPro;
using Cysharp.Threading.Tasks;
using Zenject;
using System.Threading;
using System;

public class HUDCanvasPresenterUI : ScreenBase
{
    [Header("Bindings")]
    [SerializeField] private TMP_Text _textHint;
    [SerializeField] private TMP_Text _textTimer;

    [Inject] private SettingService _settingService;
    [Inject] private HintManager _hintManager;
    [Inject] private PursuitManager _pursuitManager;

    private CancellationTokenSource _cts;


    private void Awake()
    {
        _hintManager.Hint.Subscribe(OnHintUpdatedHandler).AddTo(this);
        _pursuitManager.Timer.Subscribe(OnTimerUpdatedHandler).AddTo(this);
    }


    private void OnHintUpdatedHandler(string hint)
    {
        if (_textHint is null) return;

        _cts?.Cancel();
        _cts?.Dispose();

        _cts = new CancellationTokenSource();

        ShowHintAsync(hint, _cts.Token);
    }

    private async UniTask ShowHintAsync(string line, CancellationToken token)
    {
        if (line is null) return;

        _textHint.text = "";

        try
        {
            foreach (char c in line)
            {
                _textHint.text += c;
                await UniTask.Delay((int)(_settingService.CharPrintDelay * 1000), cancellationToken: token);
            }
        }
        catch (OperationCanceledException)
        {
            //Debug.Log("Hint typing was canceled");
        }
    }

    private void OnTimerUpdatedHandler(float time)
    {
        //int minutes = Mathf.FloorToInt(time / 60f);
        //int seconds = Mathf.FloorToInt(time % 60f);
        //_textTimer.text = $"{minutes:00}:{seconds:00}";

        _textTimer.text = $"{(int)time}";
    }
}