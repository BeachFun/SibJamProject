using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingCanvas : ScreenBase
{
    [SerializeField] private TMP_Text _caption;
    [SerializeField] private TMP_Text _text;
    public void ChangeEndingTo(string caption, string text)
    {
        _caption.text = caption;
        _caption.text = text;
    }
    public void Exit()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
