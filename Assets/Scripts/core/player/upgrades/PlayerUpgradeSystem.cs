using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUpgradeSystem : MonoBehaviour
{
    public static PlayerUpgradeSystem Instance { get; private set; }

    [SerializeField] private List<PlayerUpgrade> availableUpgrades;
    [SerializeField] private GameObject upgradeUIPanel;
    [SerializeField] private GameObject upgradeButtonPrefab;

    private List<PlayerUpgrade> currentUpgrades = new List<PlayerUpgrade>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Subscribe to level up event
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerLevelUp.AddListener(ShowUpgradeOptions);
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
            CreateUpgradeButton(upgrade,i);
        }
    }

    private void CreateUpgradeButton(PlayerUpgrade upgrade, int index)
    {
        // Implementa la creazione del bottone UI
        // Questo collegherebbe l'upgrade al sistema UI
    }

    public void SelectUpgrade(PlayerUpgrade upgrade)
    {
        upgrade.ApplyUpgrade(Player.Instance);
        currentUpgrades.Add(upgrade);

        // Hide upgrade panel
        upgradeUIPanel.SetActive(false);

        // Resume game
        Time.timeScale = 1f;
    }
}