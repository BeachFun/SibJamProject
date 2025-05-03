using UniRx;

namespace RGames.Core
{
    public interface IManager
    {
        ReactiveProperty<ManagerStatus> Status { get; }
    }

    public enum ManagerStatus
    {
        NonInitialized,
        Initializing,
        Started,
        Suspended
    }
}
