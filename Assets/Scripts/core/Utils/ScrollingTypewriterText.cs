using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// Assicurati che il RectTransform abbia anchorMin, anchorMax e pivot a (0.5, 0.5) per il centro
// ScrollingTypewriterText
//
// Mostra una riga di testo alla volta (separata da \n), con effetto typewriter e scorrimento orizzontale.
// Quando una riga termina di scorrere, scompare e appare la successiva.
//
// COME USARE:
// - Per andare a capo, inserisci '\n' (senza virgolette) nel campo fullText.
//   Esempio: "Ciao mondo!\nQuesta è la seconda riga."
// - Lo spazio tra le righe è automatico: ogni riga appare da sola, non c'è spazio visivo tra una e l'altra.
//
// PARAMETRI:
// - typeSpeed: velocità effetto macchina da scrivere (secondi per carattere, es: 0.05 = veloce, 0.2 = lento)
// - fadeOutSpeed: velocità con cui la riga scorre via dopo essere stata scritta (pixel al secondo)
// - textRect: il RectTransform del testo (deve essere figlio di un oggetto con RectMask2D per mascherare il testo)
// - tmpText: il componente TMP_Text da animare
//
// Consigliato: imposta anchor e pivot del RectTransform al centro (0.5, 0.5)
public class ScrollingTypewriterText : MonoBehaviour
{
    [Header("Testo e velocità")]
    [Tooltip("Testo da mostrare. Usa \\n per andare a capo e mostrare la riga successiva. Esempio: 'Ciao!\\nSeconda riga.'")]
    [TextArea(3, 10)]
    public string fullText;
    [Tooltip("Velocità effetto macchina da scrivere (secondi per carattere). Più basso = più veloce.")]
    public float typeSpeed = 0.05f;
    [Tooltip("Secondi di attesa dopo la scrittura della riga, prima che scompaia e appaia la successiva.")]
    public float holdTime = 1.0f;

    [Header("Riferimenti (assegna manualmente)")]
    [Tooltip("RectTransform del TMP_Text. Deve essere figlio di un oggetto con RectMask2D per mascherare il testo. Consigliato: anchor e pivot al centro (0.5, 0.5)")]
    public RectTransform textRect;
    [Tooltip("Componente TMP_Text da animare (TextMeshPro)")]
    public TMP_Text tmpText;

    public string nextSceneName;

    private float startX;
    private Coroutine typeCoroutine;
    private bool isFading = false;

    private string[] lines;
    private int currentLine = 0;

    /// <summary>
/// Cambia alla scena successiva. Assicurati di impostare il nome della scena nel campo nextSceneName.
/// </summary>
private void ChangeToNextScene()
{
    if (!string.IsNullOrEmpty(nextSceneName))
    {
        SceneManager.LoadScene(nextSceneName);
    }
    else
    {
        Debug.LogWarning("Nome della scena successiva non specificato in nextSceneName!");
    }
}
    void Awake()
    {
        if (tmpText == null)
            tmpText = GetComponent<TMP_Text>();
        if (textRect == null && tmpText != null)
            textRect = tmpText.rectTransform;
        // Imposta ancoraggio e pivot al centro (opzionale, ma consigliato)
        if (textRect != null)
        {
            textRect.anchorMin = new Vector2(0.5f, textRect.anchorMin.y);
            textRect.anchorMax = new Vector2(0.5f, textRect.anchorMax.y);
            textRect.pivot = new Vector2(0.5f, textRect.pivot.y);
        }
    }

    void OnEnable()
    {
        ResetText();
        lines = fullText.Replace("\\n", "\n").Split('\n');
        currentLine = 0;
        typeCoroutine = StartCoroutine(TypeAndScrollLines());
    }

    public void ResetText()
    {
        if (typeCoroutine != null)
            StopCoroutine(typeCoroutine);
        tmpText.text = "";
        if (textRect != null)
            textRect.anchoredPosition = new Vector2(0, textRect.anchoredPosition.y); // sempre centro
        isFading = false;
    }

    // Mostra una riga alla volta, ognuna scorre orizzontalmente e poi scompare
    IEnumerator TypeAndScrollLines()
    {
        while (currentLine < lines.Length)
        {
            yield return StartCoroutine(TypeText(lines[currentLine]));
            yield return new WaitForSeconds(holdTime); // attesa dopo la scrittura
            tmpText.text = ""; // scompare istantaneamente
            currentLine++;
        }
        tmpText.text = "";
        ChangeToNextScene();
    }

    IEnumerator TypeText(string line)
    {
        tmpText.text = "";
        if (textRect == null) yield break;
        // Mantieni la posizione X sempre a 0 (centro)
        textRect.anchoredPosition = new Vector2(0, textRect.anchoredPosition.y);
        startX = 0f;
        for (int i = 0; i < line.Length; i++)
        {
            tmpText.text += line[i];
            yield return new WaitForSeconds(typeSpeed);
            // Scroll orizzontale: sposta solo se il testo supera la larghezza della maschera
            float width = tmpText.preferredWidth;
            float offset = Mathf.Max(0, width - textRect.rect.width);
            textRect.anchoredPosition = new Vector2(-offset, textRect.anchoredPosition.y);
        }
        // Nessun fade, la riga resta visibile per holdTime secondi (gestito sopra)
    }

    // FadeOutText non più usato: la riga scompare istantaneamente dopo holdTime
}
