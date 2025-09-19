
using UnityEngine;
using UnityEngine.SceneManagement;

using UnityEngine.UI;

public class TitleSizer : MonoBehaviour
{
    [Header("Scaling Settings")]
    [Tooltip("Scala iniziale dell'immagine")] public Vector3 startScale = Vector3.one;
    [Tooltip("Scala finale dell'immagine")] public Vector3 endScale = Vector3.zero;
    [Tooltip("Durata della transizione in secondi")] public float duration = 1.5f;
    [Tooltip("Ritardo prima di iniziare la transizione")] public float delay = 0f;
    [Tooltip("Avvia automaticamente all'avvio")] public bool autoStart = true;

    [Header("UI Settings")]
    [Tooltip("Forza il pivot del RectTransform al centro (solo UI)")]
    public bool forceCenterPivot = true;

    [Header("Scene Transition")]
    [Tooltip("Nome della scena da caricare al termine della transizione. Vuoto = nessuna transizione.")]
    public string sceneToLoad = "";
    [Tooltip("Secondi di attesa dopo la fine della scala prima del cambio scena")]
    public float sceneChangeDelay = 0f;

    [Header("Fade-In Settings")]
    [Tooltip("Abilita effetto dissolvenza all'inizio")]
    public bool enableFadeIn = true;
    [Tooltip("Durata del fade-in in secondi")]
    public float fadeInDuration = 1f;
    [Tooltip("Alpha iniziale (0=trasparente, 1=opaco)")]
    [Range(0,1)] public float fadeStartAlpha = 0f;
    [Tooltip("Alpha finale (0=trasparente, 1=opaco)")]
    [Range(0,1)] public float fadeEndAlpha = 1f;

    private float timer = 0f;
    private bool isScaling = false;
    private Vector3 initialScale;
    private bool isFading = false;
    private float fadeTimer = 0f;
    private Image uiImage;
    private SpriteRenderer spriteRenderer;
    private bool waitingSceneChange = false;
    private float sceneDelayTimer = 0f;

    void Start()
    {
        // Se richiesto, forza il pivot al centro se è un RectTransform (UI)
        if (forceCenterPivot)
        {
            RectTransform rt = GetComponent<RectTransform>();
            if (rt != null)
                rt.pivot = new Vector2(0.5f, 0.5f);
        }

        // Trova componenti grafici
        uiImage = GetComponent<Image>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Imposta alpha iniziale per fade-in
        if (enableFadeIn)
        {
            SetAlpha(fadeStartAlpha);
            isFading = true;
            fadeTimer = 0f;
        }
        else
        {
            SetAlpha(fadeEndAlpha);
            isFading = false;
        }

        transform.localScale = startScale;
        if (autoStart)
        {
            StartScaling();
        }
    }

    public void StartScaling()
    {
        timer = 0f;
        isScaling = true;
        initialScale = startScale;
        transform.localScale = startScale;
        if (delay > 0f)
            Invoke(nameof(BeginScaling), delay);
        else
            BeginScaling();
    }

    private void BeginScaling()
    {
        isScaling = true;
        timer = 0f;
    }

    void Update()
    {
        // Gestione fade-in
        if (isFading)
        {
            fadeTimer += Time.deltaTime;
            float ft = Mathf.Clamp01(fadeTimer / fadeInDuration);
            float alpha = Mathf.Lerp(fadeStartAlpha, fadeEndAlpha, ft);
            SetAlpha(alpha);
            if (ft >= 1f)
            {
                SetAlpha(fadeEndAlpha);
                isFading = false;
            }
        }

        // Gestione attesa per cambio scena
        if (waitingSceneChange)
        {
            sceneDelayTimer += Time.deltaTime;
            if (sceneDelayTimer >= sceneChangeDelay)
            {
                waitingSceneChange = false;
                if (!string.IsNullOrEmpty(sceneToLoad))
                {
                    SceneManager.LoadScene(sceneToLoad);
                }
            }
            return;
        }

        if (!isScaling) return;
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / duration);
        transform.localScale = Vector3.Lerp(startScale, endScale, t);
        if (t >= 1f)
        {
            isScaling = false;
            // Inizia attesa per cambio scena se richiesto
            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                waitingSceneChange = true;
                sceneDelayTimer = 0f;
            }
        }
    }

    // Imposta l'alpha su Image o SpriteRenderer
    private void SetAlpha(float alpha)
    {
        if (uiImage != null)
        {
            Color c = uiImage.color;
            c.a = alpha;
            uiImage.color = c;
        }
        if (spriteRenderer != null)
        {
            Color c = spriteRenderer.color;
            c.a = alpha;
            spriteRenderer.color = c;
        }
    }
}
