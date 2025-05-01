using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class UIService : MonoBehaviour
{
    [SerializeField] private KeyScreenPair[] _screenHashTable;

    private Dictionary<string, ScreenBase> _screens = new();


    private void Awake()
    {
        print("UI Service is initialized");
    }

    private void Start()
    {
        print("UI Service is Started");
    }


    public void OpenScreen(string screenName)
    {
        var pair = _screenHashTable.FirstOrDefault(x => x.screenName == screenName);

        if (pair is null)
        {
            Debug.LogWarning("Не удалось создать Screen на сцене. Screen с таким именем в хэш-таблице не существует...");
            return;
        }

        ScreenBase screenPrefab = pair.screenPrefab;

        if (screenPrefab is null)
        {
            Debug.LogError("Не удалось создать Screen на сцене. Нет ссылки на префаб canvas с UI!");
            return;
        }

        GameObject screenObj = Instantiate(screenPrefab.gameObject);

        ScreenBase screen = screenObj.GetComponent<ScreenBase>();
        if (screen is null)
        {
            Debug.LogWarning("Не удалось создать Screen на сцене. К canvas не прикреплен скрипт для UI...");
            DestroyImmediate(screenObj);
            return;
        }
        screen.Show(true);
        screen.OnVisibleUpdated += OnScreenVisibleUpdatedHandler; // Смысла в отписке от события нет, ведь Сервис UI обязан быть всегда

        _screens.Add(screenName, screen);
        print($"Screen {screenName} открыт");
    }

    public void CloseScreen(string screenName)
    {
        if (!_screens.ContainsKey(screenName)) return;

        _screens[screenName].Show(false);
    }

    private void OnScreenVisibleUpdatedHandler(ScreenBase screen, bool isShow)
    {
        if (isShow) return;

        string key = _screens.FirstOrDefault(kv => kv.Value == screen).Key;
        if (key != null)
        {
            _screens.Remove(key);
            Destroy(screen.gameObject);
        }
    }
}

[System.Serializable]
public class KeyScreenPair
{
    public string screenName;
    public ScreenBase screenPrefab;
}
