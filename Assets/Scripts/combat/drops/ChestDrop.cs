using UnityEngine;

/// <summary>
/// Chest drop that provides an instant level up to the player
/// </summary>
public class ChestDrop : Drop
{
    [Header("Chest Settings")]
    [SerializeField] private bool attractToPlayer = false;
    [SerializeField] private float attractRadius = 2f;
    [SerializeField] private float attractSpeed = 3f;
    private Animator animator;
    private Transform player;
    private bool isAttracting = false;
    private bool hasBeenCollected = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        // Find player reference
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    private void Update()
    {
        if (!attractToPlayer || player == null) return;

        // Check if player is close enough to start attracting
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attractRadius)
        {
            isAttracting = true;
        }

        // Move towards player if attracting
        if (isAttracting)
        {
            transform.position = Vector3.MoveTowards(transform.position,
                player.position, attractSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("Player"))
        {// Prevent multiple collections
          if (hasBeenCollected)
                return;
            
            hasBeenCollected = true;
            animator.SetTrigger("open");
            // Give player a level up
            if (GameManager.Instance != null)
            {
                // Force level up by adding the exact XP needed
                float currentXP = GameManager.Instance.GetXP();
                float xpNeeded = GameManager.Instance.GetXPNeededToLevelUp();

                // Add the remaining XP needed to level up
                GameManager.Instance.AddXP(xpNeeded);

                // Optional: Add some visual/audio feedback here
                ShowLevelUpEffect();
            }

            // Destroy the chest
            //wait for the animation to finish before destroying
            
            Destroy(gameObject,2f);
        }
    }

    /// <summary>
    /// Shows visual effect when chest is collected (can be expanded with particles, sound, etc.)
    /// </summary>
    private void ShowLevelUpEffect()
    {
        // This can be expanded with particle effects, sound effects, etc.
        // For now, we'll just log it for debugging
        if (GameManager.Instance != null)
        {
            Debug.Log($"Chest collected! Player leveled up to level {GameManager.Instance.GetPlayerLevel()}");
        }
    }

    /// <summary>
    /// Configure chest attraction settings
    /// </summary>
    public void SetAttractionSettings(bool attract, float radius = 2f, float speed = 3f)
    {
        attractToPlayer = attract;
        attractRadius = radius;
        attractSpeed = speed;
    }
}