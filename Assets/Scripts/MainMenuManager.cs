using System;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
  [SerializeField] private Button startButton;
  [SerializeField] private Button optionsButton;
  
  [SerializeField] private Button quitButton;

    //da creare 
    [SerializeField] private GameObject optionsPanel;

    void Start()
    {
        startButton.onClick.AddListener(OnStartButtonClicked);
        optionsButton.onClick.AddListener(OnOptionsButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    private void OnQuitButtonClicked()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif

         Application.Quit();
    }

    private void OnOptionsButtonClicked()
    {
        throw new NotImplementedException();
    }

    private void OnStartButtonClicked()
    {
        // Carica la scena di gioco
        LoadingManager.Instance.LoadScene("GameScene");
    }
}
