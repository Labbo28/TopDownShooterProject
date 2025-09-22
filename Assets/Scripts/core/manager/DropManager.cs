using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public enum DropType
{
    XP,
    Medikit,
    Magnet,
    Chest
}

public class DropManager : MonoBehaviour
{  
    public static DropManager Instance { get; private set; }
    
    [System.Serializable]
    public class EnemyDropConfig
    {
        public EnemyType enemyType;
        [Header("XP Drop Settings")]
        [Range(0f, 1f)]
        public float xpDropChance = 1f;
        public int minXPDrops = 1;
        public int maxXPDrops = 3;
        
        [Header("Special Drop Settings")]
        [Range(0f, 1f)]
        public float specialDropChance = 0.1f;
        [Range(0f, 1f)]
        public float medikitWeight = 0.5f;
        [Range(0f, 1f)]
        public float magnetWeight = 0.5f;
        [Range(0f, 1f)]
        public float chestWeight = 0.05f; // Chest drops are rare
    }
    
    [System.Serializable]
    public class DropTypeConfig
    {
        public DropType dropType;
        public GameObject[] dropPrefabs;
    }

    // Configurazione dei drop prefabs per ogni tipo di drop
    [SerializeField] private List<DropTypeConfig> dropConfigs;
    
    // Configurazione dei drop per tipo di nemico
    [SerializeField] private List<EnemyDropConfig> enemyDropConfigs;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Trova tutti i nemici presenti nella scena all'avvio
        RegisterAllEnemies();
    }

    // Registra tutti i nemici presenti nella scena
    private void RegisterAllEnemies()
    {
        EnemyBase[] enemies = FindObjectsOfType<EnemyBase>();
        foreach (EnemyBase enemy in enemies)
        {
            RegisterEnemy(enemy);
        }
    }

    // Metodo per registrare un singolo nemico (utile per nemici spawnati dopo l'avvio)
    public void RegisterEnemy(EnemyBase enemy)
    {
        // Rimuovo il listener se già presente per evitare duplicati
        enemy.OnEnemyDead.RemoveListener(HandleEnemyDeath); 
        enemy.OnEnemyDead.AddListener(HandleEnemyDeath); 
    }

    // Gestisce l'evento di morte del nemico
    private void HandleEnemyDeath(EnemyType enemyType, Vector3 position)
    {
        // Trova la configurazione per questo tipo di nemico
        EnemyDropConfig config = enemyDropConfigs.Find(c => c.enemyType == enemyType);
        
        // Se non c'è una configurazione specifica, esci
        if (config == null) return;
        
        // Genera un drop speciale (medikit o magnete) con una piccola probabilità
        if (UnityEngine.Random.value <= config.specialDropChance)
        {
            // Decidi quale tipo di drop speciale generare
            DropType specialDropType = DetermineSpecialDropType(config);
            
            // Genera il drop speciale
            GenerateDrop(specialDropType, enemyType, position);
        }
        
        // Genera i normali drop di XP (indipendentemente dal drop speciale)
        if (UnityEngine.Random.value <= config.xpDropChance)
        {
            // Determina quanti XP drop generare
            int numXPDrops = UnityEngine.Random.Range(config.minXPDrops, config.maxXPDrops + 1);
            
            for (int i = 0; i < numXPDrops; i++)
            {
                GenerateDrop(DropType.XP, enemyType, position);
            }
        }
    }

    // Genera un singolo drop
    private void GenerateDrop(DropType dropType, EnemyType enemyType, Vector3 position)
    {
        // Ottieni la configurazione per questo tipo di drop
        DropTypeConfig dropConfig = dropConfigs.Find(c => c.dropType == dropType);
        if (dropConfig == null || dropConfig.dropPrefabs.Length == 0) return;
        
        // Scegli il prefab specifico da utilizzare
        int prefabIndex = GetPrefabIndexForDropType(dropType, enemyType);
        if (prefabIndex >= dropConfig.dropPrefabs.Length) prefabIndex = 0;
        
        // Aggiungi una leggera variazione alla posizione per evitare sovrapposizioni
        Vector3 offset = new Vector3(
            UnityEngine.Random.Range(-0.5f, 0.5f),
            UnityEngine.Random.Range(-0.5f, 0.5f),
            0f
        );
        
        // Crea il drop
        Instantiate(dropConfig.dropPrefabs[prefabIndex], position + offset, Quaternion.identity);
    }

    // Determina quale tipo di drop speciale generare in base alle probabilità configurate
    private DropType DetermineSpecialDropType(EnemyDropConfig config)
    {
        float totalWeight = config.medikitWeight + config.magnetWeight + config.chestWeight;
        float random = UnityEngine.Random.value;

        float medikitThreshold = config.medikitWeight / totalWeight;
        float magnetThreshold = medikitThreshold + (config.magnetWeight / totalWeight);

        // Determina il tipo di drop speciale
        if (random <= medikitThreshold)
        {
            return DropType.Medikit;
        }
        else if (random <= magnetThreshold)
        {
            if(Player.Instance.GetCollectedMagnets() >= Player.Instance.MaxMagnets)
            {
                return DropType.Chest; // Se il giocatore ha già il massimo dei magneti
            }
            return DropType.Magnet;
        }
        else
        {
            return DropType.Chest;
        }
    }

    // Determina quale prefab specifico usare per il tipo di drop e il tipo di nemico
    private int GetPrefabIndexForDropType(DropType dropType, EnemyType enemyType)
    {
        switch (dropType)
        {
            case DropType.XP:
                return GetXPDropIndexForEnemyType(enemyType);
            case DropType.Medikit:
                return 0; // Per ora c'è solo un tipo di medikit
            case DropType.Magnet:
                return 0; // Per ora c'è solo un tipo di magnete
            case DropType.Chest:
                return 0; // Per ora c'è solo un tipo di chest
            default:
                return 0;
        }
    }

    // Determina quale tipo di XP drop usare in base al tipo di nemico
    private int GetXPDropIndexForEnemyType(EnemyType enemyType)
    {
        // Logica per decidere quale XP drop usare in base al tipo di nemico
        switch (enemyType)
        {
            // Tipi generici
           
            case EnemyType.Ranged:
                return UnityEngine.Random.Range(1, 2); // XP di bronzo o argento
                
            // Tipi specifici di zombie
            case EnemyType.Zombie:
                return 0; // XP di bronzo o argento
            case EnemyType.ZombieFast:
                return UnityEngine.Random.Range(1,2); // XP d'argento (più veloce = più prezioso)
                
            // Tipi specifici di scheletri
            case EnemyType.Skeleton:
                return UnityEngine.Random.Range(0,1); // XP d'argento o oro
            case EnemyType.SkeletonBoss:
                return 2; // XP d'oro

            // Nuovo nemico TombstoneEnemy
            case EnemyType.Tombstone:
                return 2; // XP d'argento (nemico a distanza)

            default:
                return 0; // XP di bronzo come fallback
        }
    }

    /// <summary>
    /// Force spawn a chest drop at a specific position (useful for boss defeats, special events, etc.)
    /// </summary>
    public void SpawnChestDrop(Vector3 position)
    {
        GenerateDrop(DropType.Chest, EnemyType.SkeletonBoss, position);
    }

    /// <summary>
    /// Force spawn multiple chest drops (for special occasions)
    /// </summary>
    public void SpawnChestDrops(Vector3 centerPosition, int count, float spreadRadius = 2f)
    {
        for (int i = 0; i < count; i++)
        {
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * spreadRadius;
            Vector3 spawnPosition = centerPosition + new Vector3(randomOffset.x, randomOffset.y, 0);
            SpawnChestDrop(spawnPosition);
        }
    }

    // Importante: rimuovi i listener quando il DropManager viene distrutto
    private void OnDestroy()
    {
        EnemyBase[] enemies = FindObjectsOfType<EnemyBase>();
        foreach (EnemyBase enemy in enemies)
        {
            enemy.OnEnemyDead.RemoveListener(HandleEnemyDeath);
        }
    }
}