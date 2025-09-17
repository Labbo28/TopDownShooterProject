using UnityEngine;

using System.Collections;
using UnityEngine.Tilemaps;

public class PlayerUpToForest : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Dialogue dialogue;
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private Collider2D stopCollider; // opzionale, se vuoi specificare il collider

    private bool isMoving = false;
    private bool hasStarted = false;

    private void Start()
    {
        if (player == null)
            player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            player.SetMovementSpeed(moveSpeed);
            player.enabled = false; // Disabilita controlli manuali
        }
        isMoving = true;
        hasStarted = false;
    }

    private void Update()
    {
        if (isMoving && player != null)
        {
            // Muovi il player verso l'alto (asse Y)
            player.transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isMoving) return;

        // Se è il collider giusto (TilemapCollider2D o quello specificato)
        if ((stopCollider != null && other == stopCollider) ||
            (stopCollider == null && other is TilemapCollider2D))
        {
            isMoving = false;
            if (player != null)
            {
                player.enabled = true; // Riabilita controlli se vuoi dopo il dialogo
            }
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        if (dialogue != null && !hasStarted)
        {
            hasStarted = true;
            DialogueManager.Instance.StartDialogue(dialogue);
        }
    }
}
