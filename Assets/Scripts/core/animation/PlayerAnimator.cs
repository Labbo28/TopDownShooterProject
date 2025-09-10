using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

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
        }

        // Se il player non è assegnato, cerca di trovarlo
        if (player == null)
        {
            player = GetComponent<Player>();
        }

        // Iscriviti all'evento di caricamento scena per resettare l'animator
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GameScene")
        {
            // Reset dell'animator quando viene caricata la GameScene
            ResetAnimator();
            
            // Riconnetti gli eventi del player se necessario
            ReconnectPlayerEvents();
        }
    }

    private void Start()
    {
        ConnectPlayerEvents();
    }

    private void ConnectPlayerEvents()
    {
        if (player != null)
        {
            player.OnPlayerMoving.AddListener(OnPlayerMoving);
            player.OnPlayerStopMoving.AddListener(OnPlayerStopMoving);
            player.OnPlayerDead.AddListener(OnPlayerDead);
        }
        else
        {
        }
    }

    private void ReconnectPlayerEvents()
    {
        if (player == null)
        {
            player = Player.Instance;
            if (player == null)
            {
                player = GetComponent<Player>();
            }
        }

        if (player != null)
        {
            // Rimuovi i listener esistenti per evitare duplicati
            player.OnPlayerMoving.RemoveListener(OnPlayerMoving);
            player.OnPlayerStopMoving.RemoveListener(OnPlayerStopMoving);
            player.OnPlayerDead.RemoveListener(OnPlayerDead);

            // Riconnetti
            player.OnPlayerMoving.AddListener(OnPlayerMoving);
            player.OnPlayerStopMoving.AddListener(OnPlayerStopMoving);
            player.OnPlayerDead.AddListener(OnPlayerDead);
            
        }
    }

    public void ResetAnimator()
    {
        // Se l'animator è null, prova a recuperarlo
        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>();
        }
        
        if (playerAnimator != null)
        {
            // Controlla se ha un controller assegnato
            if (playerAnimator.runtimeAnimatorController == null)
            {
                return;
            }
            
            // Reset di tutti i parametri dell'animator
            playerAnimator.SetBool("isRunning", false);
            playerAnimator.SetBool("isDead", false);
            
            // Reset eventuali trigger
            playerAnimator.ResetTrigger("Hit");
            
            // Forza il ritorno allo stato idle (solo se lo stato esiste)
            try
            {
                playerAnimator.Play("Idle", 0, 0f);
            }
            catch (System.Exception e)
            {
            }
            
        }
        else
        {
        }
    }

    private void OnPlayerMoving()
    {
        if (ValidateAnimator())
        {
            playerAnimator.SetBool("isRunning", true);
        }
    }

    private void OnPlayerStopMoving()
    {
        if (ValidateAnimator())
        {
            playerAnimator.SetBool("isRunning", false);
        }
    }

    private void OnPlayerDead()
    {
        if (ValidateAnimator())
        {
            playerAnimator.SetBool("isDead", true);
            // Ferma il movimento
            playerAnimator.SetBool("isRunning", false);
        }
    }
    
    private bool ValidateAnimator()
    {
        if (playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>();
        }
        
        if (playerAnimator == null)
        {
            return false;
        }
        
        if (playerAnimator.runtimeAnimatorController == null)
        {
            return false;
        }
        
        return true;
    }

    // Metodo pubblico per forzare il reset (utile per debug o chiamate esterne)
    public void ForceResetAnimator()
    {
        ResetAnimator();
    }

    private void OnDestroy()
    {
        // Cleanup degli eventi
        if (player != null)
        {
            player.OnPlayerMoving.RemoveListener(OnPlayerMoving);
            player.OnPlayerStopMoving.RemoveListener(OnPlayerStopMoving);
            player.OnPlayerDead.RemoveListener(OnPlayerDead);
        }

        // Cleanup dell'evento di scena
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Update is called once per frame
    void Update()
    {
        // Controllo di sicurezza: se il player non è morto ma l'animator dice di sì, correggi
        if (player != null && player.GetComponent<HealthSystem>() != null)
        {
            bool playerIsAlive = player.GetComponent<HealthSystem>().IsAlive;
            bool animatorSaysDead = playerAnimator != null && playerAnimator.GetBool("isDead");
            
            if (playerIsAlive && animatorSaysDead)
            {
                ResetAnimator();
            }
        }
    }
}