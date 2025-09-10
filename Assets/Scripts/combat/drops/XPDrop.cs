using UnityEngine;

public class XPDrop : Drop
{


    [SerializeField] private float xpValue;
    
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
        
        if (distanceToPlayer <= GameManager.Instance.GetAttractRadius())
        {
            isAttracting = true;
        }
        else if (distanceToPlayer > GameManager.Instance.GetAttractRadius())
        {
            isAttracting = true;
        }
        
        if (isAttracting)
        {
            transform.position = Vector3.MoveTowards(transform.position, 
                player.position, GameManager.Instance.GetAttractSpeed() * Time.deltaTime);
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.AddXP(xpValue);
            Destroy(gameObject);
        }
    }


    public void SetXPValue(float value)
    {
        xpValue = value;
    }
    public float GetXPValue()
    {
        return xpValue;
    }
  
}