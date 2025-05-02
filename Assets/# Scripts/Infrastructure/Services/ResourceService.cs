using System;
using UnityEngine;

public class ResourceService: MonoBehaviour
{
    private SpeechData[] dialogues;

    public SpeechData GetSpeechDataByID(int targetID)
    {
        if (dialogues == null)
        {
            dialogues = Resources.LoadAll<SpeechData>("Speech"); // Путь относительно Resources
        }

        foreach (SpeechData dialogue in dialogues)
        {
            if (dialogue != null && dialogue.ID == targetID)
            {
                return dialogue;
            }
        }

        Debug.LogWarning($"Не найден объект типа SpeechData с ID: {targetID}");
        return null;
    }
}
