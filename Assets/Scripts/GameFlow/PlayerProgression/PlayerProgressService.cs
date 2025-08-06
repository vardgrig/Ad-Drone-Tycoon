using System.Collections.Generic;
using System.Linq;
using GameFlow.Contracts;
using GameFlow.Upgrade;
using GameFlow.Upgrade.Base;
using GameFlow.Upgrade.Company;
using GameFlow.Upgrade.Location;
using UnityEngine;

namespace GameFlow.PlayerProgression
{
    public class PlayerProgressService : IPlayerProgressService
    {
        private PlayerProgressData _progressData;
        private readonly IUpgradeDatabase _upgradeDatabase;
        private readonly ILocationDatabase _locationDatabase;
        private readonly ICompanyDatabase _companyDatabase;
    
        // Experience curve settings
        private readonly int _baseExperiencePerLevel = 100;
        private readonly float _experienceMultiplier = 1.2f;
        private readonly int _maxLevel = 50;
    
        public PlayerProgressData ProgressData => _progressData;
        public int PlayerLevel => _progressData.playerLevel;
        public int Experience => _progressData.experience;
        public Currency.Currency Money => _progressData.money;
        public bool IsMaxLevel => _progressData.playerLevel >= _maxLevel;
    
        // Events
        public event System.Action<int> OnLevelUp;
        public event System.Action<int> OnExperienceGained;
        public event System.Action<Currency.Currency> OnMoneyChanged;
        public event System.Action<BaseUpgradeData> OnUpgradeUnlocked;
        public event System.Action<LocationData> OnLocationUnlocked;
    
        public PlayerProgressService(
            IUpgradeDatabase upgradeDatabase,
            ILocationDatabase locationDatabase,
            ICompanyDatabase companyDatabase)
        {
            _upgradeDatabase = upgradeDatabase;
            _locationDatabase = locationDatabase;
            _companyDatabase = companyDatabase;
        
            LoadProgress();
        }
    
        #region Money Management
        public bool CanAfford(Currency.Currency cost)
        {
            return _progressData.money >= cost;
        }
    
        public bool SpendMoney(Currency.Currency cost)
        {
            if (!CanAfford(cost)) return false;
        
            _progressData.money -= cost;
            OnMoneyChanged?.Invoke(_progressData.money);
            SaveProgress();
            return true;
        }
    
        public void AddMoney(Currency.Currency amount)
        {
            _progressData.money += amount;
            _progressData.totalEarnings += amount;
            OnMoneyChanged?.Invoke(_progressData.money);
            SaveProgress();
        }
        #endregion
    
        #region Experience & Leveling
        public void AddExperience(int amount)
        {
            if (IsMaxLevel) return;
        
            _progressData.experience += amount;
            OnExperienceGained?.Invoke(amount);
        
            // Check for level up
            int requiredExp = GetExperienceRequiredForLevel(_progressData.playerLevel + 1);
            if (_progressData.experience >= requiredExp && !IsMaxLevel)
            {
                _progressData.playerLevel++;
                OnLevelUp?.Invoke(_progressData.playerLevel);
            
                Debug.Log($"Level up! Now level {_progressData.playerLevel}");
            }
        
            SaveProgress();
        }
    
        public int GetExperienceRequiredForLevel(int level)
        {
            if (level <= 1) return 0;
        
            int totalExp = 0;
            for (int i = 2; i <= level; i++)
            {
                totalExp += Mathf.RoundToInt(_baseExperiencePerLevel * Mathf.Pow(_experienceMultiplier, i - 2));
            }
            return totalExp;
        }
        #endregion
    
        #region Upgrades
        public bool IsUpgradeUnlocked(BaseUpgradeData upgrade)
        {
            return _progressData.unlockedUpgrades.Contains(upgrade.UniqueId);
        }
    
        public bool CanUnlockUpgrade(BaseUpgradeData upgrade)
        {
            if (IsUpgradeUnlocked(upgrade)) return false;
            if (!CanAfford(upgrade.cost)) return false;
        
            foreach (var requiredUpgrade in upgrade.requiredUpgrades)
            {
                if (!IsUpgradeUnlocked(requiredUpgrade))
                    return false;
            }
        
            return true;
        }
    
