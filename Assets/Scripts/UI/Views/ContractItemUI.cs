using System.Collections;
using System.Collections.Generic;
using GameFlow.Contracts;
using GameFlow.PlayerProgression;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Views
{
    public class ContractItemUI : MonoBehaviour
    {
        [Header("UI References")] [SerializeField]
        private TextMeshProUGUI contractNameText;

        [SerializeField] private TextMeshProUGUI companyNameText;
        [SerializeField] private TextMeshProUGUI rewardText;
        [SerializeField] private TextMeshProUGUI timeLimitText;
        [SerializeField] private TextMeshProUGUI difficultyText;
        [SerializeField] private Image companyLogoImage;
        [SerializeField] private Button acceptButton;
        [SerializeField] private GameObject lockedOverlay;
        [SerializeField] private TextMeshProUGUI requirementsText;
        [SerializeField] private Slider difficultySlider;

        [Header("Visual Elements")] 
        [SerializeField] private Color availableColor = Color.white;

        [SerializeField] private Color unavailableColor = Color.gray;
        [SerializeField] private Image backgroundImage;

        private ContractData _contract;
        private IContractService _contractService;
        private IPlayerProgressService _playerProgress;

        public void Initialize(
            ContractData contract, 
            IContractService contractService,
            IPlayerProgressService playerProgress)
        {
            _contract = contract;
            _contractService = contractService;
            _playerProgress = playerProgress;

            SetupUI();
            RefreshState();
        }

        private void SetupUI()
        {
            // Set basic contract info
            contractNameText.text = _contract.contractName;
            companyNameText.text = _contract.company.companyName;
            rewardText.text = $"${_contract.baseReward}";
            timeLimitText.text = FormatTime(_contract.timeLimit);
            difficultyText.text = $"Difficulty: {_contract.routeDifficulty}/10";

            // Set difficulty slider
            difficultySlider.value = _contract.routeDifficulty / 10f;

            // Set company logo if available
            if (_contract.company.companyLogo != null)
            {
                companyLogoImage.sprite = _contract.company.companyLogo;
            }

            // Set company color theme
            backgroundImage.color = _contract.company.companyColor;

            // Setup requirements text
            SetupRequirementsText();

            // Setup button
            acceptButton.onClick.AddListener(OnAcceptClicked);
        }

        private void SetupRequirementsText()
        {
            if (_contract.requiredDroneType == null || _contract.requiredDroneType.Count == 0)
            {
                requirementsText.text = "No special requirements";
                return;
            }

            var requirements = new List<string>();
            foreach (var requirement in _contract.requiredDroneType)
            {
                requirements.Add(requirement.upgradeName);
            }

            requirementsText.text = "Requires: " + string.Join(", ", requirements);
        }

        public void RefreshState()
        {
            var canAccept = CanAcceptContract();
            var companyUnlocked = _playerProgress.IsCompanyUnlocked(_contract.company);

            // Update button state
            acceptButton.interactable = canAccept && companyUnlocked;
            acceptButton.GetComponentInChildren<TextMeshProUGUI>().text =
                !companyUnlocked ? "COMPANY LOCKED" :
                !canAccept ? "REQUIREMENTS NOT MET" :
                "ACCEPT CONTRACT";

            // Update visual state
            lockedOverlay.SetActive(!companyUnlocked);

            // Update colors based on availability
            var targetColor = (canAccept && companyUnlocked) ? availableColor : unavailableColor;
            SetUIElementsColor(targetColor);

            // Update requirements text color
            bool hasRequirements = HasRequiredUpgrades();
            requirementsText.color = hasRequirements ? Color.green : Color.red;
        }

        private bool CanAcceptContract()
        {
            // Check if player has required upgrades
            if (!HasRequiredUpgrades()) return false;

            // Check if company is unlocked
            if (!_playerProgress.IsCompanyUnlocked(_contract.company)) return false;

            // Check if player level is sufficient (if company has level requirements)
            if (_playerProgress.PlayerLevel < _contract.company.minimumPlayerLevel) return false;

            // Check if player already has an active contract
            if (_contractService.CurrentSession != null) return false;

            return true;
        }

        private bool HasRequiredUpgrades()
        {
            if (_contract.requiredDroneType == null || _contract.requiredDroneType.Count == 0)
                return true;

            foreach (var requiredUpgrade in _contract.requiredDroneType)
            {
                if (!_playerProgress.IsUpgradeUnlocked(requiredUpgrade))
                    return false;
            }

            return true;
        }

        private void SetUIElementsColor(Color color)
        {
            contractNameText.color = color;
            companyNameText.color = color;
            rewardText.color = color;
            timeLimitText.color = color;
            difficultyText.color = color;
        }

        private void OnAcceptClicked()
        {
            if (!CanAcceptContract())
            {
                ShowCannotAcceptMessage();
                return;
            }

            // Show confirmation dialog or accept directly
            ShowContractConfirmation();
        }

        private void ShowContractConfirmation()
        {
            // Option 1: Accept directly
            AcceptContract();

            // Option 2: Show confirmation dialog (implement if needed)
            // ShowConfirmationDialog("Accept this contract?", AcceptContract);
        }

        private void AcceptContract()
        {
            _contractService.AcceptContract(_contract);

            // Optional: Show acceptance feedback
            ShowContractAcceptedFeedback();

            // Optional: Close the contract UI or update it
            // GetComponentInParent<ContractUIController>()?.OnContractAccepted();
        }

        private void ShowCannotAcceptMessage()
        {
            string message = GetCannotAcceptReason();

            // Show a temporary message or tooltip
            Debug.Log($"Cannot accept contract: {message}");

            // Optional: Implement a tooltip or notification system
            // NotificationSystem.Show(message, NotificationType.Warning);
        }

        private string GetCannotAcceptReason()
        {
            if (_contractService.CurrentSession != null)
                return "You already have an active contract";

            if (!_playerProgress.IsCompanyUnlocked(_contract.company))
                return "Company not unlocked";

            if (_playerProgress.PlayerLevel < _contract.company.minimumPlayerLevel)
                return $"Requires player level {_contract.company.minimumPlayerLevel}";

            if (!HasRequiredUpgrades())
                return "Missing required drone upgrades";

            return "Unknown reason";
        }

        private void ShowContractAcceptedFeedback()
        {
            // Optional: Animate the button or show a success message
            StartCoroutine(AcceptedFeedbackAnimation());
        }

        private IEnumerator AcceptedFeedbackAnimation()
        {
            var originalColor = acceptButton.image.color;
            acceptButton.image.color = Color.green;
            acceptButton.GetComponentInChildren<TextMeshProUGUI>().text = "ACCEPTED!";

            yield return new WaitForSeconds(1f);

            acceptButton.image.color = originalColor;
            acceptButton.interactable = false;
        }

        private string FormatTime(float timeInMinutes)
        {
            if (timeInMinutes < 60f)
            {
                return $"{timeInMinutes:F0}m";
            }

            var hours = timeInMinutes / 60f;
            return $"{hours:F1}h";
        }

        private void OnDestroy()
        {
            if (acceptButton != null)
                acceptButton.onClick.RemoveListener(OnAcceptClicked);
        }
    }
}