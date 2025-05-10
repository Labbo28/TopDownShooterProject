using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
  [SerializeField] private Button startButton;
  [SerializeField] private Button quitButton;

    [SerializeField] private Button optionsButton; 
 

    //da creare 
    [SerializeField] private GameObject optionsPanel;

    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
       
        quitButton.onClick.AddListener(OnQuitButtonClicked);
        optionsButton.onClick.AddListener(OnOptionsButtonClicked);
    }

    private void OnOptionsButtonClicked()
    {
        gameObject.SetActive(false);
        optionsButton.gameObject.SetActive(false);
        optionsPanel.SetActive(true);

    }

    private void OnQuitButtonClicked()
    {
        Debug.Log("Evento agganciato");
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        Debug.Log("Uscita dall'editor Unity");
        #endif

         Application.Quit();
    }

    private void OnStartButtonClicked()
    {
        // Carica la scena di gioco
        LoadingManager.Instance.LoadScene("GameScene");
    }
}
