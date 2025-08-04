using System;
using GameFlow.Contracts;
using Interfaces;
using UnityEngine;

namespace GameFlow
{
    public class Computer : MonoBehaviour, IInteractable
    {
        public event Action<ContractData> OnComputerInteracted; 
        [SerializeField] private ContractData contractData;
        
        public void Interact()
        {
            OnComputerInteracted?.Invoke(contractData);
        }
    }
}