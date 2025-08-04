using GameFlow;
using Managers.Track;
using Zenject;

namespace Installers.SceneInstallers
{
    public class DroneSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            //This installer is responsible for binding the Drone scene specific components and managers.
            Container
                .BindInterfacesAndSelfTo<TrackManager>()
                .AsSingle()
                .NonLazy();
            
            Container
                .Bind<DronePathGenerator>()
                .FromComponentInHierarchy()
                .AsSingle()
                .NonLazy();
        }
    }
}