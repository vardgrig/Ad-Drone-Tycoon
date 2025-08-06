using Signals;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Views
{
    public class UpgradeItemUI : MonoBehaviour
    {
        [Header("UI References")] [SerializeField]
        private TextMeshProUGUI nameText;

        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Button purchaseButton;
        [SerializeField] private Button equipButton;
        [SerializeField] private Button infoButton;
        [SerializeField] private Image iconImage;
        [SerializeField] private GameObject lockedOverlay;
        [SerializeField] private GameObject ownedIndicator;
        [SerializeField] private GameObject equippedIndicator;
        [SerializeField] private Image backgroundImage;

        [Header("Visual States")] [SerializeField]
        private Color defaultBackgroundColor = Color.white;

        [SerializeField] private Color ownedBackgroundColor = new Color(0.2f, 0.8f, 0.2f, 0.3f);
        [SerializeField] private Color equippedBackgroundColor = new Color(0.2f, 0.2f, 0.8f, 0.3f);
        [SerializeField] private Color affordableTextColor = Color.white;
        [SerializeField] private Color cannotAffordTextColor = Color.red;

        private SignalBus _signalBus;
        private string UpgradeId { get; set; }

        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private void Awake()
        {
            purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
            equipButton.onClick.AddListener(OnEquipButtonClicked);
            if (infoButton != null)
                infoButton.onClick.AddListener(OnInfoButtonClicked);
        }

        private void OnPurchaseButtonClicked() => _signalBus.TryFire(new PurchaseUpgradeSignal(UpgradeId));
        private void OnEquipButtonClicked() => _signalBus.TryFire(new EquipUpgradeSignal(UpgradeId));
        private void OnInfoButtonClicked() => _signalBus.TryFire(new ShowUpgradeInfoSignal(UpgradeId));


        // Called by controller to set data
        public void SetUpgradeId(string upgradeId) => UpgradeId = upgradeId;

        public void SetName(string text) => nameText.text = text;
        public void SetDescription(string text) => descriptionText.text = text;
        public void SetCost(string text) => costText.text = text;
        public void SetIcon(Sprite sprite) => iconImage.sprite = sprite;

        public void SetCostTextColor(bool canAfford)
        {
            costText.color = canAfford ? affordableTextColor : cannotAffordTextColor;
        }

        public void SetBackgroundColor(UpgradeItemUIState state)
        {
            if (!backgroundImage) return;

            backgroundImage.color = state switch
            {
                UpgradeItemUIState.Equipped => equippedBackgroundColor,
                UpgradeItemUIState.Owned => ownedBackgroundColor,
                _ => defaultBackgroundColor
            };
        }

        public void SetLockedOverlay(bool active) => lockedOverlay.SetActive(active);
        public void SetOwnedIndicator(bool active) => ownedIndicator.SetActive(active);
        public void SetEquippedIndicator(bool active) => equippedIndicator.SetActive(active);

        public void SetPurchaseButtonState(bool active, bool interactable)
        {
            purchaseButton.gameObject.SetActive(active);
            purchaseButton.interactable = interactable;
        }

        public void SetEquipButtonState(bool active, bool interactable, string text)
        {
            equipButton.gameObject.SetActive(active);
            equipButton.interactable = interactable;
            var buttonText = equipButton.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText) 
                buttonText.text = text;
        }

        private void OnDestroy()
        {
            purchaseButton.onClick.RemoveAllListeners();
            equipButton.onClick.RemoveAllListeners();
            if (infoButton) infoButton.onClick.RemoveAllListeners();
        }
    }

    public enum UpgradeItemUIState
    {
        Default,
        Owned,
        Equipped
    }
}