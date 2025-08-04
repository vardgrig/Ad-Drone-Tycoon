using System.Collections.Generic;
using GameFlow.Currency;
using GameFlow.PlayerProgression;
using GameFlow.Upgrade.Base;
using UI.Views;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Controllers
{
    public class UpgradeShopUIController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform upgradeListParent;
        [SerializeField] private GameObject upgradeItemPrefab;
        [SerializeField] private Button refreshButton;
    
        private IPlayerProgressService _playerProgress;
        private IUpgradeDatabase _upgradeDatabase;
        private List<UpgradeItemUI> _upgradeItems = new List<UpgradeItemUI>();
    
        [Inject]
        public void Construct(IPlayerProgressService playerProgress, IUpgradeDatabase upgradeDatabase)
        {
            _playerProgress = playerProgress;
            _upgradeDatabase = upgradeDatabase;
        }
    
        private void Start()
        {
            _playerProgress.OnUpgradeUnlocked += RefreshUpgradeList;
            _playerProgress.OnMoneyChanged += RefreshUpgradeList;
        
            refreshButton.onClick.AddListener(RefreshUpgradeList);
        
            CreateUpgradeList();
        }
    
        private void OnDestroy()
        {
            if (_playerProgress != null)
            {
                _playerProgress.OnUpgradeUnlocked -= RefreshUpgradeList;
                _playerProgress.OnMoneyChanged -= RefreshUpgradeList;
            }
        }
    
        private void CreateUpgradeList()
        {
            var allUpgrades = _upgradeDatabase.GetAllUpgrades();
        
            foreach (var upgrade in allUpgrades)
            {
                var upgradeObj = Instantiate(upgradeItemPrefab, upgradeListParent);
                var upgradeItem = upgradeObj.GetComponent<UpgradeItemUI>();
            
                upgradeItem.Initialize(upgrade, _playerProgress);
                _upgradeItems.Add(upgradeItem);
            }
        
            RefreshUpgradeList();
        }
    
        private void RefreshUpgradeList()
        {
            foreach (var item in _upgradeItems)
            {
                item.RefreshState();
            }
        }
    
        private void RefreshUpgradeList(BaseUpgradeData upgrade) => RefreshUpgradeList();
        private void RefreshUpgradeList(Currency money) => RefreshUpgradeList();
    }
}