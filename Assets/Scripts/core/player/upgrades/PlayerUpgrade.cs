using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public abstract class PlayerUpgrade
{
    public string upgradeName;
    public string description;
    public Sprite icon;
    public int maxLevel = 5;
    public int currentLevel = 0;
    
    public bool IsMaxLevel => currentLevel >= maxLevel;

    public abstract void ApplyUpgrade(Player player);
   
}