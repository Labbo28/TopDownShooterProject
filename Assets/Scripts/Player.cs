using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour
{
    public Transform weapon;
    public float offset;
    public Transform shotPoint;
    public GameObject projectile;
    public float timeBetweenShots;
    float nextShotTime;
    [SerializeField] private float movementSpeed = 5f;
   
    private Vector2 _movementDirection;
    
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        if (PlayerCanDash())
        {
            HandleDash();
        }
        // Rotazione Arma
        Vector3 displacement = weapon.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        float angle = Mathf.Atan2(displacement.y, displacement.x) * Mathf.Rad2Deg;
        weapon.rotation = Quaternion.Euler(0f, 0f, angle + offset);

        //Spara Peppino, SPARAAAAAA!!!!!
        if (Input.GetMouseButtonDown(0)){
            if (Time.time > nextShotTime){
                nextShotTime= Time.time + timeBetweenShots;
                Instantiate(projectile, shotPoint.position, shotPoint.rotation);
            }
        }
    }
    //metodo da implementare
    private bool PlayerCanDash()
    {
        return true ;
       
    }

    private void HandleDash()
{
    if (Input.GetKeyDown(KeyCode.LeftShift))
    {
        Vector3 dashDirection = new Vector3(_movementDirection.x, _movementDirection.y).normalized;
        float dashDistance = 6f; // Example dash distance
        Vector3 dashTarget = transform.position + dashDirection * dashDistance;
        StartCoroutine(DashMovement(dashTarget, 0.2f)); // Example duration of 0.2 seconds
    }
}

private IEnumerator DashMovement(Vector3 targetPosition, float duration)
{
    Vector3 startPosition = transform.position;
    float elapsedTime = 0f;

    while (elapsedTime < duration)
    {
        transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
        elapsedTime += Time.deltaTime;
        yield return null;
    }

    transform.position = targetPosition;
}
   
private void HandleMovement()
{
  
    _movementDirection = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
    transform.Translate(_movementDirection * (Time.deltaTime * movementSpeed));
}
    
}
