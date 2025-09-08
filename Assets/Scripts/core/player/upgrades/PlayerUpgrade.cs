using UnityEngine;

public abstract class PlayerUpgrade : ScriptableObject
{
    public string upgradeName;
    public string description;
    public Sprite icon;
    public int maxLevel = 5;
    public abstract void ApplyUpgrade(Player player, int currentLevel);
}