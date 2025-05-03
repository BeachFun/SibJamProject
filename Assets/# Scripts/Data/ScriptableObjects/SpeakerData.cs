using UnityEngine;

[CreateAssetMenu(fileName = "NewDialogueData", menuName = "New Scriptable Object/SpeakerData")]
public class SpeakerData : ScriptableObject
{
    public string Name;
    public AudioClip Sound;
}
