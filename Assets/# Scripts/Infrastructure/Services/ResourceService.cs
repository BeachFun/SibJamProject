using System;
using UnityEngine;

public class ResourceService: MonoBehaviour
{
    private SpeechData[] dialogues;

    public SpeechData GetSpeechDataByID(int targetID)
    {
        if (dialogues == null)
        {
            dialogues = Resources.LoadAll<SpeechData>("Speech"); // ���� ������������ Resources
        }

        foreach (SpeechData dialogue in dialogues)
        {
            if (dialogue != null && dialogue.ID == targetID)
            {
                return dialogue;
            }
        }

        Debug.LogWarning($"�� ������ ������ ���� SpeechData � ID: {targetID}");
        return null;
    }
}
