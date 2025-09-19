using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentObject : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "PlainToForest")
        {
            gameObject.SetActive(false); // Nasconde l'oggetto
        }
        else if (scene.name == "GameScene_second")
        {
            gameObject.SetActive(true); // Riappare nelle scene di gioco
        }
    }
}
