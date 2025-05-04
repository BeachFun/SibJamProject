using UniRx;
using UnityEngine;

public class InputService : MonoBehaviour
{
    public ReactiveProperty<bool> EscapeIsDown { get; } = new();
    public ReactiveProperty<bool> Intaraction { get; } = new();
    public ReactiveProperty<bool> DialogueSkip { get; } = new();
    public ReactiveProperty<bool> Mouse0Press { get; } = new();


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
        Intaraction.Value = Input.GetKeyDown(KeyCode.E);
        DialogueSkip.Value = Input.GetKeyDown(KeyCode.Space);
        Mouse0Press.Value = Input.GetMouseButtonDown(0);
    }
}
