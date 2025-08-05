using System.Collections.Generic;
using GameFlow.PlayerProgression;
using GameFlow.Stats;
using GameFlow.Upgrade.Base;
using UnityEngine;

namespace GameFlow.Drone
{
    public class DroneService : IDroneService
    {
        private List<BaseUpgradeData> _purchasedUpgrades = new();
        private readonly IPlayerProgressService _playerProgressService;
        private readonly IStatService _statService;

        public DroneService(
            IPlayerProgressService playerProgressService, 
            IStatService statService)
        {
            _playerProgressService = playerProgressService;
            _statService = statService;
        }


        public void ApplyUpgrade(BaseUpgradeData upgrade)
        {
            var result = _playerProgressService.UnlockUpgrade(upgrade);
            if (result)
            {
                Debug.Log($"Upgrade {upgrade.UniqueId} applied successfully.");
                _purchasedUpgrades.Add(upgrade);
                _statService.ApplyUpgradeEffects(upgrade);
            }
            else
            {
                Debug.LogWarning(
                    $"Failed to apply upgrade {upgrade.UniqueId}. It may already be unlocked or not applicable.");
                return;
            }
        }
        
        public void RemoveUpgrade(BaseUpgradeData upgrade)
        {
            if (_purchasedUpgrades.Contains(upgrade))
            {
                _purchasedUpgrades.Remove(upgrade);
                _statService.RemoveUpgradeEffects(upgrade);
                Debug.Log($"Upgrade {upgrade.UniqueId} removed successfully.");
            }
        }
        
        public float GetDroneSpeed() => _statService.GetStat(UpgradeType.Drone, UpgradeEffectType.IncreaseDroneSpeed);
        public float GetBatteryCapacity() => _statService.GetStat(UpgradeType.Drone, UpgradeEffectType.IncreaseDroneBatteryCapacity);
        public float GetStability() => _statService.GetStat(UpgradeType.Drone, UpgradeEffectType.IncreaseDroneStability);
        public float GetRepairCostReduction() => _statService.GetStat(UpgradeType.Drone, UpgradeEffectType.DecreaseDroneRepairCost);
        public float GetChargingSpeed() => _statService.GetStat(UpgradeType.Drone, UpgradeEffectType.IncreaseDroneBatteryChargingSpeed);


        public void Initialize()
        {
            LoadInitialUpgrades();
        }

        private void LoadInitialUpgrades()
        {
            _purchasedUpgrades = _playerProgressService
                .GetUnlockedUpgradesByType(UpgradeType.Drone);
        }
    }
}