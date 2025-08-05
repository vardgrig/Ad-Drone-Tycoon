using System.Collections.Generic;
using GameFlow.Upgrade.Base;

namespace GameFlow.Stats
{
    [System.Serializable]
    public class StatValue
    {
        public UpgradeEffectType statType;
        public float baseValue;
        public float currentValue;
        public List<StatModifier> modifiers = new();

        public StatValue(UpgradeEffectType type, float baseVal)
        {
            statType = type;
            baseValue = baseVal;
            currentValue = baseVal;
        }
    }
}