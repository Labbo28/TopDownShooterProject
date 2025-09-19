using UnityEngine;
using UnityEngine.SceneManagement;

public class skipButtonToGame : MonoBehaviour
{
    [SerializeField] private string sceneName;
    // Questo metodo va collegato all'evento OnClick del bottone nell'Inspector
    public void OnSkipButtonClicked()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(sceneName);
        }
        else
        {
            Debug.LogWarning("Il nome della scena non è stato impostato nello script skipButtonToGame.");
        }
    }
}
