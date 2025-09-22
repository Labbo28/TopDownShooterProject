using UnityEngine;

public class LoadSceneCollider : MonoBehaviour
{

  
    [SerializeField] private bool isEndingScene = false;
    [SerializeField] private string sceneToLoad = "GameScene_second";

   
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isEndingScene && GameManager.Instance.isCatSaved())
            {
                Debug.Log("Caricamento scena HappyEnding");
                UnityEngine.SceneManagement.SceneManager.LoadScene("HappyEnding");
            }
            else if (isEndingScene && !GameManager.Instance.isCatSaved())
            {
                Debug.Log("Caricamento scena BadEnding");
                UnityEngine.SceneManagement.SceneManager.LoadScene("BadEnding");
                
            }
            else
                UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
        }
    }
}