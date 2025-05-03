using RGames.Core;
using UniRx;
using UnityEngine;
using Zenject;
public class SpeechManager : MonoBehaviour
{
    [Inject] private ResourceService resourceService;
    public ReactiveProperty<SpeechData> speechData = new();

    public ReactiveProperty<ManagerStatus> Status => throw new System.NotImplementedException();

    public void ShowSpeech(int ID)
    {
        speechData.SetValueAndForceNotify(resourceService.GetSpeechDataByID(ID));
    }
    public void HandleResponse(string responseEffect)
    {
        //if...
    }
}
