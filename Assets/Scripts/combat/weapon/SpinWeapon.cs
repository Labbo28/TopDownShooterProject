using UnityEngine;

/// <summary>
/// SpinWeapon gestisce un'arma che ruota attorno al giocatore.
/// Crea più prefab che orbitano attorno al centro, aggiornando il loro numero se la proprietà 'amount' cambia.
/// </summary>
public class SpinWeapon : Weapon
{
    [SerializeField] private GameObject prefab; // Prefab da istanziare per ogni "lama" rotante
    [SerializeField] public float speed = 1.5f; // Velocità di rotazione delle lame
    [SerializeField] public float range = 2f; // Distanza dal centro (giocatore)
    [SerializeField] public int amount = 3; // Numero di lame rotanti
    [SerializeField] public float weaponDamage = 10f; // Danno inflitto da ogni lama

    private int lastAmount = -1; // Tiene traccia dell'ultimo valore di 'amount' per aggiornare i prefab solo quando necessario

    /// <summary>
    /// Aggiorna i prefab se il numero di lame ('amount') cambia.
    /// Distrugge i prefab esistenti e li ricrea con la nuova quantità.
    /// </summary>
    void Update()
    {
        // Se amount è cambiato, aggiorna i prefab
        if (amount != lastAmount)
        {
            // Distruggi tutti i figli esistenti (le vecchie lame)
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            // Ricrea i prefab con il nuovo amount
            for (int i = 0; i < amount; i++)
            {
                // Calcola la rotazione iniziale per distribuire le lame in modo uniforme
                float rotation = 360f / amount * i;
                var obj = Instantiate(prefab, transform.position, Quaternion.identity, transform);
                var spinPrefab = obj.GetComponent<SpinWeaponPrefab>();
                spinPrefab.Init(rotation); // Inizializza la lama con la rotazione calcolata
                spinPrefab.weapon = this; // Passa il riferimento all'arma per accedere ai parametri
            }

            lastAmount = amount; // Aggiorna il valore di controllo
        }
    }

    /// <summary>
    /// Override del metodo Shoot. Non viene usato perché la rotazione è gestita in modo automatico.
    /// </summary>
    protected override void Shoot() { }
}
