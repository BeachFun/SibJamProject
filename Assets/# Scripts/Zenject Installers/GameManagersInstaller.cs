using Zenject;

public class GameManagersInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<IGameManager>().To<GameManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<CheckpointManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<PlayerManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<SpeechManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<HintManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<SoundManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<MusicManager>().FromComponentInHierarchy().AsSingle().NonLazy();
        Container.Bind<PursuitManager>().FromComponentInHierarchy().AsSingle().NonLazy();
    }
}
