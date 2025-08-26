using UnityEngine;

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

    private void GenerateMapChunk(int x, int y)
    {
        Vector3 spawnPosistion = new Vector3(x, y) * mapChunkSize;
        Instantiate(mapChunkPrefab, spawnPosistion, Quaternion.identity, transform);
    }

}
