using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerMovementOverride : MonoBehaviour
{
    public enum VerticalDirection { Up, Down }

    [SerializeField, Tooltip("Blocca completamente il movimento del player se true. Se false, viene applicato il movimento verticale forzato.")]
    private bool blockMovement = true;

    [SerializeField, Tooltip("Direzione verticale del movimento forzato. Up = verso l'alto, Down = verso il basso. Usato solo se blockMovement è false.")]
    private VerticalDirection verticalDirection = VerticalDirection.Up;

    [SerializeField, Tooltip("Velocità del movimento verticale forzato in unità al secondo. Usato solo se blockMovement è false.")]
    private float forcedSpeed = 0f;
    
    [SerializeField, Tooltip("camera to follow the player")]
    private Camera followCamera;

    private Player playerScript;
    private InputSystem_Actions backupInputActions;
    private bool wasMoving = false;

    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        if (backupInputActions != null)
            backupInputActions.Enable();
    }

    private void OnSceneUnloaded(Scene scene)
    {
        Destroy(gameObject);
    }

    private void Update()
    {
        // Recupera dinamicamente il Player se non è presente
        if (playerScript == null || playerScript != Player.Instance)
        {
            playerScript = Player.Instance;
            if (playerScript != null)
            {
                backupInputActions = typeof(Player)
                    .GetField("inputActions", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .GetValue(playerScript) as InputSystem_Actions;

                backupInputActions?.Disable();
            }
        }

        if (playerScript == null) return;

        if (blockMovement)
        {
            if (wasMoving)
            {
                Debug.Log("PlayerMovementOverride: StopMoving");
                // Quando si ferma
                playerScript.OnPlayerStopMoving?.Invoke();
                wasMoving = false;
            }
        }
        else if (forcedSpeed > 0f)
        {
            // Muovi direttamente il player lungo la direzione verticale
            Vector2 dir = (verticalDirection == VerticalDirection.Up) ? Vector2.up : Vector2.down;
            playerScript.transform.Translate(dir * forcedSpeed * Time.deltaTime);

            // Invoca sempre l'evento di movimento per ogni frame in cui si muove
            playerScript.OnPlayerMoving?.Invoke();
            wasMoving = true;
        }
        else
        {
            if (wasMoving)
            {
                Debug.Log("PlayerMovementOverride: StopMoving (forcedSpeed 0)");
                playerScript.OnPlayerStopMoving?.Invoke();
                wasMoving = false;
            }
        }

        // Aggiorna la posizione della camera per seguire il player
        if (followCamera != null && playerScript != null)
        {
            Vector3 playerPos = playerScript.transform.position;
            Vector3 cameraPos = followCamera.transform.position;
            // Movimento fluido della camera verso il player
            followCamera.transform.position = Vector3.Lerp(
                cameraPos,
                new Vector3(playerPos.x, playerPos.y, cameraPos.z),
                0.15f // Cambia questo valore per regolare la velocità
            );
        }
    }
}
