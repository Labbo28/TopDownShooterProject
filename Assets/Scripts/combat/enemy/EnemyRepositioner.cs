using UnityEngine;

/// <summary>
/// Componente specifico per riposizionare i nemici quando si allontanano troppo dal giocatore.
/// Questo assicura che i nemici vengano sempre riposizionati in modo da poter raggiungere il giocatore.
/// </summary>
public class EnemyRepositioner : MonoBehaviour
{
    [Header("Configurazione Riposizionamento")]
    [SerializeField] private float maxDistanceFromPlayer = 20f; // Distanza massima consentita dal player
    [SerializeField] private float repositionDistance = 15f;    // Distanza a cui riposizionare il nemico
    [SerializeField] private bool debugMode = false;           // Mostra debug info
    
    [Header("Riferimenti (Opzionale - se non assegnato viene trovato automaticamente)")]
    [SerializeField] private Transform playerTransform;
    
    private void Start()
    {
        // Trova automaticamente il player se non assegnato
        if (playerTransform == null)
        {
            FindPlayer();
        }
    }
    
    private void FindPlayer()
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
            }
        }
    }
    
    private void Update()
    {
        if (playerTransform == null)
        {
            return;
        }
        
        CheckAndRepositionIfNeeded();
    }
    
    /// <summary>
    /// Controlla se il nemico è troppo lontano dal player e lo riposiziona se necessario
    /// </summary>
    private void CheckAndRepositionIfNeeded()
    {
        float distanceFromPlayer = Vector3.Distance(transform.position, playerTransform.position);
        
        if (distanceFromPlayer > maxDistanceFromPlayer)
        {
            RepositionEnemyNearPlayer();
        }
    }
    
    /// <summary>
    /// Riposiziona il nemico in una posizione casuale attorno al player, 
    /// ma abbastanza vicina da poter raggiungere il giocatore
    /// </summary>
    private void RepositionEnemyNearPlayer()
    {
        // Genera un angolo casuale attorno al player
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        
        // Calcola la nuova posizione usando trigonometria
        Vector3 repositionOffset = new Vector3(
            Mathf.Cos(randomAngle) * repositionDistance,
            Mathf.Sin(randomAngle) * repositionDistance,
            0f
        );
        
        Vector3 newPosition = playerTransform.position + repositionOffset;
        
        // Assicurati che la nuova posizione non sia troppo vicina o troppo lontana
        float actualDistance = Vector3.Distance(newPosition, playerTransform.position);
        if (actualDistance < repositionDistance * 0.8f || actualDistance > repositionDistance * 1.2f)
        {
            // Normalizza la distanza se necessario
            Vector3 direction = (newPosition - playerTransform.position).normalized;
            newPosition = playerTransform.position + direction * repositionDistance;
        }
        
        // Applica la nuova posizione
        Vector3 oldPosition = transform.position;
        transform.position = newPosition;
        
        if (debugMode)
        {
        }
        
        // Opzionale: Resetta il target dell'AI del nemico se esiste
        ResetEnemyAI();
    }
    
    /// <summary>
    /// Resetta l'AI del nemico dopo il riposizionamento per assicurare comportamento corretto
    /// </summary>
    private void ResetEnemyAI()
    {
        // Se il nemico ha un componente EnemyBase, potremmo voler resettare alcuni stati
        EnemyBase enemyBase = GetComponent<EnemyBase>();
        if (enemyBase != null)
        {
            // Qui potresti aggiungere logica per resettare stati dell'AI se necessario
            // Ad esempio: enemyBase.ResetBehaviorState();
        }
        
        // Resetta la velocità del Rigidbody2D se presente
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
    }
    
    /// <summary>
    /// Metodo pubblico per forzare il riposizionamento (utile per testing o situazioni speciali)
    /// </summary>
    public void ForceReposition()
    {
        if (playerTransform != null)
        {
            RepositionEnemyNearPlayer();
        }
    }
    
    /// <summary>
    /// Imposta manualmente il riferimento al player
    /// </summary>
    public void SetPlayerTransform(Transform player)
    {
        playerTransform = player;
    }
    
    // Visualizzazione debug nell'editor
    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null) return;
        
        // Disegna la distanza massima
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(playerTransform.position, maxDistanceFromPlayer);
        
        // Disegna la distanza di riposizionamento
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(playerTransform.position, repositionDistance);
        
        // Disegna la linea tra nemico e player
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, playerTransform.position);
        
        // Mostra la distanza attuale
        float currentDistance = Vector3.Distance(transform.position, playerTransform.position);
        if (currentDistance > maxDistanceFromPlayer)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 1f);
        }
    }
}