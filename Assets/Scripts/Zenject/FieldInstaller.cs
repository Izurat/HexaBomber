using Config;
using EndGame;
using Field;
using Field.Model;
using Field.Model.Bomb;
using Field.Model.Cell;
using Field.Model.Walker;
using Signals;
using UnityEngine;
using Zenject;

public class FieldInstaller : MonoInstaller
{
    [SerializeField] private FieldConfig _config;

    public override void InstallBindings()
    {
        SignalBusInstaller.Install(Container);

        Container.DeclareSignal<BombExplodedSignal>().RequireSubscriber().RunSync();
        Container.DeclareSignal<BombAddedSignal>().RunSync();
        Container.DeclareSignal<SetBombInputSignal>().RunSync();
        Container.DeclareSignal<AddBombSignal>().RunSync();
        Container.DeclareSignal<JoystickDirectionUpdateSignal>().RequireSubscriber().RunSync();
        Container.DeclareSignal<PlayerTargetIndexUpdatedSignal>().RunSync();
        Container.DeclareSignal<GameFinishedSignal>().RunSync();

        Container.BindInterfacesTo<FieldModel>().AsSingle().WithArguments(_config);
        Container.BindInterfacesTo<BombsManager>().AsSingle().WithArguments(_config); ;

        Container.Bind<RandomEnemyLogic>().AsSingle().WithArguments(_config);
        Container.Bind<UserGuidedLogic>().AsSingle().WithArguments(_config);
        Container.Bind<TargetEnemyLogic>().AsSingle().WithArguments(_config);

        Container.BindInterfacesTo<MapWalkersManager>().AsSingle().WithArguments(_config);

        Container.BindInterfacesTo<EndGameHandler>().AsSingle();
        Container.Bind<GameRestarter>().AsSingle().NonLazy();
    }
}