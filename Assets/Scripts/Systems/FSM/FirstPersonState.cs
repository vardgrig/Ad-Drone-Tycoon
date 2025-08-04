using GameFlow.Contracts;
using Installers;
using Interfaces;
using Managers.SceneLoader;
using Systems.StateFactory;
using UnityEngine;
using Zenject;

namespace Systems.FSM
{
    public class FirstPersonState : IState
    {
        private const string FPS_SCENE_NAME = "GarageScene";
        
        private readonly IGameFsm _fsm;
        private readonly ISceneLoader _sceneLoader;
        private readonly IStateFactory _stateFactory;
        private readonly IContractService _contractService;

        
        public FirstPersonState(
            IGameFsm fsm, 
            ISceneLoader sceneLoader, 
            IStateFactory stateFactory,
            IContractService contractService)
        {
            _fsm = fsm;
            _sceneLoader = sceneLoader;
            _stateFactory = stateFactory;
            _contractService = contractService;
        }
    
        public void Enter()
        {
            _sceneLoader.LoadScene(FPS_SCENE_NAME);
        }

        public void OnComputerInteracted(ContractData data)
        {
            _contractService.AcceptContract(data);
            
            
            var droneState = _stateFactory.CreateState<DroneFlyingState>();
            _fsm.ChangeState(droneState);
        }

        public void Exit()
        {
            Debug.Log("Exiting First Person State");
        }
    }
}