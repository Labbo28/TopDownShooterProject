using System;
using UnityEngine;
using UnityEngine.Events;

public class EnemyAnimator : MonoBehaviour
{
    private Animator _enemyAnimator;
    [SerializeField] private EnemyBase enemy;

    private void Awake()
    {
        if (TryGetComponent<Animator>(out Animator animator))
        {
            _enemyAnimator = animator;
        }
        else
        {
            Debug.LogWarning("Nessun Animator presente in EnemyPrefab");
        }

        // Trova automaticamente l'EnemyBase se non è assegnato
        if (enemy == null)
        {
            enemy = GetComponent<EnemyBase>();
            if (enemy == null)
            {
                Debug.LogError($"EnemyAnimator on {gameObject.name}: Impossibile trovare EnemyBase component!");
                return;
            }
        }
    }

    private void Start()
    {
        // Controlla che l'enemy sia valido prima di aggiungere i listener
        if (enemy != null)
        {
            enemy.OnEnemyhit.AddListener(OnEnemyHit);
            enemy.OnEnemyDead.AddListener(OnEnemyDead);
        }
        else
        {
            Debug.LogError($"EnemyAnimator on {gameObject.name}: Enemy reference is null!");
        }
    }

    private void OnEnemyHit()
    {
        if (_enemyAnimator != null && _enemyAnimator.parameters != null)
        {
            // Controlla se il parametro "Hit" esiste prima di usarlo
            foreach (var param in _enemyAnimator.parameters)
            {
                if (param.name == "Hit" && param.type == AnimatorControllerParameterType.Trigger)
                {
                    _enemyAnimator.SetTrigger("Hit");
                    return;
                }
            }
            Debug.LogWarning($"Parameter 'Hit' non trovato nell'Animator di {gameObject.name}");
        }
    }

    private void OnEnemyDead(EnemyType enemyType, Vector3 position)
    {
        if (_enemyAnimator != null && _enemyAnimator.parameters != null)
        {
            // Controlla se il parametro "Dead" esiste prima di usarlo
            foreach (var param in _enemyAnimator.parameters)
            {
                if (param.name == "Dead" && param.type == AnimatorControllerParameterType.Bool)
                {
                    _enemyAnimator.SetBool("Dead", true);
                    Debug.Log($"Nemico {gameObject.name} morto, animazione di morte attivata");
                    break;
                }
            }
        }
        
        // Rimuovi i listener per evitare errori
        if (enemy != null)
        {
            enemy.OnEnemyhit.RemoveListener(OnEnemyHit);
            enemy.OnEnemyDead.RemoveListener(OnEnemyDead);
        }
    }

    private void OnDestroy()
    {
        if (enemy != null)
        {
            enemy.OnEnemyhit.RemoveListener(OnEnemyHit);
            enemy.OnEnemyDead.RemoveListener(OnEnemyDead);
        }
    }

    // Metodo pubblico per verificare i parametri dell'animator (debug)
    [ContextMenu("Debug Animator Parameters")]
    public void DebugAnimatorParameters()
    {
        if (_enemyAnimator != null && _enemyAnimator.parameters != null)
        {
            Debug.Log($"Animator parameters for {gameObject.name}:");
            foreach (var param in _enemyAnimator.parameters)
            {
                Debug.Log($"- {param.name} ({param.type})");
            }
        }
    }
}