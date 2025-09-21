using System;
using UnityEngine;
using UnityEngine.SceneManagement;
public class WaveChangerSceneLoad : MonoBehaviour
{
    [SerializeField] private Spawner spawner;
    [SerializeField] private string sceneToLoad="PlainToForest";
    
    private bool isCatSaved;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawner.OnAllWavesCompleted += ChangeScene;
        isCatSaved = GameManager.Instance.isCatSaved();
    }

    private void ChangeScene()
    {
        if (sceneToLoad == "ForestToPlain")
            GameManager.Instance.GameOver();
        Debug.Log("Changing Scene to "+sceneToLoad);

        if (isCatSaved)
        {
            Debug.Log("Cat is saved, loading the good ending");
            sceneToLoad = "FirtEndingVideoHappy";

        }
        else
        {
            Debug.Log("Cat is not saved, loading the bad ending");
            sceneToLoad = "SecondEndingVideoSad";
        }
        SceneManager.LoadScene(sceneToLoad);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
