using System;
using System.Linq;
using GameFlow.Currency;
using GameFlow.PlayerProgression;
using GameFlow.Upgrade.Base;
using Signals;
using UI.Views;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace UI.Controllers
{
    public class UpgradeItemUIController : IInitializable, IDisposable
    {
        // private readonly UpgradeItemUI _view;
        private readonly IPlayerProgressService _playerProgress;
        private readonly IUpgradeDatabase _upgradeDatabase;
        private readonly SignalBus _signalBus;
        private GameObject _upgradeItemPrefab;


        public UpgradeItemUIController(
            // UpgradeItemUI view, 
            IPlayerProgressService playerProgress,
            SignalBus signalBus, 
            IUpgradeDatabase upgradeDatabase)
        {
            // _view = view;
            _playerProgress = playerProgress;
            _signalBus = signalBus;
            _upgradeDatabase = upgradeDatabase;
        }

        public void Initialize()
        {
            _signalBus.Subscribe<RefreshUpgradeUISignal>(OnRefreshRequested);
            _signalBus.Subscribe<PurchaseUpgradeSignal>(OnUpgradePurchased);
            _signalBus.Subscribe<EquipUpgradeSignal>(OnUpgradeEquipped);
            _signalBus.Subscribe<ShowUpgradeInfoSignal>(OnShowUpgradeInfo);
            
            SetupView();
        }

        private void OnShowUpgradeInfo(ShowUpgradeInfoSignal signal)
        {
            var upgradeId = signal.UpgradeId;
            var upgradeData = _upgradeDatabase.GetUpgradeById(upgradeId);

            if (!upgradeData)
            {
                Debug.LogError($"Upgrade with ID {upgradeId} not found in database.");
                return;
            }
            if (_playerProgress.IsUpgradeUnlocked(upgradeData))
            {
                 //TODO: Show upgrade info UI
                 _signalBus.Fire(new ShowMessageSignal("Upgrade Info: " + upgradeData.description, Color.white));
            }
            else
            {
                _signalBus.Fire(new ShowMessageSignal("Upgrade not unlocked yet.", Color.yellow));
            }
        }

        private void OnUpgradeEquipped(EquipUpgradeSignal signal)
        {
            var upgradeId = signal.UpgradeId;
            var upgradeData = _upgradeDatabase.GetUpgradeById(upgradeId);
            if (!upgradeData)
            {
                Debug.LogError($"Upgrade with ID {upgradeId} not found in database.");
                return;
            }
            
            if (_playerProgress.IsUpgradeUnlocked(upgradeData))
            {
                var equipResponse = _playerProgress.TryEquipUpgrade(upgradeData);
                if (equipResponse)
                {
                    _signalBus.Fire(new UpgradeEquippedSignal(upgradeId));
                    RefreshUI(upgradeId);
                }
                else
                {
                    _signalBus.Fire(new ShowMessageSignal("Failed to toggle upgrade equipped state.", Color.red));
                }
            }
            else
            {
                _signalBus.Fire(new ShowMessageSignal("Cannot equip upgrade. Check requirements.", Color.red));
            }
        }

        private void OnUpgradePurchased(PurchaseUpgradeSignal signal)
        {
            var upgradeId = signal.UpgradeId;
            var upgradeData = _upgradeDatabase.GetUpgradeById(upgradeId);
            if (!upgradeData)
            {
                Debug.LogError($"Upgrade with ID {upgradeId} not found in database.");
                return;
            }
            
            if (_playerProgress.CanUnlockUpgrade(upgradeData))
            {
                var upgradeResponse = _playerProgress.UnlockUpgrade(upgradeData);
                if (upgradeResponse)
                {
                    _signalBus.Fire(new MoneyChangedSignal(upgradeData.cost));
                    _signalBus.Fire(new UpgradePurchasedSignal(upgradeId));
                    RefreshUI(upgradeId);
                }
                else
                {
                    _signalBus.Fire(new ShowMessageSignal("Failed to purchase upgrade. Please try again.", Color.red));
                }
            }
            else
            {
                _signalBus.Fire(new ShowMessageSignal("Cannot purchase upgrade. Check requirements.", Color.red));
            }
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<RefreshUpgradeUISignal>(OnRefreshRequested);
        }

        private void SetupView()
        {
            var upgrades = _upgradeDatabase.GetAllUpgrades();
            if (upgrades.Count == 0)
            {
                Debug.LogWarning("No upgrades found in database.");
                return;
            }
            var upgradeEffectTypeNames = Enum.GetNames (typeof(UpgradeEffectType));

            
            foreach (var effectTypeName in upgradeEffectTypeNames)
            {
                var effectType = Enum.Parse<UpgradeEffectType>(effectTypeName);
                var firstLockedUpdateDataOfType =
                    upgrades
                        .Where(u => u.effects
                            .Any(e => e.effectType == effectType))
                        .FirstOrDefault(u => !_playerProgress.IsUpgradeUnlocked(u));
                
                if (!firstLockedUpdateDataOfType)
                {
                    Debug.LogWarning($"No locked upgrade found for effect type: {effectTypeName}");
                    continue;
                }

                var view = Object.Instantiate(_upgradeItemPrefab);
                var viewComponent = view.GetComponent<UpgradeItemUI>();
                
                viewComponent.SetUpgradeId(firstLockedUpdateDataOfType.UniqueId);
                viewComponent.SetName(firstLockedUpdateDataOfType.upgradeName);
                viewComponent.SetDescription(firstLockedUpdateDataOfType.description);
                viewComponent.SetCost(FormatCurrency(firstLockedUpdateDataOfType.cost));
            
                if (firstLockedUpdateDataOfType.icon)
                    _view.SetIcon(firstLockedUpdateDataOfType.icon);
                
                RefreshUI(firstLockedUpdateDataOfType.UniqueId);
            }
        }

        private void OnRefreshRequested(RefreshUpgradeUISignal signal)
        {
            var targetId = signal.UpgradeId;
            if (targetId == "ALL")
            {
                RefreshUI("ALL");
                return;
            }

            var upgradeId = signal.UpgradeId;
            var upgradeData = _upgradeDatabase.GetUpgradeById(upgradeId);
            if (!upgradeData)
            {
                Debug.LogError($"Upgrade with ID {upgradeId} not found in database.");
                return;
            }

            if (targetId == upgradeId)
            {
                RefreshUI(upgradeId);
            }
        }

        private void RefreshUI(string upgradeId)
        {
            var state = GetCurrentState();
            ApplyUIState(state);
        }

        private UIStateData GetCurrentState()
        {
            return new UIStateData
            {
                IsUnlocked = _playerProgress.IsUpgradeUnlocked(_upgradeData),
                CanAfford = _playerProgress.CanAfford(_upgradeData.cost),
                CanUnlock = _playerProgress.CanUnlockUpgrade(_upgradeData),
                IsEquipped = _playerProgress.IsUpgradeEquipped(_upgradeData)
            };
        }

        private void ApplyUIState(UIStateData state)
        {
            _view.SetLockedOverlay(!state.IsUnlocked && !state.CanUnlock);
            _view.SetOwnedIndicator(state.IsUnlocked);
            _view.SetEquippedIndicator(state.IsEquipped);
            
            // Background color
            var uiState = state.IsEquipped ? UpgradeItemUIState.Equipped : 
                state.IsUnlocked ? UpgradeItemUIState.Owned : 
                UpgradeItemUIState.Default;
            _view.SetBackgroundColor(uiState);
            
            // Cost color
            _view.SetCostTextColor(state.CanAfford);
            
            // Purchase button
            _view.SetPurchaseButtonState(!state.IsUnlocked, state.CanUnlock);
            
            // Equip button
            var equipButtonText = state.IsEquipped ? "UNEQUIP" : "EQUIP";
            _view.SetEquipButtonState(state.IsUnlocked, true, equipButtonText);
        }

        
        private string FormatCurrency(Currency cost)
        {
            return cost.moneyType switch
            {
                MoneyType.Dronix => $"₯{cost.amount:N0}",
                _ => $"${cost.amount:N0}"
            };
        }
    }
}