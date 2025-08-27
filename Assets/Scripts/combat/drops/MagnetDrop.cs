using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MagnetDrop : Drop
{
    private float attractRadiusMultiplier = 2f;
    private float attractSpeedMultiplier = 2f;
    private Transform player;

    public float AttractRadiusMultiplier
    {
        get => attractRadiusMultiplier;
        set => attractRadiusMultiplier = value;
    }

    public float AttractSpeedMultiplier
    {
        get => attractSpeedMultiplier;
        set => attractSpeedMultiplier = value;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Collisione con " + other.name);
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.SetAttractRadius(GameManager.Instance.GetAttractRadius() * attractRadiusMultiplier);
            GameManager.Instance.SetAttractSpeed(GameManager.Instance.GetAttractSpeed() * attractSpeedMultiplier);
            Destroy(gameObject);
        }
    }
}
