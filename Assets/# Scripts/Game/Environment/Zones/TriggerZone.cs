using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class TriggerZone : MonoBehaviour
{
    [Tooltip("Действие при входе в зону")]
    public UnityEvent action;
    [Tooltip("Действие при выходе из зоны")]
    public UnityEvent actionOnExit;

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            action.Invoke();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            actionOnExit.Invoke();
        }
    }
}