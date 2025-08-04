using Managers.SceneLoader;
using Systems.FSM;
using Systems.StateFactory;
using Zenject;

namespace Installers.Bootstrappers
{
    public class GameBootstrapper : IInitializable
    {
        private readonly IGameFsm _gameFsm;
        private readonly IStateFactory _stateFactory;

        public GameBootstrapper(
            IGameFsm gameFsm, 
            IStateFactory stateFactory)
        {
            _gameFsm = gameFsm;
            _stateFactory = stateFactory;
        }

        public void Initialize()
        {
            UnityEngine.Debug.Log("GameBootstrapper Initialized. Changing to FirstPersonState.");

            var firstPersonState = _stateFactory.CreateState<FirstPersonState>();
            _gameFsm.ChangeState(firstPersonState);
        }
    }
}