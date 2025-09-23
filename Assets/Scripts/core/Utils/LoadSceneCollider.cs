using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneCollider : MonoBehaviour
{

  
    [SerializeField] private bool isEndingScene = false;
    [SerializeField] private string sceneToLoad = "GameScene_second";

   
   private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        { SceneManager.LoadScene("SadEnding"); } /* 
          
            if (isEndingScene && GameManager.Instance.isCatSaved())
            {
                Debug.Log("Caricamento scena HappyEnding");
                UnityEngine.SceneManagement.SceneManager.LoadScene("HappyEnding");
            }
            else if (isEndingScene && !GameManager.Instance.isCatSaved())
            {
                Debug.Log("Caricamento scena BadEnding");
                UnityEngine.SceneManagement.SceneManager.LoadScene("BadEnding");

            }*/
        else
            SceneManager.LoadScene(sceneToLoad);
        }
    }

