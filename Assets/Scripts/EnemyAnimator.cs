using System;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    private Animator _enemyAnimator;
    [SerializeField] private Enemy enemy;

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
        enemy.OnEnemyMoving += EnemyOnOnEnemyMoving;
        enemy.OnEnemyStopMoving += EnemyOnOnEnemyStopMoving;
        enemy.OnEnemyAttacking += EnemyOnOnEnemyAttacking;
    }

    private void EnemyOnOnEnemyMoving(object sender, EventArgs e)
    {
        if (_enemyAnimator != null)
        {
            _enemyAnimator.SetBool("IsWalking", true);
        }
    }

    private void EnemyOnOnEnemyStopMoving(object sender, EventArgs e)
    {
        if (_enemyAnimator != null)
        {
            _enemyAnimator.SetBool("IsWalking", false);
        }
    }

    private void EnemyOnOnEnemyAttacking(object sender, EventArgs e)
    {
        if (_enemyAnimator != null)
        {
            _enemyAnimator.SetTrigger("Attack"); 
        }
    }
}