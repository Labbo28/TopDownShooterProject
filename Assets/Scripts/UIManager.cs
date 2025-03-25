using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
 [SerializeField] GameManager gameManager;
 [SerializeField] Canvas canvas;
 [SerializeField] GameObject TextKill;
 [SerializeField] GameObject TextTime;
 [SerializeField] GameObject TextLevel;
 [SerializeField] GameObject SliderXP;


   private void Awake()
    {
      
       if (gameManager == null)
       Debug.LogWarning("GameManager non assegnato");

             if (canvas == null)
        {
            Debug.LogWarning("Canvas non assegnato");
        }
        if (TextKill == null)
        {
            Debug.LogWarning("TextKill non assegnato");
        }
        if (TextTime == null)
        {
            Debug.LogWarning("TextTime non assegnato");
        }
        if (TextLevel == null)
        {
            Debug.LogWarning("TextLevel non assegnato");
        }
        if (SliderXP == null)
        {
            Debug.LogWarning("SliderXP non assegnato");
        }
       
    }
    private void Start()
    {
         gameManager.OnEnemyKilled += OnEnemyKilled;
         gameManager.OnGameTimeChanged += OnGameTimeChanged;
    }

    private void OnGameTimeChanged(object sender, EventArgs e)
    {
        TextTime.GetComponent<Text>().text = gameManager.getFormattedGameTime();
    }

    private void OnEnemyKilled(object sender, EventArgs e)
    {
       TextKill.GetComponent<Text>().text = gameManager.getEnemiesKilled().ToString();   
    }
}
