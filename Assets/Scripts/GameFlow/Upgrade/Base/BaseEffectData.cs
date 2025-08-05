using UnityEngine;

namespace GameFlow.Upgrade.Base
{
    [CreateAssetMenu(fileName = "Base Effect Data", menuName = "Data/Base Effect Data", order = 1)]
    public class BaseEffectData : ScriptableObject
    {
        public UpgradeEffectType effectType;
        public float effectValue;
        public bool isPercentage;
    }
}