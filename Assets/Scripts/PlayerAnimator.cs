using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    private Animator playerAnimator;
    [SerializeField] private Player player;

    private void Awake()
    {
        if (TryGetComponent<Animator>(out Animator animator))
        {
            playerAnimator = animator;
        }
        else
        {
            Debug.LogWarning("Nessun Animator presente in PlayerPrefab");
        }
    
    
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player.OnPlayerMoving += OnPlayerMoving;
        player.OnPlayerStopMoving += OnPlayerStopMoving;
        player.OnPlayerDead += OnPlayerDead;
    }

        private void OnPlayerMoving(object sender, System.EventArgs e)
        {
            playerAnimator.SetBool("isRunning", true);  
           
        }
    
        private void OnPlayerStopMoving(object sender, System.EventArgs e)
        {
             playerAnimator.SetBool("isRunning", false); 
           
        }
    
        private void OnPlayerDead(object sender, System.EventArgs e)
        {
            playerAnimator.SetBool("isDead", true);
           
        }

    // Update is called once per frame
    void Update()
    {
        
    }
}
