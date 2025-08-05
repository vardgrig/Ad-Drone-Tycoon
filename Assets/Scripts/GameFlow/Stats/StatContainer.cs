using System.Collections.Generic;
using GameFlow.Upgrade.Base;

namespace GameFlow.Stats
{
    [System.Serializable]
    public class StatContainer
    {
        public UpgradeType containerType;
        public Dictionary<UpgradeEffectType, StatValue> stats = new();

        public void InitializeStats(List<StatDefinition> definitions)
        {
            stats.Clear();
            foreach (var def in definitions)
            {
                stats[def.statType] = new StatValue(def.statType, def.baseValue);
            }
        }

        public float GetStatValue(UpgradeEffectType statType)
        {
            return stats.TryGetValue(statType, out var stat) ? stat.currentValue : 0f;
        }

        public void AddModifier(StatModifier modifier)
        {
            if (stats.TryGetValue(modifier.targetStat, out var stat))
            {
                stat.modifiers.RemoveAll(m => m.sourceUpgradeId == modifier.sourceUpgradeId);

                stat.modifiers.Add(modifier);

                RecalculateStat(modifier.targetStat);
            }
        }

        public void RemoveModifier(string upgradeId, UpgradeEffectType statType)
        {
            if (stats.TryGetValue(statType, out var stat))
            {
                stat.modifiers.RemoveAll(m => m.sourceUpgradeId == upgradeId);
                RecalculateStat(statType);
            }
        }

        public void RemoveAllModifiersFromUpgrade(string upgradeId)
        {
            var affectedStats = new List<UpgradeEffectType>();

            foreach (var kvp in stats)
            {
                var removedCount = kvp.Value.modifiers.RemoveAll(m => m.sourceUpgradeId == upgradeId);
                if (removedCount > 0)
                {
                    affectedStats.Add(kvp.Key);
                }
            }

            foreach (var statType in affectedStats)
            {
                RecalculateStat(statType);
            }
        }

        private void RecalculateStat(UpgradeEffectType statType)
        {
            if (!stats.TryGetValue(statType, out var stat)) return;

            var flatModifiers = 0f;
            var percentageModifiers = 0f;

            foreach (var modifier in stat.modifiers)
            {
                if (modifier.isPercentage)
                    percentageModifiers += modifier.value;
                else
                    flatModifiers += modifier.value;
            }

            stat.currentValue = (stat.baseValue + flatModifiers) * (1f + percentageModifiers / 100f);
        }
    }
}