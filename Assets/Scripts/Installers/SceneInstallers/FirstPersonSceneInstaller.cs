using GameFlow;
using Installers.Bootstrappers;
using UI.Controllers;
using UnityEngine;
using Zenject;

namespace Installers.SceneInstallers
{
    public class FirstPersonSceneInstaller : MonoInstaller
    {
        [SerializeField] private PlayerProgressUIController playerProgressUI;
        [SerializeField] private ContractUIController contractUI;

        public override void InstallBindings()
        {
            Container
                .Bind<Computer>()
                .FromComponentInHierarchy()
                .AsSingle()
                .NonLazy();

            if (playerProgressUI)
            {
                Container
                    .Bind<PlayerProgressUIController>()
                    .FromInstance(playerProgressUI)
                    .AsSingle();
            }

            Container
                .BindInterfacesAndSelfTo<UpgradeItemUIController>()
                .AsSingle();

            if (contractUI)
            {
                Container
                    .Bind<ContractUIController>()
                    .FromInstance(contractUI)
                    .AsSingle();
            }

            Container
                .BindInterfacesAndSelfTo<FirstPersonSceneBootstrapper>()
                .AsSingle()
                .NonLazy();
        }
    }
}