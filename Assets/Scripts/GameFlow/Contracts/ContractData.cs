using System.Collections.Generic;
using GameFlow.Upgrade.Base;
using GameFlow.Upgrade.Company;
using UnityEngine;

namespace GameFlow.Contracts
{
    [CreateAssetMenu(fileName = "ContractData", menuName = "Data/ContractData")]
    public class ContractData : ScriptableObject
    {
        [Range(1,10)]
        public float reward;
        
        [Header("Contract Info")]
        public CompanyData company;
        public string contractName;
        public Currency.Currency baseReward;
        public float timeLimit;
    
        [Header("Route Info")]
        public Vector3[] routePoints;
        [Range(1,10)] 
        public int routeDifficulty;
    
        [Header("Requirements")]
        public List<BaseUpgradeData> requiredDroneType;
    }
}