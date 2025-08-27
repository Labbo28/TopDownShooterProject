using System;
using UnityEngine;

/// <summary>
/// Questo componente gestisce il riposizionamento automatico degli oggetti figli del GameObject a cui è associato.
/// L'obiettivo è simulare un comportamento di "scrolling infinito", mantenendo i figli disposti attorno alla posizione del giocatore.
/// È particolarmente utile in giochi con mappe tile-based o ambienti modulari.
/// </summary>
public class InfiniteChildMover : MonoBehaviour
{
    // TODO: In futuro collegare questo sistema alla generazione di nemici, drop e armi per sincronizzare contenuti dinamici.

    [Header("Riferimenti")]
    [SerializeField] private Transform playerTransform; // Riferimento al Transform del giocatore, utilizzato per calcolare la distanza dai figli

    [Header("Parametri di Configurazione")]
    [SerializeField] private float chunkSize; // Dimensione di ciascun blocco o tile della mappa
    [SerializeField] private float distanceThreshold = 1.5f; // Fattore moltiplicatore che determina quanto il giocatore può allontanarsi prima di far "riapparire" un blocco

    [Header("Modalità Operativa")]
    [SerializeField] private bool repositionWholeObject = false; // Se true, riposiziona l'intero GameObject invece dei figli

    /// <summary>
    /// Metodo chiamato una volta al momento dell'inizializzazione.
    /// </summary>
    void Start()
    {
        // Se playerTransform non è assegnato nell'inspector, cerca il Player automaticamente
        if (playerTransform == null)
        {
            // Cerca prima usando il Player singleton
            if (Player.Instance != null)
            {
                playerTransform = Player.Instance.transform;
            }
            else
            {
                // Fallback: cerca un GameObject con tag "Player"
                GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
                if (playerObject != null)
                {
                    playerTransform = playerObject.transform;
                }
                else
                {
                    Debug.LogError("InfiniteChildMover: Impossibile trovare il Player! Assicurati che ci sia un GameObject con tag 'Player' nella scena.");
                }
            }
        }
    }

    public void setPlayerTransform(Transform player)
    {
        playerTransform = player;
    }

    /// <summary>
    /// Metodo chiamato ogni frame da Unity.
    /// Controlla continuamente la distanza rispetto al giocatore e riposiziona se necessario.
    /// </summary>
    void Update()
    {
        // Controlla se playerTransform è assegnato prima di procedere
        if (playerTransform == null)
        {
            return;
        }

        if (repositionWholeObject)
        {
            RepositionSingleIfNeeded();
        }
        else
        {
            RepositionChildrenIfNeeded();
        }
    }

    /// <summary>
    /// Cicla su tutti i figli del GameObject corrente e verifica se la loro distanza dal giocatore supera una soglia.
    /// Se sì, il figlio viene spostato lungo l'asse X e/o Y per simulare una mappa infinita.
    /// </summary>
    private void RepositionChildrenIfNeeded()
    {
        // Itera su ogni figlio dell'oggetto corrente
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i); // Ottiene il riferimento al figlio corrente

            // Calcola il vettore distanza tra il giocatore e il figlio
            Vector3 offsetFromPlayer = playerTransform.position - child.position;

            // Calcola la soglia effettiva di distanza oltre la quale è necessario riposizionare il blocco
            float threshold = chunkSize * distanceThreshold;

            // --- GESTIONE ASSE X ---

            // Se la distanza lungo l'asse X supera la soglia
            if (Math.Abs(offsetFromPlayer.x) > threshold)
            {
                // Determina la direzione del movimento (positiva o negativa) in base alla posizione relativa
                int direction = Math.Sign(offsetFromPlayer.x);

                // Calcola lo spostamento: moltiplica la soglia per 2 per compensare sia l'uscita che il rientro del tile
                float displacement = threshold * 2 * direction;

                // Applica lo spostamento lungo l'asse X
                child.position += Vector3.right * displacement;
            }

            // --- GESTIONE ASSE Y ---

