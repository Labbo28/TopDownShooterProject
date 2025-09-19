using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//let camera follow target
public class CameraFollowPlayer : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float lerpSpeed = 2.0f;

    private Vector3 offset;
    private Vector3 targetPos;

    private void Awake()
    {
        // Iscriviti all'evento di caricamento scena
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Quando una scena viene caricata, cerca di nuovo il player
        if (scene.name == "GameScene_second" || scene.name == "GameScene")
        {
            FindPlayer();
        }
    }

    private void Start()
    {
        FindPlayer();
    }

    private void FindPlayer()
    {
        // Prima prova con il Singleton
        if (Player.Instance != null)
        {
            target = Player.Instance.transform;
           
        }
        else
        {
            // Fallback: cerca con il tag
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
                
            }
            else
            {
              
                Invoke(nameof(FindPlayer), 0.1f);
                return;
            }
        }

        if (target != null)
        {
            // Calcola l'offset dalla posizione corrente della camera
            offset = transform.position - target.position;
        }
    }

    private void Update()
    {
        if (target == null)
        {
            // Se il target è nullo, prova a cercarlo di nuovo
            FindPlayer();
            return;
        }

        targetPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetPos, lerpSpeed * Time.deltaTime);
    }

    // Metodo pubblico per impostare manualmente il target (utile per debug)
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }

    private void OnDestroy()
    {
        // Cleanup dell'evento
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}