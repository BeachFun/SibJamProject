using System;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewDialogueData", menuName = "New Scriptable Object/SpeechData")]
public class SpeechData : ScriptableObject
{
    [field: SerializeField] public int ID {  get; private set; }
    [field: SerializeField] public List<SpeechTemplate> SpeechTemplates { get; private set; }
    [Serializable]
    public class SpeechTemplate
    {
        public SpeakerData SpeakerData;
        public bool IsResponse;
        [Header("Lists must have same length")]
        public List<string> SpeechLines;
        public List<string> ResponsesEffect;// hardcode
        public int charPerSecond;
        public float MonologueTimeToDisappear = 5f;
        public bool LockPlayer;
    }
}
