using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class PlayerUpgradeSystem : MonoBehaviour
{
    public static PlayerUpgradeSystem Instance { get; private set; }

    [SerializeField] private List<PlayerUpgrade> availableUpgrades;
    [SerializeField] private GameObject upgradeUIPanel;
    [SerializeField] private List<GameObject> upgradeChoices;

    // Ora uso RuntimeUpgrade invece di PlayerUpgrade
    private List<RuntimeUpgrade> runtimeUpgrades = new List<RuntimeUpgrade>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitializeRuntimeUpgrades();
    }

    private void InitializeRuntimeUpgrades()
    {
        runtimeUpgrades.Clear();
        foreach (var upgrade in availableUpgrades)
        {
            runtimeUpgrades.Add(new RuntimeUpgrade(upgrade));
        }
    }

    private void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerLevelUp.AddListener(ShowUpgradeOptions);
            GameManager.Instance.OnGameOver.AddListener(ResetUpgrades);
        }
    }

    private void ShowUpgradeOptions(int playerLevel)
    {
        Time.timeScale = 0f;

        if (upgradeUIPanel != null)
        {
            upgradeUIPanel.SetActive(true);
            PopulateUpgradeOptions();
        }
    }

    private void PopulateUpgradeOptions()
    {
        // Trova upgrade che possono essere potenziati
        List<RuntimeUpgrade> availableOptions = runtimeUpgrades.Where(u => u.CanUpgrade()).ToList();

        //chek aggiuntivo per evitare che vengano mostrati upgrade al max level
        for (int i = availableOptions.Count - 1; i >= 0; i--)
        {
            if (availableOptions[i].IsMaxLevel)
            {
                availableOptions.RemoveAt(i);
            }
        }

        // Nascondi tutti i choice UI prima di popolare
        foreach (var choice in upgradeChoices)
        {
            choice.SetActive(false);
        }

        // Seleziona random 3 upgrade
        int numChoices = Mathf.Min(3, availableOptions.Count);
        for (int i = 0; i < numChoices; i++)
        {
            int randomIndex = Random.Range(0, availableOptions.Count);
            RuntimeUpgrade upgrade = availableOptions[randomIndex];
            availableOptions.RemoveAt(randomIndex);

            CreateUpgradeButton(upgrade, i);
        }
    }

    private void CreateUpgradeButton(RuntimeUpgrade runtimeUpgrade, int index)
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

        if (image != null && runtimeUpgrade.Icon != null)
            image.sprite = runtimeUpgrade.Icon;

        if (nameTMP != null)
            nameTMP.text = $"{runtimeUpgrade.UpgradeName}  Lv.{runtimeUpgrade.CurrentLevel + 1}/{runtimeUpgrade.MaxLevel}";

        if (descTMP != null)
            descTMP.text = runtimeUpgrade.Description;

        var button = choiceGO.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => SelectUpgrade(runtimeUpgrade));
        }
    }

    public void SelectUpgrade(RuntimeUpgrade runtimeUpgrade)
    {
        runtimeUpgrade.ApplyUpgrade(Player.Instance);
        
        Debug.Log($"Selected Upgrade: {runtimeUpgrade.UpgradeName} to level {runtimeUpgrade.CurrentLevel}");
        Debug.Log($"Player Health after upgrade: {Player.Instance.GetComponent<HealthSystem>().MaxHealth}");

        upgradeUIPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void ResetUpgrades()
    {
        foreach (var runtimeUpgrade in runtimeUpgrades)
        {
            runtimeUpgrade.ResetLevel();
        }
    }

    // Metodi di utilità per accedere agli upgrade runtime
    public RuntimeUpgrade GetRuntimeUpgrade(string upgradeName)
    {
        return runtimeUpgrades.FirstOrDefault(u => u.UpgradeName == upgradeName);
    }

    public List<RuntimeUpgrade> GetAllRuntimeUpgrades()
    {
        return new List<RuntimeUpgrade>(runtimeUpgrades);
    }

    public int GetUpgradeLevel(string upgradeName)
    {
        var upgrade = GetRuntimeUpgrade(upgradeName);
        return upgrade?.CurrentLevel ?? 0;
    }
}