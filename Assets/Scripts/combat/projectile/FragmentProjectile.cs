using UnityEngine;
using System.Collections;

/// <summary>
/// Frammento che si genera dall'esplosione del Boulder
/// </summary>
public class FragmentProjectile : MonoBehaviour
{
    [Header("Fragment Settings")]
    [SerializeField] private float damage = 10f;
    [SerializeField] private float speed = 15f;
    [SerializeField] private float lifeTime = 2f;
    [SerializeField] private float gravity = 5f;

    [Header("Visual Effects")]
    [SerializeField] private GameObject destroyEffect;
    [SerializeField] private AudioClip hitSound;

    private Vector3 direction;
    private Vector3 velocity;
    private bool hasHit = false;

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
            rb.gravityScale = 0f; // Usiamo la nostra gravità custom
            rb.isKinematic = true; // Use kinematic physics
        }

        // Setup collider
        if (col != null)
        {
            col.isTrigger = true;
        }

        // Inizializza velocità
        velocity = direction * speed;

        // Distruggi dopo lifeTime
        Destroy(gameObject, lifeTime);

        // Effetti visivi iniziali
        StartCoroutine(FragmentTrail());
    }

    void Update()
    {
        if (hasHit) return;

        // Applica gravità personalizzata
        velocity.y -= gravity * Time.deltaTime;

        // Muovi il frammento
        transform.position += velocity * Time.deltaTime;

        // Rotazione del frammento
        transform.Rotate(0, 0, 720f * Time.deltaTime);

        // Controlla se ha toccato terra (opzionale)
        if (transform.position.y <= -10f) // Adjust based on your ground level
        {
            DestroyFragment();
        }
    }

    /// <summary>
    /// Imposta la direzione del frammento
    /// </summary>
    public void SetDirection(Vector3 newDirection)
    {
        direction = newDirection.normalized;

        // Orienta il frammento verso la direzione di movimento
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }

    /// <summary>
    /// Imposta il danno del frammento
    /// </summary>
    public void SetDamage(float newDamage)
    {
        damage = newDamage;
    }

    /// <summary>
    /// Imposta velocità e gravità personalizzate
    /// </summary>
    public void SetPhysics(float newSpeed, float newGravity)
    {
        speed = newSpeed;
        gravity = newGravity;
        velocity = direction * speed;
    }

    /// <summary>
    /// Effetto scia del frammento
    /// </summary>
    private IEnumerator FragmentTrail()
    {
        if (spriteRenderer == null) yield break;

        Color originalColor = spriteRenderer.color;

        while (!hasHit && gameObject != null)
        {
            // Crea una scia lasciando copie trasparenti
            GameObject trail = new GameObject("FragmentTrail");
            trail.transform.position = transform.position;
            trail.transform.rotation = transform.rotation;

            SpriteRenderer trailRenderer = trail.AddComponent<SpriteRenderer>();
            trailRenderer.sprite = spriteRenderer.sprite;
            trailRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.3f);
            trailRenderer.sortingOrder = spriteRenderer.sortingOrder - 1;

            // Fai svanire e distruggi la scia
            StartCoroutine(FadeTrail(trailRenderer));

            yield return new WaitForSeconds(0.05f);
        }
    }

    /// <summary>
    /// Fai svanire l'effetto scia
    /// </summary>
    private IEnumerator FadeTrail(SpriteRenderer trailRenderer)
    {
        Color startColor = trailRenderer.color;
        float fadeTime = 0.3f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeTime)
        {
            elapsedTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, elapsedTime / fadeTime);
            trailRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        if (trailRenderer != null && trailRenderer.gameObject != null)
        {
            Destroy(trailRenderer.gameObject);
        }
    }

    /// <summary>
    /// Distruggi il frammento con effetti
    /// </summary>
    private void DestroyFragment()
    {
        if (hasHit) return;
        hasHit = true;

        // Effetti visivi
        if (destroyEffect != null)
        {
            Instantiate(destroyEffect, transform.position, Quaternion.identity);
        }

        // Effetti sonori
        if (hitSound != null && audioSource != null)
        {
            // Crea un AudioSource temporaneo per il suono
            GameObject tempAudio = new GameObject("FragmentHitSound");
            tempAudio.transform.position = transform.position;
            AudioSource tempSource = tempAudio.AddComponent<AudioSource>();
            tempSource.clip = hitSound;
            tempSource.volume = 0.5f;
            tempSource.Play();
            Destroy(tempAudio, hitSound.length);
        }

        // Flash finale
        StartCoroutine(DestroyFlash());
    }

    /// <summary>
    /// Flash visivo prima della distruzione
    /// </summary>
    private IEnumerator DestroyFlash()
    {
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = Color.yellow;
            yield return new WaitForSeconds(0.1f);
            spriteRenderer.color = originalColor;
        }

        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        if (other.CompareTag("Player"))
        {
            // Danni al player
           HealthSystem hs = Player.Instance.GetComponent<HealthSystem>();
           hs.TakeDamage(damage);
            

            DestroyFragment();
        }
        else if (other.CompareTag("Obstacle") || other.CompareTag("Wall"))
        {
            // Impatto con ostacoli
            DestroyFragment();
        }
    }

    // Debug visualization
    void OnDrawGizmosSelected()
    {
        // Mostra la direzione del movimento
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(transform.position, direction * 2f);

        // Mostra il trigger collider
        Gizmos.color = Color.red;
        if (col != null)
        {
            Gizmos.DrawWireSphere(transform.position, col.radius);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, 0.2f);
        }
    }
}
