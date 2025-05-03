using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using RGames.Core;

/// <summary>
/// Группа активируемых предметов. Сами по себе активируемые предметы перестают сами активировать предметы находясь в группе.
/// </summary>
public class ActivatableGroup : MonoBehaviour, IActivatable
{
    [Header("Settings")]
    [SerializeField] private bool m_isMultipleUse;
    [Space, SerializeField] private UnityEvent m_onActivating = new();
    [HideInInspector] public List<ActivatableObject> _activators;

    private bool m_isActivated;

    public void Activate()
    {
        if (!_activators.All(x => x.IsActivated)) return;

        if (m_isMultipleUse)
        {
            m_onActivating.Invoke();
        }
        else if (!m_isActivated)
        {
            m_onActivating.Invoke();
            m_isActivated = true;
        }

        _activators.ForEach(x => x.IsActivated = false);
    }
}