        public bool UnlockUpgrade(BaseUpgradeData upgrade)
        {
            if (!CanUnlockUpgrade(upgrade)) return false;
        
            if (SpendMoney(upgrade.cost))
            {
                _progressData.unlockedUpgrades.Add(upgrade.UniqueId);
                OnUpgradeUnlocked?.Invoke(upgrade);
            
                // Give experience for unlocking upgrades
                AddExperience(50);
            
                TryEquipUpgrade(upgrade); // Automatically equip the upgrade if unlocked
                SaveProgress();
                return true;
            }
        
            return false;
        }
    
        public bool IsUpgradeEquipped(BaseUpgradeData upgrade)
        {
            return _progressData.equippedUpgrades.Contains(upgrade.UniqueId);
        }
    
        public bool TryEquipUpgrade(BaseUpgradeData upgrade)
        {
            if (!IsUpgradeUnlocked(upgrade)) return false;
            if (IsUpgradeEquipped(upgrade)) return false;
        
            _progressData.equippedUpgrades.Add(upgrade.UniqueId);
            SaveProgress();
            return true;
        }
    
        public void UnequipUpgrade(BaseUpgradeData upgrade)
        {
            _progressData.equippedUpgrades.Remove(upgrade.UniqueId);
            SaveProgress();
        }
    
        public List<BaseUpgradeData> GetEquippedUpgrades()
        {
            var equipped = new List<BaseUpgradeData>();
            foreach (var upgradeId in _progressData.equippedUpgrades)
            {
                var upgrade = _upgradeDatabase.GetUpgradeById(upgradeId);
                if (upgrade != null)
                    equipped.Add(upgrade);
            }
            return equipped;
        }
        
        public List<BaseUpgradeData> GetUnlockedUpgradesByType(UpgradeType type)
        {
            return _progressData.unlockedUpgrades
                .Select(upgradeId => _upgradeDatabase.GetUpgradeById(upgradeId))
                .Where(upgrade => upgrade && upgrade.upgradeType == type)
                .ToList();
        }
        #endregion
    
        #region Locations
        public bool IsLocationUnlocked(LocationData location)
        {
            return _progressData.unlockedLocations.Contains(location.UniqueId);
        }
    
        public bool CanUnlockLocation(LocationData location)
        {
            if (IsLocationUnlocked(location)) return false;
            if (!CanAfford(location.unlockCost)) return false;
        
            // Check required upgrades
            foreach (var requiredUpgrade in location.requiredUpgrades)
            {
                if (!IsUpgradeUnlocked(requiredUpgrade))
                    return false;
            }
        
            return true;
        }
    
        public bool UnlockLocation(LocationData location)
        {
            if (!CanUnlockLocation(location)) return false;
        
            if (SpendMoney(location.unlockCost))
            {
                _progressData.unlockedLocations.Add(location.UniqueId);
                OnLocationUnlocked?.Invoke(location);
                AddExperience(100);
                SaveProgress();
                return true;
            }
        
            return false;
        }

        #endregion
    
        #region Companies
        public bool IsCompanyUnlocked(CompanyData company)
        {
            return _progressData.unlockedCompanies.Contains(company.UniqueId);
        }
    
        public bool CanUnlockCompany(CompanyData company)
        {
            if (IsCompanyUnlocked(company)) return false;
        
            // Check player level requirement
            if (_progressData.playerLevel < company.minimumPlayerLevel)
                return false;
        
            // Check required upgrades
            foreach (var requiredUpgrade in company.requiredUpgrades)
            {
                if (!IsUpgradeUnlocked(requiredUpgrade))
                    return false;
            }
        
            return true;
        }
    
        public void UnlockCompany(CompanyData company)
        {
            if (!CanUnlockCompany(company)) return;
        
            _progressData.unlockedCompanies.Add(company.UniqueId);
            AddExperience(75); // Reward for unlocking new company
            SaveProgress();
        
            Debug.Log($"Company unlocked: {company.companyName}");
        }
        #endregion
        
