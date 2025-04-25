using UnityEngine;


[ CreateAssetMenu(fileName = "Weapon", menuName = "ScriptableObjects/Weapon", order = 2)]
    public class WeaponSO : ScriptableObject
    {
        public ProjectileSO projectile;
        public int maxAmmo;
        public float fireRate;
        public float reloadTime;
        
        
    }
