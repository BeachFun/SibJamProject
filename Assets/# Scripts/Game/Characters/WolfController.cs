using UnityEngine;
using UniRx;
using RGames.Core;

public class WolfController : MonoBehaviour, IManager
{
    [Header("Settings")]
    [SerializeField] private float _baseSpeed = 10f;

    public ReactiveProperty<PlayerController> Target { get; private set; } = new();
    public ReactiveProperty<ManagerStatus> Status { get; } = new();


    public void DoubleSpeed()
    {
        _baseSpeed *= 2f;
    }

    private void Awake()
    {
        GameManager.Instance.CurrentGameState.Subscribe(OnGameStateChangedHandler).AddTo(this);
    }

    private void Start()
    {
        Status.Value = ManagerStatus.Started;
    }

    private void FixedUpdate()
    {
        if (Status.Value != ManagerStatus.Started || Target.Value == null) return;

        Vector3 direction = (Target.Value.transform.position - transform.position).normalized;
        transform.position += direction * _baseSpeed * Time.fixedDeltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerController>().Kill();
            Target = null;
        }
    }


    private void OnGameStateChangedHandler(GameState state)
    {
        if (state == GameState.Paused || state == GameState.Dialogue)
        {
            Status.Value = ManagerStatus.Suspended;
        }
        else
        {
            Status.Value = ManagerStatus.Started;
        }
    }
}
