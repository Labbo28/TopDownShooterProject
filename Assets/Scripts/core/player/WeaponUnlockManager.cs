using UnityEngine;
using System.Collections.Generic;

public class WeaponUnlockManager : MonoBehaviour
{
    [SerializeField] private List<string> unlockedWeapons = new List<string>();
    
    // Define the main weapon that's unlocked by default
    private const string MainWeapon = "Assault_Rifle";
    
    private void Awake()
    {
        // Ensure main weapon is unlocked by default
        if (!unlockedWeapons.Contains(MainWeapon))
        {
            unlockedWeapons.Add(MainWeapon);
        }
    }
    
    private void Start()
    {
        // Apply initial weapon states
        UpdateWeaponStates();
    }
    
    public bool IsWeaponUnlocked(string weaponName)
    {
        return unlockedWeapons.Contains(weaponName);
    }
    
    public void UnlockWeapon(string weaponName)
    {
        if (!unlockedWeapons.Contains(weaponName))
        {
            unlockedWeapons.Add(weaponName);
            UpdateWeaponStates();
            Debug.Log($"Weapon unlocked: {weaponName}");
        }
    }
    
    private void UpdateWeaponStates()
    {
        // Find all weapon components in the Player's Weapons GameObject
        Transform weaponsParent = transform.Find("Weapons");
        if (weaponsParent == null) return;
        
        Weapon[] allWeapons = weaponsParent.GetComponentsInChildren<Weapon>(true);
        
        foreach (Weapon weapon in allWeapons)
        {
            string weaponType = weapon.GetType().Name;
            bool shouldBeActive = IsWeaponUnlocked(weaponType);
            
            // Enable/disable the weapon GameObject
            weapon.gameObject.SetActive(shouldBeActive);
        }
    }
    
    public void ResetUnlockedWeapons()
    {
        unlockedWeapons.Clear();
        unlockedWeapons.Add(MainWeapon);
        UpdateWeaponStates();
    }
}