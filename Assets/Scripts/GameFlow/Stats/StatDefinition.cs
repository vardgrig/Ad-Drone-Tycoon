using GameFlow.Upgrade.Base;

namespace GameFlow.Stats
{
    [System.Serializable]
    public class StatDefinition
    {
        public UpgradeEffectType statType;
        public float baseValue;
        public float minValue = float.MinValue;
        public float maxValue = float.MaxValue;
        public string displayName;
        public string unit; // e.g., "km/h", "%", "seconds"
    }
}