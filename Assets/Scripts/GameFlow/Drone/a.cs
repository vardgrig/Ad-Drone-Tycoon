using System.Collections.Generic;
using GameFlow.Upgrade.Base;
using UnityEngine;
using Zenject;

namespace GameFlow.Drone
{
    public interface IDroneService
    {
        void ApplyUpgrade(BaseUpgradeData upgrade);
    }
    
    public class DroneService : IDroneService, IInitializable
    {
        private List<BaseUpgradeData> _purchasedUpgrades = new();
        private readonly IUpgradeDatabase _upgradeDatabase;
        
        public DroneService(IUpgradeDatabase upgradeDatabase)
        {
            _upgradeDatabase = upgradeDatabase;
        }


        public void ApplyUpgrade(BaseUpgradeData upgrade)
        {
            if (!upgrade)
            {
                Debug.LogError("Attempted to apply a null upgrade.");
                return;
            }
            if (_purchasedUpgrades.Contains(upgrade))
            {
                Debug.LogWarning($"Upgrade {upgrade.UniqueId} has already been applied.");
                return;
            }
            _purchasedUpgrades.Add(upgrade);
        }

        public void Initialize()
        {
            LoadInitialUpgrades();
        }

        private void LoadInitialUpgrades()
        {
            _purchasedUpgrades = _upgradeDatabase.GetUpgradesByType(UpgradeType.Drone);
        }
    }
}