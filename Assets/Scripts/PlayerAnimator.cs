using UnityEngine;
using UnityEngine.Events;

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

    private void Start()
    {
        player.OnPlayerMoving.AddListener(OnPlayerMoving);
        player.OnPlayerStopMoving.AddListener(OnPlayerStopMoving);
        player.OnPlayerDead.AddListener(OnPlayerDead);
    }

    private void OnPlayerMoving()
    {
        playerAnimator.SetBool("isRunning", true);  
    }

    private void OnPlayerStopMoving()
    {
        playerAnimator.SetBool("isRunning", false); 
    }

    private void OnPlayerDead()
    {
        playerAnimator.SetBool("isDead", true);
    }

    private void OnDestroy()
    {
        if (player != null)
        {
            player.OnPlayerMoving.RemoveListener(OnPlayerMoving);
            player.OnPlayerStopMoving.RemoveListener(OnPlayerStopMoving);
            player.OnPlayerDead.RemoveListener(OnPlayerDead);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
