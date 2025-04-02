using System;
using UnityEngine;

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
        enemy.OnEnemyhit += OnEnemyHit;
        enemy.OnEnemyDead += OnEnemyDead;
      
    }

    private void OnEnemyHit(object sender, EventArgs e)
    {
        _enemyAnimator.SetTrigger("Hit");
    }
    private void OnEnemyDead(object sender, EventArgs e)
    {
        _enemyAnimator.SetBool("Dead", true);
        enemy.OnEnemyhit -= OnEnemyHit;
        enemy.OnEnemyDead -= OnEnemyDead;
        
    }

   
}