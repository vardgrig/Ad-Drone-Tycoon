using System.Collections.Generic;
using GameFlow.Contracts;
using GameFlow.PlayerProgression;
using GameFlow.Upgrade.Base;
using UI.Views;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Controllers
{
    public class ContractUIController : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private Transform contractListParent;
        [SerializeField] private GameObject contractItemPrefab;
        [SerializeField] private GameObject noContractsMessage;
        [SerializeField] private Button refreshButton;
    
        private IContractService _contractService;
        private IPlayerProgressService _playerProgress;
        private List<ContractItemUI> _contractItems = new List<ContractItemUI>();
    
        [Inject]
        public void Construct(IContractService contractService, IPlayerProgressService playerProgress)
        {
            _contractService = contractService;
            _playerProgress = playerProgress;
        }
    
        private void Start()
        {
            // Subscribe to events
            _contractService.OnContractAccepted += OnContractAccepted;
            _contractService.OnContractCompleted += OnContractCompleted;
            _playerProgress.OnUpgradeUnlocked += RefreshContractList;
            _playerProgress.OnLevelUp += RefreshContractList;
        
            if (refreshButton != null)
                refreshButton.onClick.AddListener(RefreshContractList);
        
            CreateContractList();
        }
    
        private void OnDestroy()
        {
            if (_contractService != null)
            {
                _contractService.OnContractAccepted -= OnContractAccepted;
                _contractService.OnContractCompleted -= OnContractCompleted;
            }
        
            if (_playerProgress != null)
            {
                _playerProgress.OnUpgradeUnlocked -= RefreshContractList;
                _playerProgress.OnLevelUp -= RefreshContractList;
            }
        }
    
        private void CreateContractList()
        {
            ClearContractList();
        
            var availableContracts = _contractService.AvailableContracts;
        
            if (availableContracts.Length == 0)
            {
                noContractsMessage?.SetActive(true);
                return;
            }
        
            noContractsMessage?.SetActive(false);
        
            foreach (var contract in availableContracts)
            {
                var contractObj = Instantiate(contractItemPrefab, contractListParent);
                var contractItem = contractObj.GetComponent<ContractItemUI>();
            
                contractItem.Initialize(contract, _contractService, _playerProgress);
                _contractItems.Add(contractItem);
            }
        }
    
        private void ClearContractList()
        {
            foreach (var item in _contractItems)
            {
                if (item != null)
                    Destroy(item.gameObject);
            }
            _contractItems.Clear();
        }
    
        private void RefreshContractList()
        {
            foreach (var item in _contractItems)
            {
                if (item != null)
                    item.RefreshState();
            }
        }
    
        private void RefreshContractList(BaseUpgradeData upgrade) => RefreshContractList();
        private void RefreshContractList(int level) => RefreshContractList();
    
        private void OnContractAccepted(ContractSession session)
        {
            Debug.Log($"Contract accepted: {session.ContractData.contractName}");
            RefreshContractList();
        
            // Optional: Close the contract UI or show active contract info
            // gameObject.SetActive(false);
        }
    
        private void OnContractCompleted(ContractSession session)
        {
            Debug.Log("Contract completed, refreshing available contracts");
            RefreshContractList();
        }
    }
}