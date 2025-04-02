using UnityEngine;

public class XPDrop : MonoBehaviour
{
    [SerializeField] private float xpValue;
    [SerializeField] private float attractSpeed = 5f;
    [SerializeField] private float attractRadius = 1.5f;
    
    private Transform player;
    private bool isAttracting = false;
    
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }
    
    private void Update()
    {
        // Attrazione verso il giocatore quando è vicino
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= attractRadius)
        {
            isAttracting = true;
        }
        
        if (isAttracting)
        {
            transform.position = Vector3.MoveTowards(transform.position, 
                player.position, attractSpeed * Time.deltaTime);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collisione con " + other.name);
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddXP(xpValue);
            Destroy(gameObject);
        }
    }
}