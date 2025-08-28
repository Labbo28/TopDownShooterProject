using System;
using System.Collections;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
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

    [SerializeField] GameObject TextDead;
    [SerializeField] GameObject RetryButton;
    [SerializeField] GameObject QuitButton;

    private void Start()
    {
        gameManager.OnEnemyKilled.AddListener(OnEnemyKilled);
        gameManager.OnGameTimeChanged.AddListener(OnGameTimeChanged);
        gameManager.OnXPChanged.AddListener(OnXPChanged);
        gameManager.OnPlayerLevelUp.AddListener(OnPlayerLevelUp);
        gameManager.OnGameOver.AddListener(OnGameOver);
        RetryButton.GetComponent<Button>().onClick.AddListener(OnRetryButtonClicked);
        QuitButton.GetComponent<Button>().onClick.AddListener(OnQuitButtonClicked);
    }

    private void OnRetryButtonClicked()
    {
        Debug.Log("Retrying...");
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }

    private void OnQuitButtonClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    private void OnGameOver()
    {
        Debug.Log("GAME OVER!");

        if (TextDead != null)
        {
            StartCoroutine(FadeInGameOver());
        }
        RetryButton.SetActive(true);
        QuitButton.SetActive(true);
    
}

private IEnumerator FadeInGameOver()
{
    CanvasGroup cg = TextDead.GetComponent<CanvasGroup>();
    if (cg == null)
    {
        // Se non c'è CanvasGroup, mostra subito
        TextDead.SetActive(true);
        yield break;
    }
    
    TextDead.SetActive(true);
    cg.alpha = 0f;
    
    float duration = 2f;
    float elapsedTime = 0f;
    
    while (elapsedTime < duration)
    {
        elapsedTime += Time.deltaTime;
        cg.alpha = Mathf.Lerp(0f, 1f, elapsedTime / duration); // ✅ Parametri corretti
        yield return null; // ✅ Non blocca il gioco
    }
    
    cg.alpha = 1f; // Assicura che sia completamente visibile
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
