using UnityEngine;

[System.Serializable]
public class RuntimeUpgrade
{
    [SerializeField] private PlayerUpgrade upgradeData;
    [SerializeField] private int currentLevel;

    public PlayerUpgrade UpgradeData => upgradeData;
    public int CurrentLevel => currentLevel;
    public string UpgradeName => upgradeData.upgradeName;
    public string Description => upgradeData.description;
    public Sprite Icon => upgradeData.icon;
    public int MaxLevel => upgradeData.maxLevel;
    public bool IsMaxLevel => currentLevel >= upgradeData.maxLevel;

    public RuntimeUpgrade(PlayerUpgrade upgrade)
    {
        upgradeData = upgrade;
        currentLevel = 0;
    }

    public bool CanUpgrade()
    {
        return !IsMaxLevel;
    }

    public void ApplyUpgrade(Player player)
    {
        if (!CanUpgrade()) return;

        upgradeData.ApplyUpgrade(player, currentLevel);
        currentLevel++;
    }

    public void ResetLevel()
    {
        currentLevel = 0;
    }
}