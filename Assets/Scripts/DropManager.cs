using UnityEngine;

using System;
using System.Collections.Generic;
using Unity.VisualScripting;


public class DropManager : MonoBehaviour
{  
    public static DropManager Instance { get; private set; }
    [System.Serializable]


    public class EnemyDropConfig
    {
        public EnemyType enemyType;
        [Range(0f, 1f)]
        public float dropChance = 1f;
        public int minDrops = 1;
        public int maxDrops = 3;
    }

    // Contiene i 3 tipi di XP drop
    [SerializeField] private GameObject[] XPDrops;
    
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
    //se dimentichiamo di registrare un nemico siamo cucinati
    public void RegisterEnemy(EnemyBase enemy)
    {
        // Rimuovo il listener se già presente per evitare duplicati
        enemy.OnEnemyDead -= HandleEnemyDeath; 
        enemy.OnEnemyDead += HandleEnemyDeath; 
    }

    // Gestisce l'evento di morte del nemico
    private void HandleEnemyDeath(object sender, EventArgs e)
    {
        // Cast degli EventArgs al tipo specifico
        if (e is EnemyBase.EnemyDeadEventArgs args)
        {
            EnemyType enemyType = args.EnemyType;
            Vector3 dropPosition = args.Position;
            
            // Trova la configurazione per questo tipo di nemico
            EnemyDropConfig config = enemyDropConfigs.Find(c => c.enemyType == enemyType);
            
            // Se non c'è una configurazione specifica, usa valori predefiniti o esci
            if (config == null) return;
            
            // Determina se deve droppare qualcosa
            if (UnityEngine.Random.value <= config.dropChance)
            {
                // Determina quanti drop generare
                int numDrops = UnityEngine.Random.Range(config.minDrops, config.maxDrops + 1);
                
                for (int i = 0; i < numDrops; i++)
                {
                    // Scegli un tipo di XP drop in base al tipo di nemico
                    int dropIndex = GetDropIndexForEnemyType(enemyType);
                    
                    // Aggiungi una leggera variazione alla posizione per evitare sovrapposizioni
                    Vector3 offset = new Vector3(
                        UnityEngine.Random.Range(-0.5f, 0.5f),
                        UnityEngine.Random.Range(-0.5f, 0.5f),
                        0f
                    );
                    
                    // Crea il drop
                    Instantiate(XPDrops[dropIndex], dropPosition + offset, Quaternion.identity);
                }
            }
        }
    }

    // Determina quale tipo di XP drop usare in base al tipo di nemico
    private int GetDropIndexForEnemyType(EnemyType enemyType)
    {
        // Logica per decidere quale XP drop usare in base al tipo di nemico
        switch (enemyType)
        {
            case EnemyType.Melee:
                return 0; // XP di bronzo
            case EnemyType.Ranged:
                return UnityEngine.Random.Range(0, 2); // XP di bronzo o argento
            case EnemyType.Sniper:
                return 1; // XP d'argento 
            case EnemyType.Boss:
                return 2; // XP d'oro 
            default:
                return 0;
        }
    }

    // Importante: rimuovi i listener quando il DropManager viene distrutto
    private void OnDestroy()
    {
        EnemyBase[] enemies = FindObjectsOfType<EnemyBase>();
        foreach (EnemyBase enemy in enemies)
        {
            enemy.OnEnemyDead -= HandleEnemyDeath;
        }
    }
}
