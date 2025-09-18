using UnityEngine;

/// <summary>
/// Simple pet AI: follows the player at a comfortable distance and does nothing else.
/// The pet is purely cosmetic/companion and should not interfere with gameplay.
/// </summary>
public class PetFollower : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [Tooltip("Distanza alla quale il pet considera di aver raggiunto la sua destinazione e si ferma")]
    [SerializeField] private float stopDistance = 1.6f;
    [Tooltip("Distanza dalla destinazione oltre la quale il pet riprende a muoversi verso il player")]
    [SerializeField] private float resumeDistance = 5.6f;
    [SerializeField] private float catchUpDistance = 4f;
    [SerializeField] private float rotationLerp = 10f;

    [Header("Offset Around Player")]
    [SerializeField] private Vector2 followOffset = new Vector2(0.8f, -0.6f);

    private Transform playerTransform;
    private bool isMoving = true;

    private void Start()
    {
        // Try to get player via singleton, fallback to tag search
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

    private void Update()
    {
        if (playerTransform == null)
        {
            // Try to reconnect if player respawned or loaded
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
                else
                {
                    return;
                }
            }
        }

        Vector3 desiredPosition = playerTransform.position + (Vector3)followOffset;

        float distanceToDesired = Vector2.Distance(transform.position, desiredPosition);

        // Hysteresis: if currently stopped, only start when sufficiently far; if moving, stop when close enough
        if (!isMoving && distanceToDesired >= resumeDistance)
        {
            isMoving = true;
        }
        else if (isMoving && distanceToDesired <= stopDistance)
        {
            isMoving = false;
        }

        if (!isMoving)
        {
            return;
        }

        // If too far, move faster to catch up
        float speed = (distanceToDesired > catchUpDistance) ? moveSpeed * 1.75f : moveSpeed;

        // Move towards the desired position
        Vector3 direction = (desiredPosition - transform.position).normalized;
        transform.position += direction * (speed * Time.deltaTime);

        // Optional rotation to face movement direction
        if (direction.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationLerp * Time.deltaTime);
        }
    }
}


