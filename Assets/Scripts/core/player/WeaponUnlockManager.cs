using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;

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
            Debug.Log($"Weapon unlocked: {weaponName}");
            UpdateWeaponStates();
        }
        else
        {
            Debug.Log($"Weapon {weaponName} was already unlocked");
        }
    }
    

    
    private void UpdateWeaponStates()
    {
        // Find all weapon components in the Player - try Weapons folder first, then whole Player
        Transform weaponsParent = transform.Find("Weapons");

        Weapon[] allWeapons;
        if (weaponsParent != null)
        {
            // Use Weapons folder if it exists
            allWeapons = weaponsParent.GetComponentsInChildren<Weapon>(true);
        }
        else
        {
            // Fallback: search in entire Player hierarchy
            allWeapons = GetComponentsInChildren<Weapon>(true);
        }

        foreach (Weapon weapon in allWeapons)
        {
            string weaponType = weapon.GetType().Name;
            bool shouldBeActive = IsWeaponUnlocked(weaponType);

            // Enable/disable the weapon GameObject
            weapon.gameObject.SetActive(shouldBeActive);

            // Debug info
            Debug.Log($"WeaponUnlockManager: {weaponType} - Unlocked: {shouldBeActive}");
        }
    }
    
    public void ResetUnlockedWeapons()
    {
        unlockedWeapons.Clear();
        unlockedWeapons.Add(MainWeapon);
        UpdateWeaponStates();
    }

    public void DisableAllWeapons()
    {
        // Find all weapon components in the Player - try Weapons folder first, then whole Player
        Transform weaponsParent = transform.Find("Weapons");

        Weapon[] allWeapons;
        if (weaponsParent != null)
        {
            // Use Weapons folder if it exists
            allWeapons = weaponsParent.GetComponentsInChildren<Weapon>(true);
        }
        else
        {
            // Fallback: search in entire Player hierarchy
            allWeapons = GetComponentsInChildren<Weapon>(true);
        }

        foreach (Weapon weapon in allWeapons)
        {
            if (weapon != null)
            {
                weapon.gameObject.SetActive(false);
                Debug.Log($"WeaponUnlockManager: Disabled {weapon.GetType().Name}");
            }
        }
    }

    public void EnableUnlockedWeapons()
    {
        UpdateWeaponStates();
        Debug.Log("WeaponUnlockManager: Re-enabled all unlocked weapons");
    }
}