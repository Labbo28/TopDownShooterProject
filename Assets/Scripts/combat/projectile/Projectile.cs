using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] public ProjectileSO projectileSO;
    
    void Start()
    {
        //il proiettile viene distrutto quando il time to live arriva a zero
        Destroy(gameObject,projectileSO.lifeTime );
    }
    
    public float Damage 
    { 
        get 
        {
            float baseDamage = projectileSO.damage;
            
            // Applica i modificatori del danno se disponibili
            if (Player.Instance != null)
            {
                RangedWeaponStatsModifier modifier = Player.Instance.GetComponent<RangedWeaponStatsModifier>();
                if (modifier != null)
                {
                    baseDamage *= modifier.DamageMultiplier;
                }
            }
            
            return baseDamage;
        }
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * (projectileSO.speed * Time.deltaTime));
    }
}
