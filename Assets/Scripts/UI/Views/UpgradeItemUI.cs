using GameFlow.PlayerProgression;
using GameFlow.Upgrade.Base;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class UpgradeItemUI : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Button purchaseButton;
        [SerializeField] private Button equipButton;
        [SerializeField] private Image iconImage;
        [SerializeField] private GameObject lockedOverlay;
        [SerializeField] private GameObject ownedIndicator;
        [SerializeField] private GameObject equippedIndicator;
    
        private BaseUpgradeData _upgrade;
        private IPlayerProgressService _playerProgress;
    
        public void Initialize(BaseUpgradeData upgrade, IPlayerProgressService playerProgress)
        {
            _upgrade = upgrade;
            _playerProgress = playerProgress;
        
            // Set static UI elements
            nameText.text = upgrade.upgradeName;
            descriptionText.text = upgrade.description;
            costText.text = $"${upgrade.cost.amount}";
        
            if (upgrade.icon != null)
                iconImage.sprite = upgrade.icon;
        
            // Set up buttons
            purchaseButton.onClick.AddListener(OnPurchaseClicked);
            equipButton.onClick.AddListener(OnEquipClicked);
        
            RefreshState();
        }
    
        public void RefreshState()
        {
            bool isUnlocked = _playerProgress.IsUpgradeUnlocked(_upgrade);
            bool canAfford = _playerProgress.CanAfford(_upgrade.cost);
            bool canUnlock = _playerProgress.CanUnlockUpgrade(_upgrade);
            bool isEquipped = _playerProgress.IsUpgradeEquipped(_upgrade);
        
            // Update UI state
            lockedOverlay.SetActive(!isUnlocked);
            ownedIndicator.SetActive(isUnlocked);
            equippedIndicator.SetActive(isEquipped);
        
            // Update buttons
            purchaseButton.gameObject.SetActive(!isUnlocked);
            purchaseButton.interactable = canUnlock;
        
            equipButton.gameObject.SetActive(isUnlocked);
            equipButton.interactable = !isEquipped;
            equipButton.GetComponentInChildren<TextMeshProUGUI>().text = isEquipped ? "EQUIPPED" : "EQUIP";
        
            // Update cost color
            costText.color = canAfford ? Color.white : Color.red;
        }
    
        private void OnPurchaseClicked()
        {
            if (_playerProgress.UnlockUpgrade(_upgrade))
            {
                Debug.Log($"Purchased upgrade: {_upgrade.upgradeName}");
                // Optional: Play purchase sound/animation
            }
            else
            {
                Debug.Log("Cannot purchase upgrade!");
                // Optional: Show error message
            }
        }
    
        private void OnEquipClicked()
        {
            if (_playerProgress.IsUpgradeEquipped(_upgrade))
            {
                _playerProgress.UnequipUpgrade(_upgrade);
            }
            else
            {
                _playerProgress.EquipUpgrade(_upgrade);
            }
        
            RefreshState();
        }
    }
}