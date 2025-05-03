using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class GameLoadingUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _textField;
    [SerializeField] private float _delayBetweenLines = 1.5f;
    [SerializeField] private float _dotAnimationDelay = 0.4f;

    [Inject] private SettingService _settingService;

    private void Start()
    {
        ShowTextSequenceAsync();
    }

    private async void ShowTextSequenceAsync()
    {
        _textField.text = "";

        await ShowLineAsync("> Загрузка маршрута");
        await AnimateDotsAsync();

        await ShowLineAsync("> Получатель: Babushka_Os [Тишина 42 дней]");
        await AnimateDotsAsync();

        await ShowLineAsync("> Груз: 10 пирожков [Резервные копии]");
        await AnimateDotsAsync();

        await ShowLineAsync("> Протокол: Red Hat v1.0.1");

        SceneManager.LoadScene("Game");
    }

    private async UniTask ShowLineAsync(string line)
    {
        _textField.text += "\n";

        foreach (char c in line)
        {
            _textField.text += c;
            await UniTask.Delay((int)(_settingService.CharPrintDelay * 1000));
        }
    }

    private async UniTask AnimateDotsAsync()
    {
        int dotCount = 3;
        string baseText = _textField.text;

        for (int i = 1; i <= dotCount; i++)
        {
            _textField.text = baseText + new string('.', i);
            await UniTask.Delay((int)(_dotAnimationDelay * 1000));
        }

        _textField.text = baseText + "\n";
        await UniTask.Delay((int)(_delayBetweenLines * 1000));
    }
}