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
        gameManager.OnEnemyKilled.AddListener(OnEnemyKilled);
        gameManager.OnGameTimeChanged.AddListener(OnGameTimeChanged);
        gameManager.OnXPChanged.AddListener(OnXPChanged);
        gameManager.OnPlayerLevelUp.AddListener(OnPlayerLevelUp);
    }

    private void OnPlayerLevelUp(int level)
    {
        TextLevel.GetComponent<Text>().text = "Lv." + level.ToString();
    }

    private void OnXPChanged(float xp)
    {
        SliderXP.GetComponent<Slider>().value = xp/gameManager.GetXPToLevelUp();
    }

    private void OnGameTimeChanged()
    {
        TextTime.GetComponent<Text>().text = gameManager.getFormattedGameTime();
    }

    private void OnEnemyKilled()
    {
        TextKill.GetComponent<Text>().text = gameManager.getEnemiesKilled().ToString();   
    }

    private void OnDestroy()
    {
        if (gameManager != null)
        {
            gameManager.OnEnemyKilled.RemoveListener(OnEnemyKilled);
            gameManager.OnGameTimeChanged.RemoveListener(OnGameTimeChanged);
            gameManager.OnXPChanged.RemoveListener(OnXPChanged);
            gameManager.OnPlayerLevelUp.RemoveListener(OnPlayerLevelUp);
        }
    }
}
