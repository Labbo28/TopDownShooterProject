using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    [Header("Loading Screen")]
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private Slider progressBar;
    [SerializeField] private Text progressText;
    
    private static LoadingManager instance;
    
    // Singleton pattern per accedere al LoadingManager da qualsiasi script
    public static LoadingManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<LoadingManager>();
                
                if (instance == null)
                {
                    GameObject obj = new GameObject("LoadingManager");
                    instance = obj.AddComponent<LoadingManager>();
                }
            }
            
            return instance;
        }
    }
    
    private void Awake()
    {
        // Mantiene questo oggetto tra le scene
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        
        // Nasconde la schermata di caricamento all'avvio
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }
    
    // Metodo pubblico per caricare una scena con la schermata di caricamento
    public void LoadScene(string sceneName)
    {
        loadingScreen.SetActive(true);
        StartCoroutine(LoadSceneAsync(sceneName));
    }
    
    // Coroutine per caricare la scena in modo asincrono e mostrare il progresso
    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // Mostra la schermata di caricamento
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(true);
        }
        
        // Avvia il caricamento asincrono della scena
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        
        // Non permettere che la scena si attivi fino a quando non è completamente caricata
        asyncLoad.allowSceneActivation = false;
        
        // Monitora il progresso del caricamento
        while (!asyncLoad.isDone)
        {
            // Calcola il progresso da 0 a 0.9 (Unity riserva il 10% finale per l'attivazione)
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            
            // Aggiorna l'UI del progresso
            if (progressBar != null)
            {
                progressBar.value = progress;
            }
            
            if (progressText != null)
            {
                progressText.text = $"{Mathf.Round(progress * 100)}%";
            }
            
            // Quando il caricamento è al 90%, attiva la scena
            if (asyncLoad.progress >= 0.9f)
            {
                // Piccolo ritardo per vedere il 100%
                yield return new WaitForSeconds(0.5f);
                asyncLoad.allowSceneActivation = true;
            }
            
            yield return null;
        }
        
        // Nascondi la schermata di caricamento
        if (loadingScreen != null)
        {
            loadingScreen.SetActive(false);
        }
    }
}