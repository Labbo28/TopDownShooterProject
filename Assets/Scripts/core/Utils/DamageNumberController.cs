using UnityEngine;


/// <summary>
/// Controller singleton per gestire la creazione di numeri di danno fluttuanti.
/// Permette di istanziare numeri di danno sopra entità danneggiate, con valori e colori appropriati.
/// </summary>
public class DamageNumberController : MonoBehaviour
{
    /// Singleton instance
    public static DamageNumberController Instance;
    /// Prefab del numero di danno da istanziare
    public DamageNumber prefab;


    /// <summary>
    /// Inizializza l'istanza singleton.
    /// </summary>
    private void Awake()
    {   // Implementazione del pattern Singleton per assicurarsi che ci sia una sola istanza di questo controller
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    /// <summary>
    /// Crea un numero di danno alla posizione specificata con il valore e il colore appropriati.
    /// </summary>
    public void CreateNumber(float value, Vector3 location, bool isPlayer)
    {
        DamageNumber damageNumber = Instantiate(prefab, location, transform.rotation, transform);
        damageNumber.SetValue(Mathf.RoundToInt(value), isPlayer); // Imposta il valore e il colore del numero di danno e usa Mathf.RoundToInt per arrotondare il valore float a int
    }
}
