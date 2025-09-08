using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Prefab che gestisce l'arma ad area.
/// Si occupa di:
/// - Applicare un effetto di crescita/riduzione della scala in base al raggio dell'arma.
/// - Rilevare i nemici all'interno dell'area tramite trigger collider.
/// - Infliggere danni periodici ai nemici presenti nella zona.
/// - Distruggere se stesso al termine della durata.
/// </summary>
public class AreaWeaponPrefab : MonoBehaviour
{
    /// <summary>
    /// Riferimento al componente AreaWeapon da cui vengono letti i parametri.
    /// </summary>
    public AreaWeapon weapon;

    /// <summary>
    /// Dimensione target dell'area in base al raggio dell'arma.
    /// </summary>
    private Vector3 targetSize;

    /// <summary>
    /// Lista dei nemici attualmente all'interno dell'area di effetto.
    /// </summary>
    public List<EnemyBase> enemiesInRange;

    /// <summary>
    /// Timer che gestisce la durata complessiva dell'arma.
    /// </summary>
    private float timer;

    /// <summary>
    /// Contatore per gestire il danno periodico.
    /// </summary>
    private float counter;

    // Inizializzazione
    void Start()
    {
        // Recupera il componente AreaWeapon dalla scena
        weapon = GameObject.Find("Area Weapon").GetComponent<AreaWeapon>();

        // Imposta la dimensione target in base al range
        targetSize = Vector3.one * weapon.range;

        // L'effetto parte da scala zero (espansione graduale)
        transform.localScale = Vector3.zero;

        // Imposta i timer in base alla durata dell'arma
        timer = weapon.duration;
        counter = weapon.duration;
    }

    // Aggiornamento per frame
    void Update()
    {
        // Crescita verso la dimensione target
        transform.localScale = Vector3.MoveTowards(transform.localScale, targetSize, Time.deltaTime * 10);
        //quando almeno un nemico è dentro la trappola essa iuncomincia a roteare 
        if (enemiesInRange.Count > 0)
        {
            transform.Rotate(0, 0, weapon.rotationSpeed *300 * Time.deltaTime);
        }
        // Riduzione del timer principale
        timer -= Time.deltaTime;

        // Quando scade la durata, l'area inizia a ridursi fino a scomparire
        if (timer <= 0)
        {
            targetSize = Vector3.zero;

            // Se la scala è tornata a zero, distrugge il prefab
            if (transform.localScale.x == 0f)
            {
                Destroy(gameObject);
            }
        }

        // Gestione del danno periodico
        counter -= Time.deltaTime*weapon.rotationSpeed; //la velocità di rotazione influisce sulla frequenza dei danni
        if (counter <= 0)
        {
            // Infligge danno a tutti i nemici presenti nella lista
            for (int i = 0; i < enemiesInRange.Count; i++)
            {
                enemiesInRange[i].GetComponent<IDamageable>().TakeDamage(weapon.weaponDamage);
            }
            // Reset del counter (altrimenti i danni vengono inflitti ogni frame)
            counter = weapon.duration;
        }
    }

    /// <summary>
    /// Aggiunge i nemici che entrano nell'area e infligge subito un primo danno.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            enemiesInRange.Add(collider.GetComponent<EnemyBase>());

            // Se il nemico implementa IDamageable, infligge subito danno
            if (collider.TryGetComponent<IDamageable>(out var target))
            {
                target.TakeDamage(weapon.weaponDamage);
            }
        }
    }

    /// <summary>
    /// Rimuove i nemici che escono dall'area.
    /// </summary>
    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Enemy"))
        {
            enemiesInRange.Remove(collider.GetComponent<EnemyBase>());
        }
    }
}
