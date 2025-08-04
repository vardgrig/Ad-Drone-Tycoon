using System;
using GameFlow.PlayerProgression;
using UnityEngine;

namespace GameFlow.Contracts
{
    public class ContractService : IContractService
    {
        private readonly IPlayerProgressService _playerProgress;
        
        public ContractSession CurrentSession { get; private set; }
        public ContractData[] AvailableContracts { get; private set; }
        
        public event Action<ContractSession> OnContractAccepted;
        public event Action<ContractSession> OnContractCompleted;
        public event Action<ContractSession> OnContractFailed;
        
        public ContractService(IPlayerProgressService playerProgress)
        {
            _playerProgress = playerProgress;
            LoadAvailableContracts();
        }
        
        public void LoadAvailableContracts()
        {
            //TODO: Load contracts from a database or server in the future.
            //TODO: Use a more efficient loading mechanism (e.g., addressables).
            
            Debug.Log("Loading available contracts...");
            AvailableContracts = Resources.LoadAll<ContractData>("Contracts");
        }
        
        public bool CanAcceptContract(ContractData contract)
        {
            if (CurrentSession == null)
            {
                // TODO: Check drone and banner parts.
                return true;
            }
            
            Debug.LogWarning("Cannot accept a new contract while another is active.");
            return false;
        }
        
        public void AcceptContract(ContractData contract)
        {
            if (!CanAcceptContract(contract))
            {
                Debug.LogWarning($"Cannot accept contract: {contract.contractName}");
                return;
            }
            
            CurrentSession = new ContractSession(contract);
            OnContractAccepted?.Invoke(CurrentSession);
            
            Debug.Log($"Contract accepted: {contract.contractName}");
        }
        
        public void CompleteContract(float performanceMultiplier)
        {
            if (CurrentSession == null) return;
            
            CurrentSession.CurrentMultiplier = performanceMultiplier;
            CurrentSession.Status = ContractStatus.Completed;
            
            var reward = CurrentSession.CalculateFinalReward();
            //TODO: Handle float precision issues with money.
            
            _playerProgress.AddMoney(reward);
            
            OnContractCompleted?.Invoke(CurrentSession);
            
            Debug.Log($"Contract completed! Reward: {reward.amount}");
            CurrentSession = null;
        }
        
        public void FailContract()
        {
            if (CurrentSession == null) return;
            
            CurrentSession.Status = ContractStatus.Failed;
            OnContractFailed?.Invoke(CurrentSession);
            
            Debug.Log("Contract failed!");
            CurrentSession = null;
        }
    }
}