using UniRx;
using UnityEngine;
using Zenject;
public class SpeechManager : MonoBehaviour
{
    [Inject] private ResourceService resourceService;
    public ReactiveProperty<SpeechData> speechData = new();
    public void ShowSpeech(int ID)
    {
        speechData.SetValueAndForceNotify(resourceService.GetSpeechDataByID(ID));
    }
}
