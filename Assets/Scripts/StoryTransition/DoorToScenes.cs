using UnityEngine;

using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DoorToScenes : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Nome della scena da caricare quando il player entra nel trigger")]
    public string sceneToLoad;



    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
    }

    // ...nessun fade, cambio scena immediato...
}
