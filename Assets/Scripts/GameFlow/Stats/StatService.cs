using System;
using System.Collections.Generic;
using GameFlow.PlayerProgression;
using GameFlow.Upgrade.Base;
using Systems.SaveLoad;
using Zenject;

namespace GameFlow.Stats
{
    public class StatService : IStatService, IInitializable
    {
        private readonly Dictionary<UpgradeType, StatContainer> _statContainers = new();
        private readonly IPlayerProgressService _playerProgressService;
        private readonly ISaveService _saveService;


        //TODO: Move to Scriptable Objects later
        private readonly Dictionary<UpgradeType, List<StatDefinition>> _statDefinitions = new()
        {
            [UpgradeType.Drone] = new List<StatDefinition>
            {
                new()
                {
                    statType = UpgradeEffectType.IncreaseDroneBatteryCapacity, baseValue = 100f, minValue = 0f,
                    displayName = "Battery Capacity", unit = "%"
                },
                new()
                {
                    statType = UpgradeEffectType.IncreaseDroneSpeed, baseValue = 50f, minValue = 0f,
                    displayName = "Speed", unit = "km/h"
                },
                new()
                {
                    statType = UpgradeEffectType.IncreaseDroneStability, baseValue = 75f, minValue = 0f,
                    maxValue = 100f, displayName = "Stability", unit = "%"
                },
                new()
                {
                    statType = UpgradeEffectType.DecreaseDroneRepairCost, baseValue = 0f,
                    displayName = "Repair Cost Reduction", unit = "%"
                },
                new()
                {
                    statType = UpgradeEffectType.IncreaseDroneBatteryChargingSpeed, baseValue = 1f, minValue = 0.1f,
                    displayName = "Charging Speed", unit = "x"
                }
            },
            [UpgradeType.Banner] = new List<StatDefinition>
            {
                new()
                {
                    statType = UpgradeEffectType.IncreaseBannerWeight, baseValue = 10f, minValue = 0f,
                    displayName = "Weight", unit = "kg"
                },
                new()
                {
                    statType = UpgradeEffectType.IncreaseBannerDurability, baseValue = 100f, minValue = 0f,
                    displayName = "Durability", unit = "HP"
                },
                new()
                {
                    statType = UpgradeEffectType.IncreaseBannerVisibility, baseValue = 50f, minValue = 0f,
                    maxValue = 100f, displayName = "Visibility", unit = "%"
                },
                new()
                {
                    statType = UpgradeEffectType.DecreaseBannerRepairCost, baseValue = 0f,
                    displayName = "Repair Cost Reduction", unit = "%"
                }
            }
        };

        public StatService(IPlayerProgressService playerProgressService, ISaveService saveService)
        {
            _playerProgressService = playerProgressService;
            _saveService = saveService;
        }

        public void Initialize()
        {
            foreach (var kvp in _statDefinitions)
            {
                InitializeStatContainer(kvp.Key, kvp.Value);
            }

            LoadStats();

            var equippedUpgrades = _playerProgressService.GetEquippedUpgrades();
            foreach (var upgrade in equippedUpgrades)
            {
                ApplyUpgradeEffects(upgrade);
            }
        }

        public void InitializeStatContainer(UpgradeType type, List<StatDefinition> definitions)
        {
            var container = new StatContainer { containerType = type };
            container.InitializeStats(definitions);
            _statContainers[type] = container;
        }

        public float GetStat(UpgradeType containerType, UpgradeEffectType statType)
        {
            if (_statContainers.TryGetValue(containerType, out var container))
            {
                return container.GetStatValue(statType);
            }

            return 0f;
        }

        public void ApplyUpgradeEffects(BaseUpgradeData upgrade)
        {
            if (!_statContainers.TryGetValue(upgrade.upgradeType, out var container))
                return;

            foreach (var effect in upgrade.effects)
            {
                var modifier = new StatModifier(
                    upgrade.UniqueId,
                    effect.effectValue,
                    effect.isPercentage,
                    effect.effectType
                );

                container.AddModifier(modifier);
            }

            SaveStats();
        }

        public void RemoveUpgradeEffects(BaseUpgradeData upgrade)
        {
            if (!_statContainers.TryGetValue(upgrade.upgradeType, out var container))
                return;

            container.RemoveAllModifiersFromUpgrade(upgrade.UniqueId);
            SaveStats();
        }

        public Dictionary<UpgradeEffectType, float> GetAllStats(UpgradeType containerType)
        {
            var result = new Dictionary<UpgradeEffectType, float>();

            if (_statContainers.TryGetValue(containerType, out var container))
            {
                foreach (var kvp in container.stats)
                {
                    result[kvp.Key] = kvp.Value.currentValue;
                }
            }

            return result;
        }

        public void SaveStats()
        {
            
            var statsData = new StatsData
            {
                Containers = _statContainers,
                SaveTimestamp = DateTime.UtcNow.Ticks
            };

            _saveService.SaveData(statsData, "player_stats");
        }

        public void LoadStats()
        {
            var statsData = _saveService.LoadData<StatsData>("player_stats");
        
            if (statsData?.Containers != null)
            {
                foreach (var kvp in statsData.Containers)
                {
                    _statContainers[kvp.Key] = kvp.Value;
                }
            }
        }
    }
}