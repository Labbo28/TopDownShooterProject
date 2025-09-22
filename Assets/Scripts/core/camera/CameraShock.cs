using UnityEngine;

/// <summary>
/// CameraShock permette di scuotere la camera principale quando l'oggetto con questo script
/// viene toccato da un oggetto con un certo tag oppure all'avvio della scena.
/// Da assegnare all'oggetto con Collider (impostato come Trigger).
/// </summary>
public class CameraShock : MonoBehaviour
{
    [Header("Camera Shake Settings")]
    [Tooltip("Camera principale da scuotere. Se lasciato vuoto, verrà usata Camera.main.\n" +
             "Assegna qui la camera che vuoi far tremare. Se non sai quale scegliere, lascia vuoto per usare la camera principale della scena.")]
    public Camera mainCamera; // Da collegare nell'Inspector

    [Tooltip("Durata della scossa in secondi.\n" +
             "Imposta per quanto tempo la camera dovrà tremare ogni volta che viene attivata la scossa.\n" +
             "Esempio: 0.3 significa che la scossa dura 0.3 secondi.")]
    public float shakeDuration = 0.3f;

    [Tooltip("Intensità della scossa.\n" +
             "Valore che determina quanto la camera si sposterà durante la scossa.\n" +
             "Valori bassi (es: 0.1) producono un tremolio leggero, valori alti (es: 0.5) un tremolio più forte.")]
    public float shakeMagnitude = 0.2f;

    [Tooltip("Se attivo, la camera trema all'avvio della scena.\n" +
             "Abilita questa opzione se vuoi che la camera tremi automaticamente appena parte la scena.")]
    public bool shakeOnStart = false;

    [Tooltip("Tag dell'oggetto che attiva la scossa quando entra in collisione.\n" +
             "Inserisci qui il tag dell'oggetto che deve far partire la scossa quando entra nel trigger (es: 'Player').\n" +
             "Assicurati che il tag sia scritto esattamente come quello assegnato all'oggetto nella scena.")]
    public string triggerTag = "Player"; // Tag dell'oggetto che attiva la scossa

    private Vector3 originalPos; // Posizione originale della camera
    private float shakeTimeRemaining = 0f; // Tempo residuo della scossa

    /// <summary>
    /// Inizializza la posizione originale della camera e avvia la scossa se richiesto.
    /// </summary>
    void Start()
    {
        // Se la camera non è assegnata, usa Camera.main
        if (mainCamera == null)
            mainCamera = Camera.main;

        // Salva la posizione originale della camera
        if (mainCamera != null)
            originalPos = mainCamera.transform.localPosition;

        // Se richiesto, avvia la scossa all'avvio
        if (shakeOnStart)
            StartShake();
    }

    /// <summary>
    /// Aggiorna la posizione della camera se la scossa è attiva.
    /// </summary>
    void Update()
    {
        // Se la scossa è attiva, applica un offset casuale alla posizione della camera
        if (shakeTimeRemaining > 0)
        {
            mainCamera.transform.localPosition = originalPos + Random.insideUnitSphere * shakeMagnitude;
            shakeTimeRemaining -= Time.deltaTime;

            // Quando la scossa termina, ripristina la posizione originale
            if (shakeTimeRemaining <= 0)
                mainCamera.transform.localPosition = originalPos;
        }
    }

    /// <summary>
    /// Avvia la scossa della camera.
    /// </summary>
    public void StartShake()
    {
        shakeTimeRemaining = shakeDuration;
    }

    /// <summary>
    /// Quando un oggetto con il tag specificato entra nel trigger 2D, avvia la scossa.
    /// </summary>
    /// <param name="other">Collider2D dell'oggetto entrato nel trigger</param>
    void OnTriggerEnter2D(Collider2D other)
    {
        // Controlla se il tag dell'oggetto corrisponde a quello impostato
        if (other.CompareTag(triggerTag))
        {
            StartShake();
        }
    }
}
