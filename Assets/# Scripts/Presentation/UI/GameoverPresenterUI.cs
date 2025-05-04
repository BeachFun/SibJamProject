using TMPro;
using UnityEngine;
using Zenject;

public class GameoverPresenterUI : ScreenBase
{
    [Header("Bindings")]
    [SerializeField] private TMP_Text _caption;
    [SerializeField] private TMP_Text _content;

    [Inject] private IGameManager _gameManager;


    private void Awake()
    {
        _caption.text = string.Empty;
        _content.text = string.Empty;
    }

    public void Exit()
    {
        _gameManager.ExitGame();
    }
}
