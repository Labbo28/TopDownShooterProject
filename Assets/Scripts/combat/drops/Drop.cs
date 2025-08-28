using System;
using System.Collections;
using UnityEngine;

public abstract class Drop : MonoBehaviour
{
    // Time (in seconds) before the drop is automatically destroyed
    [SerializeField] private float timeToLive = 180f;

    private void Awake()
    {
        // Schedule the drop for destruction after the specified time
        DestroyAfterTime(timeToLive);
    }

 
private void DestroyAfterTime(float timeToLive)
{
    Destroy(gameObject, timeToLive);
}
}