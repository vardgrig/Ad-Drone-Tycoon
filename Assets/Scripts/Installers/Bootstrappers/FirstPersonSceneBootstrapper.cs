using System;
using GameFlow;
using Systems.FSM;
using Zenject;

namespace Installers.Bootstrappers
{
    public class FirstPersonSceneBootstrapper : IInitializable, IDisposable
    {
        private readonly Computer _computer;
        private readonly FirstPersonState _firstPersonState;

        public FirstPersonSceneBootstrapper(Computer computer, FirstPersonState firstPersonState)
        {
            _computer = computer;
            _firstPersonState = firstPersonState;
        }

        public void Initialize()
        {
            UnityEngine.Debug.Log("FirstPersonSceneController Initialized. Subscribing to computer event.");

            _computer.OnComputerInteracted += _firstPersonState.OnComputerInteracted;
        }
        
        public void Dispose()
        {
            UnityEngine.Debug.Log("FirstPersonSceneController Disposed. Unsubscribing from computer event.");

            _computer.OnComputerInteracted -= _firstPersonState.OnComputerInteracted;
        }
    }
}