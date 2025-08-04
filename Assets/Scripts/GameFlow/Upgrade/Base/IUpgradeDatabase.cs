using System.Collections.Generic;

namespace GameFlow.Upgrade.Base
{
    public interface IUpgradeDatabase
    {
        List<BaseUpgradeData> GetAllUpgrades();
        BaseUpgradeData GetUpgradeById(string id);
        List<BaseUpgradeData> GetUpgradesByType(UpgradeType type);
    }
}