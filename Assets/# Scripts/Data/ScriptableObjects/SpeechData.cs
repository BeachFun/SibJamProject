using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;

[CreateAssetMenu(fileName = "NewDialogueData", menuName = "New Scriptable Object/SpeechData")]
public class SpeechData : ScriptableObject
{
    [field: SerializeField, Tooltip("")]
    public int ID { get; private set; }

    [field: SerializeField, Tooltip("")]
    public List<SpeechTemplate> SpeechTemplates { get; private set; }

    [field: SerializeField, Tooltip("")]
    public bool LockPlayer { get; private set; }

    [field: SerializeField, Tooltip("")]
    public bool IsMonologue { get; private set; }


    [Serializable]
    public class SpeechTemplate
    {
        [field: SerializeField, Tooltip("")]
        public SpeakerData SpeakerData { get; private set; }

        [field: SerializeField, Tooltip("")]
        public bool IsResponse { get; private set; }


        [Header("Lists must have same length")]

        [field: SerializeField, Tooltip("")]
        public List<string> SpeechLines { get; private set; }

        [field: SerializeField, Tooltip("")]
        public List<string> ResponsesEffect { get; private set; } // hardcode

        [field: SerializeField, Tooltip("Скорость вывода символов")]
        public int charPerSecond { get; private set; } = 10;

        [field: SerializeField, Tooltip("Время между обновлениями текста")]
        public float IntervalBetweenSpeechLines { get; private set; } = 5f;

        [field: SerializeField, Tooltip("Будет ли задержка в конце речи")]
        public bool IsDelayInEnd { get; private set; } = true;
    }
}
