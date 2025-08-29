using UnityEngine;

public class AreaWeapon : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    private float spawnCounter;
    public float weaponDamage = 10f;
    public float range = 0.2f;

    public float cooldown = 5f;
    public float duration = 3f;
    // Update is called once per frame
    void Update()
    {
        spawnCounter -= Time.deltaTime;
        if (spawnCounter <= 0)
        {
            spawnCounter = cooldown;
            Instantiate(prefab,transform.position,transform.rotation);// se si aggiunge trasform come quarto argometo alla fine del metodo l'arma da area segue il giocatore
        }
    }
}