            // Se la distanza lungo l'asse Y supera la soglia
            if (Math.Abs(offsetFromPlayer.y) > threshold)
            {
                int direction = Math.Sign(offsetFromPlayer.y); // Determina la direzione del movimento
                float displacement = threshold * 2 * direction; // Calcola lo spostamento come per l'asse X

                // Applica lo spostamento lungo l'asse Y
                child.position += Vector3.up * displacement;
            }

            // Nota: non viene gestito l'asse Z, in quanto si assume un contesto 2D. Per ambienti 3D, estendere il controllo sull'asse Z.
        }
    }

    /// <summary>
    /// Verifica se l'intero GameObject (a cui è attaccato questo componente) deve essere riposizionato 
    /// in base alla distanza dal giocatore. Se la distanza supera la soglia, sposta l'intero GameObject 
    /// per simulare una mappa infinita.
    /// </summary>
    private void RepositionSingleIfNeeded()
    {
        // Calcola il vettore distanza tra il giocatore e questo GameObject
        Vector3 offsetFromPlayer = playerTransform.position - transform.position;

        // Calcola la soglia effettiva di distanza oltre la quale è necessario riposizionare l'oggetto
        float threshold = chunkSize * distanceThreshold;

        // --- GESTIONE ASSE X ---

        // Se la distanza lungo l'asse X supera la soglia
        if (Math.Abs(offsetFromPlayer.x) > threshold)
        {
            // Determina la direzione del movimento (positiva o negativa) in base alla posizione relativa
            int direction = Math.Sign(offsetFromPlayer.x);

            // Calcola lo spostamento: moltiplica la soglia per 2 per compensare sia l'uscita che il rientro del tile
            float displacement = threshold * 2 * direction;

            // Applica lo spostamento lungo l'asse X all'intero GameObject
            transform.position += Vector3.right * displacement;
        }

        // --- GESTIONE ASSE Y ---

        // Se la distanza lungo l'asse Y supera la soglia
        if (Math.Abs(offsetFromPlayer.y) > threshold)
        {
            int direction = Math.Sign(offsetFromPlayer.y); // Determina la direzione del movimento
            float displacement = threshold * 2 * direction; // Calcola lo spostamento come per l'asse X

            // Applica lo spostamento lungo l'asse Y all'intero GameObject
            transform.position += Vector3.up * displacement;
        }

        // Nota: non viene gestito l'asse Z, in quanto si assume un contesto 2D. Per ambienti 3D, estendere il controllo sull'asse Z.
    }

    /// <summary>
    /// Versione specifica per riposizionare nemici dalla parte opposta per farli andare verso il player.
    /// Questo metodo riposiziona i nemici in modo più aggressivo per assicurare che vadano sempre verso il giocatore.
    /// </summary>
    public void RepositionEnemyTowardsPlayer()
    {
        if (playerTransform == null) return;

        // Calcola la distanza dal player
        float distanceFromPlayer = Vector3.Distance(transform.position, playerTransform.position);
        
        // Se il nemico è troppo lontano, riposizionalo dalla parte opposta del player
        if (distanceFromPlayer > chunkSize * distanceThreshold)
        {
            // Calcola la direzione dal player verso il nemico
            Vector3 directionFromPlayer = (transform.position - playerTransform.position).normalized;
            
            // Posiziona il nemico dalla parte opposta, a una distanza specifica dal player
            float repositionDistance = chunkSize * (distanceThreshold * 0.8f); // Leggermente più vicino della soglia
            Vector3 newPosition = playerTransform.position - directionFromPlayer * repositionDistance;
            
            // Applica la nuova posizione
            transform.position = newPosition;
            
            // Debug log per verificare il riposizionamento
            Debug.Log($"Enemy {gameObject.name} repositioned from distance {distanceFromPlayer:F2} to new position", this);
        }
    }

    // Metodo per debug - visualizza le soglie nell'editor
    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null) return;
        
        // Disegna la soglia di riposizionamento
        Gizmos.color = Color.yellow;
        float threshold = chunkSize * distanceThreshold;
        Gizmos.DrawWireCube(playerTransform.position, new Vector3(threshold * 2, threshold * 2, 0.1f));
        
        // Disegna la linea di connessione tra questo oggetto e il player
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, playerTransform.position);
    }
}