using UnityEngine;

public class SettingService : MonoBehaviour
{
    [SerializeField] private SettingData _data;

    /// <summary>
    /// Задержка между выводами символов на экран
    /// </summary>
    public float CharPrintDelay => _data._charPrintDelay;


    private void Awake()
    {
        print("Setting Service is initialized");
    }

    private void Start()
    {
        print("Setting Service is Started");
    }
}
