using UnityEngine;
using UnityEngine.Events;
using RGames.Core;

[RequireComponent(typeof(AudioSource))]
public class ActivatableObject : MonoBehaviour, IActivatable
{
    [Header("Settings")]
    [SerializeField] protected bool m_isMultipleUse;
    [SerializeField] private AudioClip m_clip;
    [Tooltip("Не работает если находится в группе кнопок"), Space]
    [SerializeField] protected UnityEvent m_action = new();
    [Header("Bindings")]
    [SerializeField] private ActivatableGroup group;

    private bool m_isActivated;
    private AudioSource m_audioSource;

    public bool IsActivated
    {
        get => m_isActivated;
        internal set => m_isActivated = value;
    }


    protected virtual void Awake()
    {
        if (group is not null)
            group._activators.Add(this);

        m_audioSource = GetComponent<AudioSource>();

        if (m_audioSource is not null)
            m_audioSource.clip = m_clip;
    }

    private void OnDestroy()
    {
        if (group is not null && group._activators.Contains(this))
            group._activators.Remove(this);
    }

    public void Activate()
    {
        m_audioSource.Play();

        if (group is not null)
        {
            m_isActivated = true;
            group.Activate();
        }
        else if (m_isMultipleUse)
        {
            m_action.Invoke();
        }
        else if (!m_isActivated)
        {
            m_action.Invoke();
            m_isActivated = true;
        }
    }

    /// <summary>
    /// Подсвечивает объект при наведении
    /// </summary>
    /// <param name="state"></param>
    public void Highlight(bool state)
    {

    }
}
