using System;
using UnityEngine;

public class Fragment : MonoBehaviour
{
private float damage;
private Vector2 velocity;


public void Init(float damage, Vector2 velocity)
{
this.damage = damage;
this.velocity = velocity;
Destroy(gameObject, 3f);
}


private void Update()
{
transform.position += (Vector3)(velocity * Time.deltaTime);
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
Destroy(gameObject);
}
else if (other.CompareTag("Ground"))
{
Destroy(gameObject);
}
}
}