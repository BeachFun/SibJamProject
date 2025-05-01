using UniRx;
using UnityEngine;

public class InputService : MonoBehaviour
{
    public ReactiveProperty<bool> EscapeIsDown { get; private set; } = new();


    private void Awake()
    {
        print("Input Service is initialized");
    }

    private void Start()
    {
        print("Input Service is Started");
    }

    private void Update()
    {
        EscapeIsDown.Value = Input.GetKeyDown(KeyCode.Escape);
    }
}
