using UnityEngine;
using System.Collections.Generic;

public class PlayerConstrainMapMovment : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float defaultMoveSpeed = 4f;
    [SerializeField] private List<Vector3> waypoints; // Lista di coordinate da Inspector
    [SerializeField] private List<float> waypointSpeeds; // Velocità per ogni segmento

    private int currentWaypointIndex = 0;
    private bool isMoving = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (player == null)
            player = FindFirstObjectByType<Player>();
        if (player != null)
        {
            player.SetMovementSpeed(defaultMoveSpeed);
            player.enabled = false; // Disabilita controlli manuali
        }
        if (waypoints != null && waypoints.Count > 0)
        {
            isMoving = true;
            player.transform.position = waypoints[0]; // opzionale: parte dal primo punto
            currentWaypointIndex = 1;
            if (player != null)
            {
                // Attiva animazione corsa
                player.OnPlayerMoving.Invoke();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isMoving && player != null && currentWaypointIndex < waypoints.Count)
        {
            Vector3 target = waypoints[currentWaypointIndex];
            float speed = defaultMoveSpeed;
            if (waypointSpeeds != null && waypointSpeeds.Count >= currentWaypointIndex)
                speed = waypointSpeeds[currentWaypointIndex - 1];

            player.transform.position = Vector3.MoveTowards(
                player.transform.position,
                target,
                speed * Time.deltaTime
            );

            // Chiama sempre l'evento di movimento mentre si muove
            player.OnPlayerMoving?.Invoke();

            if (Vector3.Distance(player.transform.position, target) < 0.05f)
            {
                currentWaypointIndex++;
                // Se ha raggiunto l'ultimo waypoint, ferma il movimento e l'animazione corsa
                if (currentWaypointIndex >= waypoints.Count)
                {
                    isMoving = false;
                    player.enabled = true; // Riabilita controlli manuali
                    player.OnPlayerStopMoving.Invoke(); // Ferma animazione corsa
                }
            }
        }
    }
}
