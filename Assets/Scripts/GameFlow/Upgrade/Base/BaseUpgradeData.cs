using System.Collections.Generic;
using GameFlow.Currency;
using UnityEngine;

namespace GameFlow.Upgrade.Base
{
    [CreateAssetMenu(fileName = "BaseUpgradeData", menuName = "Data/BaseUpgradeData", order = 0)]
    public class BaseUpgradeData : ScriptableObject
    {
        [Header("Info")] 
        public string upgradeName;
        [TextArea(3, 5)] 
        public string description;
        public UpgradeType upgradeType;
        public Sprite icon;

        [Header("Cost & Requirements")] public Currency.Currency cost;
        public List<BaseUpgradeData> requiredUpgrades;

        [Header("Effects")] 
        public List<BaseEffectData> effects;

        public string UniqueId => name;

#if UNITY_EDITOR
        public void Initialize(string newName)
        {
            upgradeName = newName;
            description = "";
            cost = new Currency.Currency { moneyType = MoneyType.Dronix, amount = 0 };
            requiredUpgrades = new List<BaseUpgradeData>();
            effects = new List<BaseEffectData>();
        }
#endif
    }
}