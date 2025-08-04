using System.Collections.Generic;
using GameFlow.Upgrade.Base;
using UnityEngine;

namespace GameFlow.Upgrade.Location
{
    [CreateAssetMenu(fileName = "LocationData", menuName = "Data/LocationData", order = 2)]
    public class LocationData : ScriptableObject
    {
        public string locationName;
        public Currency.Currency unlockCost;
        [TextArea(2, 3)] public string description;
        public Sprite locationImage;
        public List<BaseUpgradeData> requiredUpgrades;
    
        public string UniqueId => name;
    }
}