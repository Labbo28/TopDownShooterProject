using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 20;

    [SerializeField] private float damage = 20;
    [SerializeField] private float ttl;  //time to live
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //il proiettile viene distrutto quando il time to live arriva a zero
        Destroy(gameObject, ttl);
    }
    public float Damage { get => damage; }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * (speed * Time.deltaTime));
    }
}
