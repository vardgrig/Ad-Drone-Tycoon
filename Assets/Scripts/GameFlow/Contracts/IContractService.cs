using System;

namespace GameFlow.Contracts
{
    public interface IContractService
    {
        ContractSession CurrentSession { get; }
        ContractData[] AvailableContracts { get; }
    
        void LoadAvailableContracts();
        bool CanAcceptContract(ContractData contract);
        void AcceptContract(ContractData contract);
        void CompleteContract(float performanceMultiplier);
        void FailContract();
    
        event Action<ContractSession> OnContractAccepted;
        event Action<ContractSession> OnContractCompleted;
        event Action<ContractSession> OnContractFailed;
    }
}