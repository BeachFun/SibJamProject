using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLoader : MonoBehaviour
{
    [SerializeField] private float _loadDelay;
    [SerializeField] private string _sceneName = "MainMenu";
    private void Start()
    {
        LoadMainMenu();
    }

    private async void LoadMainMenu()
    {
        await UniTask.Delay((int)_loadDelay * 1000);

        SceneManager.LoadScene(_sceneName);
    }
}
