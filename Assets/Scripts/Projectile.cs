using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] ProjectileSO projectileSO;
    void Start()
    {
        //il proiettile viene distrutto quando il time to live arriva a zero
        Destroy(gameObject,projectileSO.lifeTime );
    }
    public float Damage { get => projectileSO.damage; }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * (projectileSO.speed * Time.deltaTime));
    }
}