        #region Statistics
        public void RecordContractCompletion(ContractSession session)
        {
            _progressData.contractsCompleted++;
        
            // Calculate experience based on contract difficulty and performance
            int baseExp = Mathf.RoundToInt(session.ContractData.routeDifficulty * 20);
            int bonusExp = Mathf.RoundToInt(baseExp * session.CurrentMultiplier);
            AddExperience(baseExp + bonusExp);
        
            SaveProgress();
        }
    
        public void RecordCrash()
        {
            _progressData.totalCrashes++;
            SaveProgress();
        }
    
        public void AddFlightTime(float seconds)
        {
            _progressData.totalFlightTime += Mathf.RoundToInt(seconds);
            SaveProgress();
        }
    
        public void AddDistanceFlown(float distance)
        {
            _progressData.totalDistanceFlown += distance;
            SaveProgress();
        }
        #endregion
    
        #region Persistence
        public void SaveProgress()
        {
            string json = JsonUtility.ToJson(_progressData, true);
            PlayerPrefs.SetString("PlayerProgress", json);
            PlayerPrefs.Save();
        }
    
        public void LoadProgress()
        {
            string json = PlayerPrefs.GetString("PlayerProgress", "");
        
            if (string.IsNullOrEmpty(json))
            {
                // Create new progress
                _progressData = new PlayerProgressData();
            
                // Unlock starting location and basic upgrades
                if (_locationDatabase.GetAllLocations().Length > 0)
                {
                    _progressData.unlockedLocations.Add(_locationDatabase.GetAllLocations()[0].UniqueId);
                }
            }
            else
            {
                _progressData = JsonUtility.FromJson<PlayerProgressData>(json);
            }
        }
        
        private void InitializeNewPlayerProgress()
    {
        // Unlock first location
        var allLocations = _locationDatabase.GetAllLocations();
        if (allLocations.Length > 0)
        {
            _progressData.unlockedLocations.Add(allLocations[0].UniqueId);
        }
        
        // Unlock first company
        var allCompanies = _companyDatabase.GetAllCompanies();
        if (allCompanies.Length > 0)
        {
            _progressData.unlockedCompanies.Add(allCompanies[0].UniqueId);
        }
        
        // Give starting upgrades if any are marked as starting equipment
        var startingUpgrades = _upgradeDatabase.GetAllUpgrades()
            .Where(u => u.cost.amount == 0) // Free upgrades are starting equipment
            .ToArray();
            
        foreach (var upgrade in startingUpgrades)
        {
            _progressData.unlockedUpgrades.Add(upgrade.UniqueId);
            _progressData.equippedUpgrades.Add(upgrade.UniqueId);
        }
        
        SaveProgress();
    }
    
    private void ValidateLoadedProgress()
    {
        // Remove any unlocked upgrades that no longer exist
        var validUpgradeIds = _upgradeDatabase.GetAllUpgrades()
            .Select(u => u.UniqueId)
            .ToHashSet();
        _progressData.unlockedUpgrades.RemoveAll(id => !validUpgradeIds.Contains(id));
        _progressData.equippedUpgrades.RemoveAll(id => !validUpgradeIds.Contains(id));
        
        // Remove any unlocked locations that no longer exist
        var validLocationIds = _locationDatabase.GetAllLocations().Select(l => l.UniqueId).ToHashSet();
        _progressData.unlockedLocations.RemoveAll(id => !validLocationIds.Contains(id));
        
        // Remove any unlocked companies that no longer exist
        var validCompanyIds = _companyDatabase.GetAllCompanies().Select(c => c.UniqueId).ToHashSet();
        _progressData.unlockedCompanies.RemoveAll(id => !validCompanyIds.Contains(id));
        
        // Ensure at least one location and company are unlocked
        if (_progressData.unlockedLocations.Count == 0)
        {
            var firstLocation = _locationDatabase.GetAllLocations().FirstOrDefault();
            if (firstLocation != null)
                _progressData.unlockedLocations.Add(firstLocation.UniqueId);
        }
        
        if (_progressData.unlockedCompanies.Count == 0)
        {
            var firstCompany = _companyDatabase.GetAllCompanies().FirstOrDefault();
            if (firstCompany != null)
                _progressData.unlockedCompanies.Add(firstCompany.UniqueId);
        }
    }
        #endregion
    }
}