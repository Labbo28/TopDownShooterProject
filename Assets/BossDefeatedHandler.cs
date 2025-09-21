using System;
using UnityEngine;

public class BossDefeatedHandler : MonoBehaviour
{
    [SerializeField] private Spawner spawner;
    [SerializeField] private string sceneToLoad = "ForestToPlain";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawner.OnBossDefeated += ChangeScene;
        
    }

    private void ChangeScene()
    {
        Debug.Log("Changing Scene to " + sceneToLoad);
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
