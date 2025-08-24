using UnityEngine;

public class GameOverUI : MonoBehaviour
{

    [SerializeField] GameObject panel;       // Il Panel nero
    //[SerializeField] Text message;           // “You Died”
    // [SerializeField] float delay = 5f;       // Tempo prima del menu


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        panel.SetActive(false);   // Disattivato all’avvio        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
}
