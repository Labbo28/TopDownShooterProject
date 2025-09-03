using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUpgradeSystem : MonoBehaviour
{
    public static PlayerUpgradeSystem Instance { get; private set; }

    [SerializeField] private List<PlayerUpgrade> availableUpgrades;
    [SerializeField] private GameObject upgradeUIPanel;
    [SerializeField] private List<GameObject> upgradeChoices;

    private List<PlayerUpgrade> currentUpgrades = new List<PlayerUpgrade>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
    }

    private void Start()
    {
        // Subscribe to level up event
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerLevelUp.AddListener(ShowUpgradeOptions);
            GameManager.Instance.OnGameOver.AddListener(ResetUpgrades);
        }
    }


    private void ShowUpgradeOptions(int playerLevel)
    {
        // Pause game
        Time.timeScale = 0f;

        // Show upgrade panel
        if (upgradeUIPanel != null)
        {
            upgradeUIPanel.SetActive(true);
            PopulateUpgradeOptions();
        }
    }

    private void PopulateUpgradeOptions()
    {
        // Show 3 random upgrades that aren't maxed
        List<PlayerUpgrade> availableOptions = availableUpgrades.FindAll(u => !u.IsMaxLevel);

        // Randomly select 3 upgrades
        for (int i = 0; i < Mathf.Min(3, availableOptions.Count); i++)
        {
            int randomIndex = Random.Range(0, availableOptions.Count);
            PlayerUpgrade upgrade = availableOptions[randomIndex];
            availableOptions.RemoveAt(randomIndex);

            // Create upgrade button UI
            CreateUpgradeButton(upgrade, i);
        }
    }

    private void CreateUpgradeButton(PlayerUpgrade upgrade, int index)
    {
        if (index < 0 || index >= upgradeChoices.Count)
            return;

        var choiceGO = upgradeChoices[index];
        choiceGO.SetActive(true);

        var image = choiceGO.transform.GetChild(0).GetComponent<UnityEngine.UI.Image>();
        var descTMP = choiceGO.transform.GetChild(1).GetComponent<TMPro.TextMeshProUGUI>();
        var nameTMP = choiceGO.transform.GetChild(2).GetComponent<TMPro.TextMeshProUGUI>();

        descTMP.fontSize = 20;
        nameTMP.fontSize = 25;
    

        if (image != null && upgrade.icon != null)
            image.sprite = upgrade.icon;

        if (nameTMP != null)
            nameTMP.text = $"{upgrade.upgradeName}  Lv.{upgrade.currentLevel + 1}/{upgrade.maxLevel}";

        if (descTMP != null)
            descTMP.text = upgrade.description;

        var button = choiceGO.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SelectUpgrade(upgrade));
        }
    }

    public void SelectUpgrade(PlayerUpgrade upgrade)
    {
        upgrade.ApplyUpgrade(Player.Instance);
        currentUpgrades.Add(upgrade);
        Debug.Log($"Selected Upgrade: {upgrade.upgradeName} to level {upgrade.currentLevel}");
        Debug.Log($"Player Health after upgrade: {Player.Instance.GetComponent<HealthSystem>().MaxHealth}");

        // Hide upgrade panel
        upgradeUIPanel.SetActive(false);

        // Resume game
        Time.timeScale = 1f;
    }
    
    private void ResetUpgrades()
    {
        currentUpgrades.Clear();
        foreach (var upgrade in availableUpgrades)
        {
            upgrade.currentLevel = 0;
        }
    }
}