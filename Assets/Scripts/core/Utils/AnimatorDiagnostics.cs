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
        Debug.Log("=== ANIMATOR DIAGNOSTICS START ===");
        
        // Controlla tutti gli Animator nella scena
        Animator[] allAnimators = FindObjectsOfType<Animator>();
        Debug.Log($"Found {allAnimators.Length} Animators in scene");
        
        foreach (Animator animator in allAnimators)
        {
            DiagnoseAnimator(animator);
        }
        
        Debug.Log("=== ANIMATOR DIAGNOSTICS END ===");
    }
    
    private void DiagnoseAnimator(Animator animator)
    {
        string objName = animator.gameObject.name;
        Debug.Log($"--- Diagnosing Animator on: {objName} ---");
        
        // Controlla se ha un RuntimeAnimatorController
        if (animator.runtimeAnimatorController == null)
        {
            Debug.LogWarning($"⚠️ {objName}: RuntimeAnimatorController is NULL!");
            
            // Controlla se ci sono componenti correlati
            if (animator.GetComponent<PlayerAnimator>() != null)
            {
                Debug.Log($"   - Has PlayerAnimator component");
            }
            if (animator.GetComponent<EnemyAnimator>() != null)
            {
                Debug.Log($"   - Has EnemyAnimator component");
            }
            if (animator.GetComponent<Player>() != null)
            {
                Debug.Log($"   - Has Player component");
            }
            if (animator.GetComponent<EnemyBase>() != null)
            {
                Debug.Log($"   - Has EnemyBase component");
            }
        }
        else
        {
            Debug.Log($"✅ {objName}: RuntimeAnimatorController OK - {animator.runtimeAnimatorController.name}");
            
            if (logDetailedInfo)
            {
                // Log dei parametri
                var parameters = animator.parameters;
                Debug.Log($"   - Parameters count: {parameters.Length}");
                foreach (var param in parameters)
                {
                    Debug.Log($"     * {param.name} ({param.type})");
                }
            }
        }
        
        // Controlla lo stato dell'animator
        if (!animator.enabled)
        {
            Debug.LogWarning($"⚠️ {objName}: Animator component is DISABLED!");
        }
        
        // Controlla se il GameObject è attivo
        if (!animator.gameObject.activeInHierarchy)
        {
            Debug.LogWarning($"⚠️ {objName}: GameObject is INACTIVE in hierarchy!");
        }
    }
    
    /// <summary>
    /// Tenta di recuperare automaticamente gli Animator Controller mancanti
    /// </summary>
    [ContextMenu("Attempt Auto-Fix Missing Controllers")]
    public void AttemptAutoFix()
    {
        Debug.Log("=== ATTEMPTING AUTO-FIX FOR MISSING ANIMATOR CONTROLLERS ===");
        
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
        
        Debug.Log($"Auto-fix completed. Fixed {fixedCount} Animator Controllers.");
    }
    
    private bool TryFixAnimatorController(Animator animator)
    {
        string objName = animator.gameObject.name;
        
        // Logica per tentare di recuperare il controller basata sul tipo di oggetto
        if (animator.GetComponent<Player>() != null || animator.GetComponent<PlayerAnimator>() != null)
        {
            Debug.Log($"Attempting to fix Player Animator Controller for: {objName}");
            // Qui potresti caricare un controller di default per il player
            // RuntimeAnimatorController playerController = Resources.Load<RuntimeAnimatorController>("Animators/PlayerController");
            // if (playerController != null)
            // {
            //     animator.runtimeAnimatorController = playerController;
            //     Debug.Log($"✅ Fixed Player Animator Controller for: {objName}");
            //     return true;
            // }
        }
        else if (animator.GetComponent<EnemyBase>() != null || animator.GetComponent<EnemyAnimator>() != null)
        {
            Debug.Log($"Attempting to fix Enemy Animator Controller for: {objName}");
            // Qui potresti caricare un controller di default per i nemici
            // RuntimeAnimatorController enemyController = Resources.Load<RuntimeAnimatorController>("Animators/EnemyController");
            // if (enemyController != null)
            // {
            //     animator.runtimeAnimatorController = enemyController;
            //     Debug.Log($"✅ Fixed Enemy Animator Controller for: {objName}");
            //     return true;
            // }
        }
        
        Debug.LogWarning($"❌ Could not auto-fix Animator Controller for: {objName}");
        return false;
    }
}