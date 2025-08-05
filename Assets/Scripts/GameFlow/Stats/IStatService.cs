using System.Collections.Generic;
using GameFlow.Upgrade.Base;

namespace GameFlow.Stats
{
    public interface IStatService
    {
        void InitializeStatContainer(UpgradeType type, List<StatDefinition> definitions);
        float GetStat(UpgradeType containerType, UpgradeEffectType statType);
        void ApplyUpgradeEffects(BaseUpgradeData upgrade);
        void RemoveUpgradeEffects(BaseUpgradeData upgrade);
        Dictionary<UpgradeEffectType, float> GetAllStats(UpgradeType containerType);
        void SaveStats();
        void LoadStats();
    }
}