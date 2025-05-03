using UniRx;

namespace RGames.Core
{
    public interface ICharacter : IKillable
    {
        ReactiveProperty<CharacterStatus> Status { get; }
    }

    public enum CharacterStatus
    {
        Alive,
        Died
    }
}
