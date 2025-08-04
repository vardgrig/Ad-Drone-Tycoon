using System.Collections.Generic;
using GameFlow.Upgrade.Base;
using UnityEngine;

namespace GameFlow.Upgrade.Company
{
    [CreateAssetMenu(fileName = "CompanyData", menuName = "Data/CompanyData", order = 1)]
    public class CompanyData : ScriptableObject
    {
        public string companyName;
        public Sprite companyLogo;
        public Color companyColor = Color.white;
    
        [Header("Deal Parameters")]
        public Currency.Currency minimumDealValue;
        public Currency.Currency maximumDealValue;
        [Range(1,24)] public int dealDurationInHours;
        [Range(0, 1)] public float dealSuccessRate;
    
        [Header("Requirements")]
        public List<BaseUpgradeData> requiredUpgrades;
        public int minimumPlayerLevel = 1;
    
        public string UniqueId => name;
    }
}