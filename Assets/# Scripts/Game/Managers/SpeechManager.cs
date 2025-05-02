using UniRx;
using UnityEngine;
using Zenject;
public class SpeechManager : MonoBehaviour
{
    public ReactiveProperty<SpeechData> dialogueData;
    private ResourceService resourceService;
    public void ShowSpeech(int ID)
    {
        dialogueData.SetValueAndForceNotify(resourceService.GetSpeechDataByID(ID));
    }
}
