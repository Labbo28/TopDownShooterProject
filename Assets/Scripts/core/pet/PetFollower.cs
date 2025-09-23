using UnityEngine;

/// <summary>
/// Gatto che segue il player quando triggerato
/// </summary>
public class PetFollower : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 3f;
    
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Transform playerTransform;
    private bool isFollowing = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        // Trova il player
        FindPlayer();
        
        // Inizia fermo
        if (animator != null)
        {
            animator.SetBool("isRunning", false);
        }
    }

    private void Update()
    {
        if (isFollowing && playerTransform != null)
        {
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
        // Muoviti verso il player
        Vector3 direction = (playerTransform.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;
        
        // Flip sprite in base alla direzione
        if (direction.x > 0)
        {
            spriteRenderer.flipX = false; // Guardando a destra
        }
        else if (direction.x < 0)
        {
            spriteRenderer.flipX = true; // Guardando a sinistra
        }
    }

    private void FindPlayer()
    {
        // Cerca il player
        if (Player.Instance != null)
        {
            playerTransform = Player.Instance.transform;
        }
        else
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                playerTransform = playerObject.transform;
            }
        }
    }

    // Metodo pubblico per attivare il following (chiamato dal DialogueTrigger)
    public void StartFollowing()
    {
        isFollowing = true;
        
        if (animator != null)
        {
            animator.SetBool("isRunning", true);
        }
        
        Debug.Log("Gatto attivato - segue il player!");
    }
}