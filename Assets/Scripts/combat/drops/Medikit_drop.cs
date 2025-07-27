using UnityEngine;

public class Medikit_drop : MonoBehaviour
{
    private Transform player;

    // Start is called once before the first ex
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            HealthSystem hs = other.GetComponent<HealthSystem>();
            hs.Heal(GameManager.Instance.GetHealAmount() * hs.MaxHealth);
            Debug.Log("il player e stato curato di "); // Assuming Heal() is a method in Player
            Destroy(gameObject);
        }
    }
}
