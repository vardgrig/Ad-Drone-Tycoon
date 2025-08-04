using System.Collections;
using GameFlow.Currency;
using GameFlow.PlayerProgression;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Controllers
{
    public class PlayerProgressUIController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI experienceText;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private Slider experienceBar;
    [SerializeField] private GameObject levelUpNotification;
    
    private IPlayerProgressService _playerProgress;
    
    [Inject]
    public void Construct(IPlayerProgressService playerProgress)
    {
        _playerProgress = playerProgress;
    }
    
    private void Start()
    {
        // Subscribe to events
        _playerProgress.OnLevelUp += HandleLevelUp;
        _playerProgress.OnExperienceGained += HandleExperienceGained;
        _playerProgress.OnMoneyChanged += HandleMoneyChanged;
        
        // Initialize UI with current values
        UpdateUI();
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from events
        if (_playerProgress != null)
        {
            _playerProgress.OnLevelUp -= HandleLevelUp;
            _playerProgress.OnExperienceGained -= HandleExperienceGained;
            _playerProgress.OnMoneyChanged -= HandleMoneyChanged;
        }
    }
    
    private void UpdateUI()
    {
        levelText.text = $"Level {_playerProgress.PlayerLevel}";
        moneyText.text = $"${_playerProgress.Money.amount}";
        
        UpdateExperienceBar();
    }
    
    private void UpdateExperienceBar()
    {
        var currentExp = _playerProgress.Experience;
        var requiredExp = _playerProgress.GetExperienceRequiredForLevel(_playerProgress.PlayerLevel + 1);
        var previousLevelExp = _playerProgress.GetExperienceRequiredForLevel(_playerProgress.PlayerLevel);
        
        var progress = (float)(currentExp - previousLevelExp) / (requiredExp - previousLevelExp);
        experienceBar.value = progress;
        
        experienceText.text = $"{currentExp - previousLevelExp} / {requiredExp - previousLevelExp} XP";
    }
    
    private void HandleLevelUp(int newLevel)
    {
        levelText.text = $"Level {newLevel}";
        UpdateExperienceBar();
        
        // Show level up notification
        ShowLevelUpNotification(newLevel);
    }
    
    private void HandleExperienceGained(int amount)
    {
        UpdateExperienceBar();
        
        // Optional: Show floating XP text
        ShowFloatingXP(amount);
    }
    
    private void HandleMoneyChanged(Currency newMoney)
    {
        moneyText.text = $"${newMoney.amount}";
    }
    
    private void ShowLevelUpNotification(int newLevel)
    {
        // Implement level up animation/notification
        StartCoroutine(LevelUpAnimation(newLevel));
    }
    
    private void ShowFloatingXP(int amount)
    {
        // Implement floating XP text animation
    }
    
    private IEnumerator LevelUpAnimation(int newLevel)
    {
        levelUpNotification.SetActive(true);
        levelUpNotification.GetComponent<TextMeshProUGUI>().text = $"LEVEL UP!\nLevel {newLevel}";
        
        yield return new WaitForSeconds(3f);
        
        levelUpNotification.SetActive(false);
    }
}
}