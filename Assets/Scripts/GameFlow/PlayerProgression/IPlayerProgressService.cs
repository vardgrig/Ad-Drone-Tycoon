using System.Collections.Generic;
using GameFlow.Contracts;
using GameFlow.Upgrade.Base;
using GameFlow.Upgrade.Company;
using GameFlow.Upgrade.Location;

namespace GameFlow.PlayerProgression
{
    public interface IPlayerProgressService
    {
        // Properties
        PlayerProgressData ProgressData { get; }
        int PlayerLevel { get; }
        int Experience { get; }
        Currency.Currency Money { get; }
    
        // Money Management
        bool CanAfford(Currency.Currency cost);
        bool SpendMoney(Currency.Currency cost);
        void AddMoney(Currency.Currency amount);
    
        // Experience & Leveling
        void AddExperience(int amount);
        int GetExperienceRequiredForLevel(int level);
        bool IsMaxLevel { get; }
    
        // Upgrades
        bool IsUpgradeUnlocked(BaseUpgradeData upgrade);
        bool CanUnlockUpgrade(BaseUpgradeData upgrade);
        bool UnlockUpgrade(BaseUpgradeData upgrade);
        bool IsUpgradeEquipped(BaseUpgradeData upgrade);
        void EquipUpgrade(BaseUpgradeData upgrade);
        void UnequipUpgrade(BaseUpgradeData upgrade);
        List<BaseUpgradeData> GetEquippedUpgrades();
    
        // Locations
        bool IsLocationUnlocked(LocationData location);
        bool CanUnlockLocation(LocationData location);
        bool UnlockLocation(LocationData location);
    
        // Companies
        bool IsCompanyUnlocked(CompanyData company);
        bool CanUnlockCompany(CompanyData company);
        void UnlockCompany(CompanyData company);
    
        // Statistics
        void RecordContractCompletion(ContractSession session);
        void RecordCrash();
        void AddFlightTime(float seconds);
        void AddDistanceFlown(float distance);
    
        // Persistence
        void SaveProgress();
        void LoadProgress();
    
        // Events
        event System.Action<int> OnLevelUp;
        event System.Action<int> OnExperienceGained;
        event System.Action<Currency.Currency> OnMoneyChanged;
        event System.Action<BaseUpgradeData> OnUpgradeUnlocked;
        event System.Action<LocationData> OnLocationUnlocked;
    }
}