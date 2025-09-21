using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WaveChangerSceneLoad : MonoBehaviour
{
    [SerializeField] private Spawner spawner;
    [SerializeField] private string sceneToLoad="PlainToForest";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawner.OnAllWavesCompleted += ChangeScene;
    }

    private void ChangeScene()
    {
        Debug.Log("Changing Scene to "+sceneToLoad);
        SceneManager.LoadScene(sceneToLoad);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
