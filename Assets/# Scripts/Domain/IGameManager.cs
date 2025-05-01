using RGames.Core;
using UniRx;

public interface IGameManager : IManager
{
    ReactiveProperty<GameState> CurrentGameState { get; }

    void ChangeGameState(GameState state);
    void ExitGame();
    void RestartGame();
}