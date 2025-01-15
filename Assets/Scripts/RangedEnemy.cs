using UnityEngine;

public class RangedEnemy : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float stoppingDistance;
    [SerializeField] private float retreatDistance;
    [SerializeField] private float rangedEnemyHealth;

    [SerializeField] private float startTimeBetweenShots;
    [SerializeField] private GameObject projectile;

    private float timeBetweenShots;
    private Transform target;

    void Start()
    {
        target = GameObject.FindGameObjectWithTag("Player")?.transform;
        timeBetweenShots = startTimeBetweenShots;
    }

    void Update()
    {
        if (target != null)
        {
            HandleMovement();
            HandleShooting();
            AimAtPlayer();
        }
        Die();
    }

    private void HandleMovement()
    {
        float distanceToTarget = Vector2.Distance(transform.position, target.position);

        if (distanceToTarget > stoppingDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }
        else if (distanceToTarget < stoppingDistance && distanceToTarget > retreatDistance)
        {
            transform.position = transform.position;
        }
        else if (distanceToTarget < retreatDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, -speed * Time.deltaTime);
        }
    }

    private void HandleShooting()
    {
        if (timeBetweenShots <= 0)
        {
            Shoot();
            timeBetweenShots = startTimeBetweenShots;
        }
        else
        {
            timeBetweenShots -= Time.deltaTime;
        }
    }

    private void Shoot()
    {
            Vector2 direction = target.position - transform.position;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Instantiate(projectile, transform.position, Quaternion.Euler(new Vector3(0, 0, angle - 90f)));
        }

    

    private void AimAtPlayer()
    {
        Vector2 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
    }

    private void Die()
    {
        if (rangedEnemyHealth <= 0)
        {
            Destroy(gameObject);
            Debug.Log("Enemy is dead");
        }
    }
}

