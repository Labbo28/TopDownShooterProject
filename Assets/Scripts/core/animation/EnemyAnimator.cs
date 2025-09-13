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
        }

        // Trova automaticamente l'EnemyBase se non è assegnato
        if (enemy == null)
        {
            enemy = GetComponent<EnemyBase>();
            if (enemy == null)
            {
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
        }
    }

    private void OnEnemyHit()
    {
        if (ValidateEnemyAnimator())
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
        }
    }

    private void OnEnemyDead(EnemyType enemyType, Vector3 position)
    {
        if (ValidateEnemyAnimator())
        {
            // Controlla se il parametro "Dead" esiste prima di usarlo
            foreach (var param in _enemyAnimator.parameters)
            {
                if (param.name == "Dead" && param.type == AnimatorControllerParameterType.Bool)
                {
                    _enemyAnimator.SetBool("Dead", true);
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
    
    private bool ValidateEnemyAnimator()
    {
        if (_enemyAnimator == null)
        {
            _enemyAnimator = GetComponent<Animator>();
        }
        
        if (_enemyAnimator == null)
        {
            return false;
        }
        
        if (_enemyAnimator.runtimeAnimatorController == null)
        {
            return false;
        }
        
        if (_enemyAnimator.parameters == null)
        {
            return false;
        }
        
        return true;
    }

    private void OnDestroy()
    {
        if (enemy != null)
        {
            enemy.OnEnemyhit.RemoveListener(OnEnemyHit);
            enemy.OnEnemyDead.RemoveListener(OnEnemyDead);
        }
    }

    
    
}