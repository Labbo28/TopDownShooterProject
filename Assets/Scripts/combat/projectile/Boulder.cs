using System;
using UnityEngine;

public class Boulder : MonoBehaviour
{
    private GameObject fragmentPrefab;
    private int fragmentCount;
    private float fragmentSpreadForce;
    private float damage;
    private Vector3 startPos;
    private Vector3 targetPos;
    private float arcHeight;
    private float travelTime;
    private float elapsed;

    public void Init(GameObject fragmentPrefab, int fragmentCount, float fragmentSpreadForce, float damage, Vector3 targetPos, float arcHeight, float travelTime)
    {
        this.fragmentPrefab = fragmentPrefab;
        this.fragmentCount = fragmentCount;
        this.fragmentSpreadForce = fragmentSpreadForce;
        this.damage = damage;
        this.startPos = transform.position;
        this.targetPos = targetPos;
        this.arcHeight = arcHeight;
        this.travelTime = travelTime;
        this.elapsed = 0f;
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / travelTime);

        // Interpolazione orizzontale lineare
        Vector3 pos = Vector3.Lerp(startPos, targetPos, t);

        // Aggiunge un arco parabolico sull'asse Y
        float height = arcHeight * (1f - (2f * t - 1f) * (2f * t - 1f));
        pos.y += height;

        transform.position = pos;

        if (t >= 1f)
        {
            Explode();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            IDamageable playerHealth = other.GetComponent<IDamageable>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Explode();
        }
        else if (other.CompareTag("Ground"))
        {
            Explode();
        }
    }

    private void Explode()
    {
        if (fragmentPrefab != null)
        {
            for (int i = 0; i < fragmentCount; i++)
            {
                GameObject fragment = Instantiate(fragmentPrefab, transform.position, Quaternion.identity);
                Fragment fragScript = fragment.AddComponent<Fragment>();
                Vector2 randomDir = UnityEngine.Random.insideUnitCircle.normalized;
                fragScript.Init(damage * 0.5f, randomDir * fragmentSpreadForce);
            }
        }
    }
}
