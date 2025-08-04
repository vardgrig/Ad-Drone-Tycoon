using GameFlow.Contracts;
using GameFlow.PlayerProgression;
using GameFlow.Upgrade.Base;
using GameFlow.Upgrade.Company;
using GameFlow.Upgrade.Location;
using Managers.SceneLoader;
using Systems;
using Systems.FSM;
using Systems.Pool;
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
            
            /*Container
                .BindInterfacesAndSelfTo<UpgradeService>()
                .AsSingle()
                .NonLazy();*/
            
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
    }
}