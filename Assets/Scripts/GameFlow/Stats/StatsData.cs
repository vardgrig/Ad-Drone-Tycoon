using System.Collections.Generic;
using GameFlow.Upgrade.Base;

namespace GameFlow.Stats
{
    [System.Serializable]
    public class StatsData
    {
        public Dictionary<UpgradeType, StatContainer> Containers;
        public long SaveTimestamp;
    }
}