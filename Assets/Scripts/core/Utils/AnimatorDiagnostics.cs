using UnityEngine;

/// <summary>
/// Utility per diagnosticare problemi con gli Animator Controllers
/// </summary>
public class AnimatorDiagnostics : MonoBehaviour
{
    [Header("Auto-Check Settings")]
    [SerializeField] private bool runDiagnosticsOnStart = true;
    [SerializeField] private bool logDetailedInfo = true;
    
    private void Start()
    {
        if (runDiagnosticsOnStart)
        {
            RunAnimatorDiagnostics();
        }
    }
    
    [ContextMenu("Run Animator Diagnostics")]
    public void RunAnimatorDiagnostics()
    {
        
        // Controlla tutti gli Animator nella scena
        Animator[] allAnimators = FindObjectsOfType<Animator>();
        
        foreach (Animator animator in allAnimators)
        {
            DiagnoseAnimator(animator);
        }
        
    }
    
    private void DiagnoseAnimator(Animator animator)
    {
        string objName = animator.gameObject.name;
        
        // Controlla se ha un RuntimeAnimatorController
        if (animator.runtimeAnimatorController == null)
        {
            
            // Controlla se ci sono componenti correlati
            if (animator.GetComponent<PlayerAnimator>() != null)
            {
            }
            if (animator.GetComponent<EnemyAnimator>() != null)
            {
            }
            if (animator.GetComponent<Player>() != null)
            {
            }
            if (animator.GetComponent<EnemyBase>() != null)
            {
            }
        }
        else
        {
            
            if (logDetailedInfo)
            {
                // Log dei parametri
                var parameters = animator.parameters;
                foreach (var param in parameters)
                {
                }
            }
        }
        
        // Controlla lo stato dell'animator
        if (!animator.enabled)
        {
        }
        
        // Controlla se il GameObject è attivo
        if (!animator.gameObject.activeInHierarchy)
        {
        }
    }
    
    /// <summary>
    /// Tenta di recuperare automaticamente gli Animator Controller mancanti
    /// </summary>
    [ContextMenu("Attempt Auto-Fix Missing Controllers")]
    public void AttemptAutoFix()
    {
        
        Animator[] allAnimators = FindObjectsOfType<Animator>();
        int fixedCount = 0;
        
        foreach (Animator animator in allAnimators)
        {
            if (animator.runtimeAnimatorController == null)
            {
                if (TryFixAnimatorController(animator))
                {
                    fixedCount++;
                }
            }
        }
        
    }
    
    private bool TryFixAnimatorController(Animator animator)
    {
        string objName = animator.gameObject.name;
        
        // Logica per tentare di recuperare il controller basata sul tipo di oggetto
        if (animator.GetComponent<Player>() != null || animator.GetComponent<PlayerAnimator>() != null)
        {
            // Qui potresti caricare un controller di default per il player
            // RuntimeAnimatorController playerController = Resources.Load<RuntimeAnimatorController>("Animators/PlayerController");
            // if (playerController != null)
            // {
            //     animator.runtimeAnimatorController = playerController;
            //     return true;
            // }
        }
        else if (animator.GetComponent<EnemyBase>() != null || animator.GetComponent<EnemyAnimator>() != null)
        {
            // Qui potresti caricare un controller di default per i nemici
            // RuntimeAnimatorController enemyController = Resources.Load<RuntimeAnimatorController>("Animators/EnemyController");
            // if (enemyController != null)
            // {
            //     animator.runtimeAnimatorController = enemyController;
            //     return true;
            // }
        }
        
        return false;
    }
}