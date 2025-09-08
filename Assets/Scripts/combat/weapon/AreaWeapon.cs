using UnityEngine;

/// <summary>
/// Questa classe gestisce la generazione periodica di un'arma ad area.
/// L'arma viene istanziata sotto forma di prefab alla posizione dell'entità che possiede questo script.
/// </summary>
public class AreaWeapon : MonoBehaviour
{
    // Prefab da istanziare che rappresenta l'arma ad area
    [SerializeField] private GameObject prefab;

    [SerializeField] public float rotationSpeed = 1f;

    // Contatore interno che gestisce il tempo di cooldown tra un'istanza e l'altra
    private float spawnCounter;

    // Danno inflitto dall'arma ad area
    public float weaponDamage = 10f;

    // Raggio di efficacia dell'arma ad area
    public float range = 0.2f;

    // Tempo di ricarica tra un'attivazione e l'altra
    public float cooldown = 5f;

    // Durata dell'effetto dell'arma (non utilizzata nel codice corrente)
    public float duration = 3f;

    /// <summary>
    /// Metodo chiamato ad ogni frame. Gestisce il countdown per l'attivazione dell'arma.
    /// </summary>
    void Update()
    {
        // Riduce il contatore in base al tempo trascorso
        spawnCounter -= Time.deltaTime;

        // Se il contatore raggiunge o scende sotto lo zero, viene istanziato il prefab
        if (spawnCounter <= 0)
        {
            // Reset del contatore
            spawnCounter = cooldown;

            // Istanzia il prefab alla posizione e rotazione attuali dell'oggetto
            // Se si volesse far seguire il prefab al giocatore, aggiungere 'transform' come quarto argomento
            Instantiate(prefab, transform.position, transform.rotation);
        }
    }
}
