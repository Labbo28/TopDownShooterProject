using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WaveChangerSceneLoad : MonoBehaviour
{
    [SerializeField] private Spawner spawner;
    [SerializeField] private string sceneToLoad="PlainToForest";

    private bool isCatSaved = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isCatSaved = GameManager.Instance.isCatSaved();
        spawner.OnAllWavesCompleted += ChangeScene;
        
    }

    private void ChangeScene()
    {
       
        //TODO: Logica cambio scena gatto e collare 
        SceneManager.LoadScene(sceneToLoad);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
