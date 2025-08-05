using GameFlow.Upgrade.Base;

namespace GameFlow.Stats
{
    [System.Serializable]
    public class StatModifier
    {
        public string sourceUpgradeId;
        public float value;
        public bool isPercentage;
        public UpgradeEffectType targetStat;

        public StatModifier(string upgradeId, float val, bool percentage, UpgradeEffectType stat)
        {
            sourceUpgradeId = upgradeId;
            value = val;
            isPercentage = percentage;
            targetStat = stat;
        }
    }
}