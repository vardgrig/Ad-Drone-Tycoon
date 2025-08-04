using Installers.Bootstrappers;
using Zenject;

namespace Installers.SceneInstallers
{
    public class BootstrapperInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container
                .BindInterfacesAndSelfTo<GameBootstrapper>()
                .AsSingle()
                .NonLazy();
        }
    }
}