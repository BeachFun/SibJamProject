using UniRx;
using UnityEngine;
using Zenject;
public class SpeechManager : MonoBehaviour
{
    public ReactiveProperty<SpeechData> speechData = new();
    private ResourceService resourceService;
    public void ShowSpeech(int ID)
    {
        speechData.SetValueAndForceNotify(resourceService.GetSpeechDataByID(ID));
    }
}
