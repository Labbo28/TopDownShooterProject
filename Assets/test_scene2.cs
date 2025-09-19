using UnityEngine;

public class test_scene2 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.T)){
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene_second");
        }
        else if(Input.GetKeyDown(KeyCode.Y)){
            UnityEngine.SceneManagement.SceneManager.LoadScene("PlainToForest");
        }
    }
}
