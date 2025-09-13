using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ScreenFader : MonoBehaviour
{
    private Image fadeImage;
    private CanvasGroup fadeGroup;

    public float fadeDuration = 3f;

    private void Awake()
    {
        // Recupera entrambi i componenti dal GameObject del Canvas
        fadeImage = GetComponent<Image>();
        fadeGroup = GetComponent<CanvasGroup>();

        if (fadeImage == null)
            Debug.LogError("Nessun componente Image trovato sul GameObject.");
        if (fadeGroup == null)
            Debug.LogError("Nessun componente CanvasGroup trovato sul GameObject.");

        // Assicurati che il canvas parta trasparente
        fadeGroup.alpha = 0f;
    }

    private void Start()
    {
        GameManager.Instance.OnGameOver.AddListener(FadeIn);
    }

    public void FadeIn()
    {
        StartCoroutine(Fade(0f, 1f));
    }

    public void FadeOut()
    {
        StartCoroutine(Fade(1f, 0f));
    }

    private IEnumerator Fade(float start, float end)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            fadeGroup.alpha = Mathf.Lerp(start, end, t);
            yield return null;
        }
    }
}
