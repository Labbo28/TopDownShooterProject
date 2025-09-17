using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MainMenuManager : MonoBehaviour
{
    public UnityEvent onButtonClicked;

    [SerializeField] private Button startButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button optionsButton;
    
    [SerializeField] private GameObject optionsPanel;

    void Start()
    {
        // Add button click listeners
        startButton.onClick.AddListener(OnStartButtonClicked);
        quitButton.onClick.AddListener(OnQuitButtonClicked);
        optionsButton.onClick.AddListener(OnOptionsButtonClicked);
        
        // Add button sound listeners to all buttons
        AddSoundEffectsToButton(startButton);
        AddSoundEffectsToButton(quitButton);
        AddSoundEffectsToButton(optionsButton);
    }

    private void AddSoundEffectsToButton(Button button)
    {
        // Add hover sound
        EventTrigger eventTrigger = button.gameObject.GetComponent<EventTrigger>();
        if (eventTrigger == null)
            eventTrigger = button.gameObject.AddComponent<EventTrigger>();
            
        // Setup pointer enter event (hover)
        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
        pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
        pointerEnterEntry.callback.AddListener((data) => {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayButtonHover();
        });
        eventTrigger.triggers.Add(pointerEnterEntry);
        
        // Add click sound via additional listener
        button.onClick.AddListener(() => {
            if (AudioManager.Instance != null)
                AudioManager.Instance.PlayButtonClick();
        });
    }

    private void OnOptionsButtonClicked()
    {
        onButtonClicked?.Invoke();
        gameObject.SetActive(false);
        optionsButton.gameObject.SetActive(false);
        optionsPanel.SetActive(true);
    }

    private void OnQuitButtonClicked()
    {
        onButtonClicked?.Invoke();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        
        Application.Quit();
    }

    private void OnStartButtonClicked()
    {
        onButtonClicked?.Invoke();
        // Carica la scena di gioco
        LoadingManager.Instance.LoadScene("GameScene");
        if(Player.Instance!= null)
        Player.Instance.gameObject.SetActive(true);
    }
}