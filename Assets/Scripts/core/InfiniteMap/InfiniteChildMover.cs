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

    /// <summary>
    /// Metodo chiamato una volta al momento dell’inizializzazione. Attualmente non contiene logica.
    /// </summary>
    void Start()
    {
        // Nessuna inizializzazione necessaria al momento
    }

    /// <summary>
    /// Metodo chiamato ogni frame da Unity.
    /// Controlla continuamente la distanza dei figli rispetto al giocatore e li riposiziona se necessario.
    /// </summary>
    void Update()
    {
        RepositionChildrenIfNeeded();
    }

    /// <summary>
    /// Cicla su tutti i figli del GameObject corrente e verifica se la loro distanza dal giocatore supera una soglia.
    /// Se sì, il figlio viene spostato lungo l’asse X e/o Y per simulare una mappa infinita.
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

            // Se la distanza lungo l’asse X supera la soglia
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

            // Se la distanza lungo l’asse Y supera la soglia
            if (Math.Abs(offsetFromPlayer.y) > threshold)
            {
                int direction = Math.Sign(offsetFromPlayer.y); // Determina la direzione del movimento
                float displacement = threshold * 2 * direction; // Calcola lo spostamento come per l'asse X

                // Applica lo spostamento lungo l'asse Y
                child.position += Vector3.up * displacement;
            }

            // Nota: non viene gestito l'asse Z, in quanto si assume un contesto 2D. Per ambienti 3D, estendere il controllo sull’asse Z.
        }
    }
}
