using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Enemy : MonoBehaviour
{
    public float speed;
    Transform player;
    void Start (){
     player = FindObjectOfType<PlayerController>().transform;
    }
    void Update(){
        transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
    }
}
