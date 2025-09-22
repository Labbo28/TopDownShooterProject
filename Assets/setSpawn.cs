using UnityEngine;

public class setSpawn : MonoBehaviour
{
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Player.Instance.transform.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
