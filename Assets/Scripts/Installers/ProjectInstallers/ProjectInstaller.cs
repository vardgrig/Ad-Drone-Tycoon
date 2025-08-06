using GameFlow.Contracts;
using GameFlow.Drone;
using GameFlow.PlayerProgression;
using GameFlow.Stats;
using GameFlow.Upgrade.Base;
using GameFlow.Upgrade.Company;
using GameFlow.Upgrade.Location;
using Managers.SceneLoader;
using Signals;
using Systems.FSM;
using Systems.Pool;
using Systems.SaveLoad;
using Systems.StateFactory;
using Zenject;

namespace Installers.ProjectInstallers
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            // This installer is responsible for binding the game-wide components and managers.
            Container
                .BindInterfacesAndSelfTo<StateFactory>()
                .AsSingle()
                .NonLazy();
            
            Container
                .BindInterfacesAndSelfTo<GameFsm>()
                .AsSingle()
                .NonLazy();
        
            Container
                .BindInterfacesAndSelfTo<SceneLoader>()
                .AsSingle()
                .NonLazy();
            
            Container
                .BindInterfacesAndSelfTo<UpgradeDatabase>()
                .AsSingle()
                .NonLazy();
            
            Container
                .BindInterfacesAndSelfTo<LocationDatabase>()
                .AsSingle()
                .NonLazy();
            
            Container
                .BindInterfacesAndSelfTo<CompanyDatabase>()
                .AsSingle()
                .NonLazy();
            
            Container
                .BindInterfacesAndSelfTo<PlayerProgressService>()
                .AsSingle()
                .NonLazy();
            
            Container
                .BindInterfacesAndSelfTo<ContractService>()
                .AsSingle()
                .NonLazy();
            
            Container
                .BindInterfacesAndSelfTo<StatService>()
                .AsSingle()
                .NonLazy();
            
            Container
                .BindInterfacesAndSelfTo<ObfuscatedSaveService>()
                .AsSingle()
                .NonLazy();
            
            Container
                .BindInterfacesAndSelfTo<DroneService>()
                .AsSingle()
                .NonLazy();
            
            SignalInstaller();
            
            Container
                .Bind<PoolManager>()
                .FromNewComponentOnNewGameObject()
                .AsSingle();

            Container
                .Bind<FirstPersonState>()
                .AsSingle();

            Container
                .Bind<DroneFlyingState>()
                .AsSingle();
        }

        private void SignalInstaller()
        {
            SignalBusInstaller.Install(Container);
            
            Container.DeclareSignal<PurchaseUpgradeSignal>();
            Container.DeclareSignal<EquipUpgradeSignal>();
            Container.DeclareSignal<ShowUpgradeInfoSignal>();
            Container.DeclareSignal<RefreshUpgradeUISignal>();
            Container.DeclareSignal<ShowMessageSignal>();
            Container.DeclareSignal<PlaySoundSignal>();
            Container.DeclareSignal<UpgradePurchasedSignal>();
            Container.DeclareSignal<UpgradeEquippedSignal>();
            Container.DeclareSignal<UpgradeUnequippedSignal>();
            Container.DeclareSignal<MoneyChangedSignal>();
        }
    }
}