using UnityEngine;
using System.Collections;

/// <summary>
/// Boulder projectile che si frammenta all'impatto creando fragment projectiles
/// </summary>
public class BoulderProjectile : MonoBehaviour
{
    [Header("Boulder Settings")]
    [SerializeField] private float damage = 25f;
    [SerializeField] private float speed = 8f;
    [SerializeField] private float lifeTime = 5f;

    [Header("Fragmentation")]
    [SerializeField] private GameObject fragmentPrefab;
    [SerializeField] private int fragmentCount = 6;
    [SerializeField] private float fragmentSpread = 360f;
    [SerializeField] private float explosionRadius = 3f;

    [Header("Visual Effects")]
    [SerializeField] private GameObject impactEffect;
    [SerializeField] private AudioClip impactSound;

    private Vector3 targetPosition;
    private Vector3 startPosition;
    private float journeyLength;
    private float journeyTime = 0f;
    private bool hasImpacted = false;

    // Componenti
    private Rigidbody2D rb;
    private CircleCollider2D col;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;

    void Start()
    {
        // Setup componenti
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();

        // Setup physics - Kinematic movement
        if (rb != null)
        {
            rb.gravityScale = 0f;
            rb.isKinematic = true; // Use kinematic physics
        }

        // Setup collider per trigger
        if (col != null)
        {
            col.isTrigger = true;
        }

        // Calcola la lunghezza del viaggio
        startPosition = transform.position;
        journeyLength = Vector3.Distance(startPosition, targetPosition);

        // Distruggi dopo lifeTime se non ha colpito nulla
        Destroy(gameObject, lifeTime);

        // Effetto visivo di lancio
        StartCoroutine(BoulderTrajectory());
    }

    /// <summary>
    /// Imposta il target del boulder
    /// </summary>
    public void SetTarget(Vector3 target)
    {
        targetPosition = target;
    }

    /// <summary>
    /// Imposta i parametri del boulder
    /// </summary>
    public void SetBoulderParameters(float newDamage, float newSpeed, int newFragmentCount)
    {
        damage = newDamage;
        speed = newSpeed;
        fragmentCount = newFragmentCount;
    }

    void Update()
    {
        if (hasImpacted) return;

        // Movimento verso il target
        MoveBoulder();

        // Controlla se ha raggiunto il target
        if (Vector3.Distance(transform.position, targetPosition) <= 0.5f)
        {
            Impact();
        }
    }

    /// <summary>
    /// Movimento del boulder verso il target
    /// </summary>
    private void MoveBoulder()
    {
        journeyTime += Time.deltaTime;
        float fractionOfJourney = (journeyTime * speed) / journeyLength;

        if (fractionOfJourney <= 1f)
        {
            transform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);

            // Rotazione del boulder
            transform.Rotate(0, 0, 360f * Time.deltaTime);
        }
        else
        {
            transform.position = targetPosition;
            Impact();
        }
    }

    /// <summary>
    /// Traiettoria curva del boulder (opzionale)
    /// </summary>
    private IEnumerator BoulderTrajectory()
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = targetPosition;

        // Calcola il punto di controllo per la curva (punto più alto)
        Vector3 controlPoint = (startPos + endPos) * 0.5f;
        controlPoint.y += 3f; // Altezza dell'arco

        float elapsedTime = 0f;
        float totalTime = journeyLength / speed;

        while (elapsedTime < totalTime && !hasImpacted)
        {
            float t = elapsedTime / totalTime;

            // Curva di Bézier quadratica
            Vector3 position = Mathf.Pow(1 - t, 2) * startPos +
                              2 * (1 - t) * t * controlPoint +
                              Mathf.Pow(t, 2) * endPos;

            transform.position = position;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        if (!hasImpacted)
        {
            Impact();
        }
    }

    /// <summary>
    /// Gestisce l'impatto del boulder
    /// </summary>
    private void Impact()
    {
        if (hasImpacted) return;
        hasImpacted = true;

        // Danni diretti nell'area dell'impatto
        DamageInRadius();

        // Crea i frammenti
        CreateFragments();

        // Effetti visivi e sonori
        PlayImpactEffects();

        // Distruggi il boulder
        Destroy(gameObject, 0.1f);
    }

    /// <summary>
    /// Causa danni in un raggio dall'impatto
    /// </summary>
    private void DamageInRadius()
    {
        Collider2D[] hitColliders = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                HealthSystem hs =Player.Instance.GetComponent<HealthSystem>();
                
                hs.TakeDamage(damage);
                
            }
        }
    }

    /// <summary>
    /// Crea i frammenti che si disperdono dall'impatto
    /// </summary>
    private void CreateFragments()
    {
        if (fragmentPrefab == null) return;

        float angleStep = fragmentSpread / fragmentCount;
        float startAngle = -fragmentSpread / 2f;

        for (int i = 0; i < fragmentCount; i++)
        {
            float angle = startAngle + (angleStep * i) + Random.Range(-15f, 15f);
            Vector3 direction = new Vector3(
                Mathf.Cos(angle * Mathf.Deg2Rad),
                Mathf.Sin(angle * Mathf.Deg2Rad),
                0
            ).normalized;

            // Crea il frammento
            GameObject fragment = Instantiate(fragmentPrefab, transform.position, Quaternion.identity);

            // Configura il frammento
            FragmentProjectile fragmentScript = fragment.GetComponent<FragmentProjectile>();
            if (fragmentScript != null)
            {
                fragmentScript.SetDirection(direction);
                fragmentScript.SetDamage(damage * 0.4f); // I frammenti fanno meno danno
            }

            // I frammenti usano movimento cinematico gestito dal proprio script
        }
    }

    /// <summary>
    /// Riproduce effetti visivi e sonori dell'impatto
    /// </summary>
    private void PlayImpactEffects()
    {
        // Effetto visivo
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        // Effetto sonoro
        if (impactSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(impactSound);
        }

        // Screen shake (se disponibile)
            // Se hai un CameraShake script, decommentare questa riga:
            // cameraShake.Shake(0.3f, 0.5f);

        // Flash dello sprite prima della distruzione
        StartCoroutine(ImpactFlash());
    }

    /// <summary>
    /// Flash visivo all'impatto
    /// </summary>
    private IEnumerator ImpactFlash()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Impatto con ostacoli o player
        if (other.CompareTag("Player") || other.CompareTag("Obstacle"))
        {
            AudioManager.Instance.PlayRockImpact();
            Impact();
        }
    }

    // Debug visualization
    void OnDrawGizmosSelected()
    {
        // Mostra il raggio di esplosione
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);

        // Mostra la traiettoria verso il target
        if (targetPosition != Vector3.zero)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, targetPosition);
            Gizmos.DrawSphere(targetPosition, 0.5f);
        }
    }
}
