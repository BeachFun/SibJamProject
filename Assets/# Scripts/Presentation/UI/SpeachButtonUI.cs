using System;
using TMPro;
using UnityEngine;

public class SpeachButtonUI : MonoBehaviour
{
    [Header("Bindings")]
    [SerializeField] private TMP_Text _textPrefix;
    [SerializeField] private TMP_Text _textContent;

    public string Content
    {
        get => _textContent.text;
        set => _textContent.text = value;
    }

    public string ResponceEffect { get; set; }


    public event Action<SpeachButtonUI> OnClick;


    public void OnClickHandler() => OnClick?.Invoke(this);
}
