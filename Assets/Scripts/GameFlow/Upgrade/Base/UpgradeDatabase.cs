using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameFlow.Upgrade.Base
{
    public class UpgradeDatabase : IUpgradeDatabase
    {
        private List<BaseUpgradeData> _allUpgrades = new();
    
        public UpgradeDatabase()
        {
            LoadUpgrades();
        }
    
        private void LoadUpgrades()
        {
            _allUpgrades = Resources
                .LoadAll<BaseUpgradeData>("Upgrades")
                .ToList();
        }
    
        public List<BaseUpgradeData> GetAllUpgrades() => _allUpgrades;
    
        public BaseUpgradeData GetUpgradeById(string id)
        {
            return _allUpgrades
                .FirstOrDefault(u => u.UniqueId == id);
        }
    
        public List<BaseUpgradeData> GetUpgradesByType(UpgradeType type)
        {
            return _allUpgrades
                .Where(u => u.upgradeType == type)
                .ToList();
        }
    }
}