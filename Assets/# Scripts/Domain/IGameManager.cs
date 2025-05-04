using RGames.Core;
using UniRx;

public interface IGameManager : IManager
{
    ReactiveProperty<GameState> CurrentGameState { get; }

    void Resume();
    void ExitGame();
    void RestartGame();
}