using System;
using UnityEngine;

public class ScreenBase : MonoBehaviour, IScreen<ScreenBase>
{
    public bool IsVisible { get; private set; }

    public event Action<ScreenBase, bool> OnVisibleUpdated;

    public void Show(bool isShow)
    {
        IsVisible = isShow;
        this.gameObject.SetActive(isShow);

        OnVisibleUpdated?.Invoke(this, isShow);
    }
}