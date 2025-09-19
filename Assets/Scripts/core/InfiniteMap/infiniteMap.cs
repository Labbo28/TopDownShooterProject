using UnityEngine;
using UnityEngine.SceneManagement;
public class infiniteMap : MonoBehaviour
{
    [Header("Element")]
    [SerializeField] private GameObject mapChunkPrefab;

    [Header("Settings")]
    [SerializeField] private float mapChunkSize;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        GenerateMap();
        
    }

    /// <summary>
    /// Genera una griglia iniziale di chunk disposti intorno all'origine.
    /// </summary>
    private void GenerateMap()
    {
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                GenerateMapChunk(x, y);
            }
        }
    }
  

    /// <summary>
    /// Genera un singolo chunk della mappa in base agli indici della griglia.
    /// </summary>
    /// <param name="gridX">Posizione X nella griglia</param>
    /// <param name="gridY">Posizione Y nella griglia</param>
    private void GenerateMapChunk(int x, int y)
    {
        // Calcola la posizione del blocco moltiplicando gli indici della griglia per la dimensione del chunk
        Vector3 spawnPosistion = new Vector3(x, y) * mapChunkSize;
        // Instanzia il prefab del chunk nella scena, con rotazione nulla e come figlio del MapManager
        Instantiate(mapChunkPrefab, spawnPosistion, Quaternion.identity, transform);
    }

}
