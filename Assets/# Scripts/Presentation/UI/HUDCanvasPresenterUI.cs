using UnityEngine;
using UniRx;
using TMPro;
using Cysharp.Threading.Tasks;
using Zenject;

public class HUDCanvasPresenterUI : ScreenBase
{
    [SerializeField] private TMP_Text _textHint;

    [Inject] private SettingService _settingService;
    [Inject] private HintManager _hintManager;


    private void Awake()
    {
        _hintManager.Hint.Subscribe(OnHintUpdatedHandler).AddTo(this);
    }


    private void OnHintUpdatedHandler(string hint)
    {
        if (_textHint is null) return;

        ShowHintAsync(hint);
    }

    private async UniTask ShowHintAsync(string line)
    {
        if (line is null) return;

        _textHint.text = "";

        foreach (char c in line)
        {
            _textHint.text += c;
            await UniTask.Delay((int)(_settingService.CharPrintDelay * 1000));
        }
    }
}