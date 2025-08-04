using System.Collections.Generic;
using GameFlow.Currency;
using UnityEngine;

namespace GameFlow.PlayerProgression
{
    [System.Serializable]
    public class PlayerProgressData
    {
        [Header("Player Stats")]
        public int playerLevel = 1;
        public int experience = 0;
        public Currency.Currency money = new() { moneyType = MoneyType.Dronix, amount = 1000 };
    
        [Header("Unlocked Content")]
        public List<string> unlockedUpgrades = new();
        public List<string> unlockedLocations = new();
        public List<string> unlockedCompanies = new();
    
        [Header("Statistics")]
        public int contractsCompleted = 0;
        public int totalFlightTime = 0; // in seconds
        public float totalDistanceFlown = 0f;
        public int totalCrashes = 0;
        public Currency.Currency totalEarnings = new();
    
        [Header("Current Equipment")]
        public List<string> equippedUpgrades = new();
        public string currentDroneLoadout = "";
    }
}