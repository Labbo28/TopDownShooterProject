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
    }

    private void Start()
    {
        enemy.OnEnemyhit.AddListener(OnEnemyHit);
        enemy.OnEnemyDead.AddListener(OnEnemyDead);
    }

    private void OnEnemyHit()
    {
        _enemyAnimator.SetTrigger("Hit");
    }

    private void OnEnemyDead(EnemyType enemyType, Vector3 position)
    {
        _enemyAnimator.SetBool("Dead", true);
        enemy.OnEnemyhit.RemoveListener(OnEnemyHit);
        enemy.OnEnemyDead.RemoveListener(OnEnemyDead);
        Debug.Log("Nemico morto, animazione di morte attivata");
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