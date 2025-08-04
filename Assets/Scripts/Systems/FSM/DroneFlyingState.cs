using GameFlow.Contracts;
using Interfaces;
using Managers.SceneLoader;
using Managers.Track;
using Systems.StateFactory;
using UnityEngine;
using Zenject;

namespace Systems.FSM
{
    public class DroneFlyingState : IState
    {
        private const string DRONE_SCENE_NAME = "DroneScene";
        
        private readonly ISceneLoader _sceneLoader;
        private readonly IGameFsm _fsm;
        private readonly IStateFactory _stateFactory;
        private readonly IContractService _contractService;


        private ContractData _activeContract;
        private ITrackManager _trackManager;

        public DroneFlyingState(
            IGameFsm fsm, 
            IStateFactory stateFactory, 
            ISceneLoader sceneLoader, 
            IContractService contractService)
        {
            _fsm = fsm;
            _stateFactory = stateFactory;
            _sceneLoader = sceneLoader;
            _contractService = contractService;
        }

        public void Enter()
        {
            var session = _contractService.CurrentSession;
            var contractData = session.ContractData;

            _sceneLoader.LoadScene("DroneScene", 
                extraBindings: (container) => 
                {
                    container.Bind<ContractSession>().FromInstance(session).AsSingle();
                    container.Bind<ContractData>().FromInstance(contractData).AsSingle();
                },
                onLoaded: OnSceneLoaded);
        }

        private void OnSceneLoaded(DiContainer container)
        {
            Debug.Log("The Drone Flying State is now active and the scene is loaded. Trying to get TrackManager...");
            _trackManager = container.Resolve<ITrackManager>();
            _trackManager.OnTrackCompleted += HandleTrackCompleted;
        }

        public void Exit()
        {
            Debug.Log("Exiting Drone Flying State.");

            if (_trackManager == null) return;

            _trackManager.OnTrackCompleted -= HandleTrackCompleted;
            _trackManager = null;
        }

        private void HandleTrackCompleted()
        {
            //TODO: Replace with actual performance multiplier
            _contractService.CompleteContract(1); 
            
            var firstPersonState = _stateFactory.CreateState<FirstPersonState>();
            _fsm.ChangeState(firstPersonState);

        }
    }
}