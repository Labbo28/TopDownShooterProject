using UnityEngine;

public class LoadSceneCollider : MonoBehaviour
{

    [SerializeField] private string sceneToLoad = "GameScene_second";
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
        }
    }
}