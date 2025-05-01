using Zenject;

public class GameManagersInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IGameManager>().To<GameManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<CheckpointManager>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
}
