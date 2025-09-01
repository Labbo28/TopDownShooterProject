using TMPro;
using UnityEngine;


/// <summary>
/// Visualizza un numero di danno fluttuante sopra un'entità.
/// Gestisce la posizione, il colore e la durata della visualizzazione.
/// </summary>
public class DamageNumber : MonoBehaviour
{
    // Riferimento al componente TMP_Text per visualizzare il numero di danno
    [SerializeField] private TMP_Text damageText;

    // Velocità di salita del numero di danno
    private float floatSpeed = 0.5f;


    /// <summary>
    /// Inizializza il numero di danno con un offset casuale e programma la distruzione dell'oggetto dopo 0.5 secondi.
    /// </summary>
    void Start()
    {
        // Offset casuale per evitare sovrapposizione
        float xOffset = Random.Range(-0.3f, 0.3f);
        float yOffset = Random.Range(-0.1f, 0.1f);
        transform.position += new Vector3(xOffset, yOffset, 0);

        Destroy(gameObject, 0.5f);
    }


    /// <summary>
    /// Aggiorna la posizione del numero di danno per farlo salire lentamente.
    /// </summary>
    void Update()
    {
        transform.position += Vector3.up * Time.deltaTime * floatSpeed;
    }

    /// <summary>
    /// Imposta il valore del danno e il colore in base al tipo di entità (giocatore o nemico).
    /// </summary>
    /// <param name="value">Valore del danno da visualizzare.</param>
    /// <param name="isPlayer">True se l'entità è il giocatore, false se è un nemico.</param>
    public void SetValue(int value, bool isPlayer)
    {
        damageText.text = value.ToString();
        damageText.color = isPlayer ? Color.red : Color.white;// Rosso per il giocatore, bianco per i nemici
    }
}
