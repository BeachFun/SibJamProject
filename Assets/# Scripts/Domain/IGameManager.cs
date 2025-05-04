using RGames.Core;
using UniRx;

public interface IGameManager : IManager
{
    ReactiveProperty<GameState> CurrentGameState { get; }

    void ExitGame();
    void RestartGame();
}