using UnityEngine;

public class LookAtCursor : MonoBehaviour
{
    

    // Update is called once per frame
    void Update()
    {
        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        transform.rotation =  Quaternion.Euler(0, 0, angle);
    }
}
