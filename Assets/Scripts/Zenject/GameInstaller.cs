using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public LevelData LevelData;
    public Line linePrefab;
    public LineRenderer illegalLine;
    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);
        Container.DeclareSignal<OnDrawSignal>().OptionalSubscriber();
        Container.DeclareSignal<DrawCountChangedSignal>();
        Container.DeclareSignal<TimeChangedSignal>().OptionalSubscriber();
        Container.DeclareSignal<MissionFinishedSignal>();
        Container.DeclareSignal<MissionFinishCancelSignal>().OptionalSubscriber();
        Container.DeclareSignal<LevelFinishedSignal>();
        Container.DeclareSignal<FinishTimerStartedSignal>();
        Container.DeclareSignal<LevelFinishDelayTimeChangedSignal>().OptionalSubscriber();
        Container.DeclareSignal<FinishTimerStoppedSignal>();

        Container.Bind<GameManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<LevelData>().FromInstance(LevelData).AsSingle();
        Container.Bind<IScreenRecorderSprite>().To<ScreenRecorder>().FromComponentInHierarchy().AsSingle();

        Container.Bind<DrawManager>().FromComponentInHierarchy().AsSingle();
        Container.Bind<DrawStateMachine>().AsSingle();
        Container.Bind<IDrawState>().WithId("BeforeDraw").To<BeforeDrawState>().AsSingle().WithArguments(linePrefab);
        Container.Bind<IDrawState>().WithId("Illegal").To<IllegalDrawState>().AsSingle().WithArguments(illegalLine);
        Container.Bind<IDrawState>().WithId("Drawing").To<DrawingState>().AsSingle();
        Container.Bind<IDrawState>().WithId("Locked").To<LockedState>().AsSingle();
    }
}
