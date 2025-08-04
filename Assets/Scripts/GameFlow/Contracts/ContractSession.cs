using System;
using System.Collections.Generic;
using GameFlow.Currency;
using UnityEngine;

namespace GameFlow.Contracts
{
    [Serializable]
    public class ContractSession
    {
        public ContractData ContractData { get; private set; }
        public DateTime StartTime { get; private set; }
        public ContractStatus Status { get; set; }
        public float CurrentMultiplier { get; set; }
    
        // Runtime tracking data
        public int ObstacleHits { get; set; }
        public float TimeRemaining { get; set; }
        public List<Vector3> ActualPath { get; set; }
    
        public ContractSession(ContractData contractData)
        {
            ContractData = contractData;
            StartTime = DateTime.Now;
            Status = ContractStatus.Active;
            CurrentMultiplier = 1.0f;
            TimeRemaining = contractData.timeLimit;
            ActualPath = new List<Vector3>();
        }
    
        public Currency.Currency CalculateFinalReward()
        {
            var reward =  Mathf.RoundToInt(ContractData.baseReward.amount * CurrentMultiplier);
            return new Currency.Currency 
            { 
                moneyType = MoneyType.Dronix, 
                amount = reward 
            };
        }
    }

    public enum ContractStatus
    {
        Active,
        Completed,
        Failed,
        Abandoned
    }
}