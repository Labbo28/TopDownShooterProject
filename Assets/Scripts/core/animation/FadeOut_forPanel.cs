using UnityEngine;
using UnityEngine.UI;

public class FadeOut_forPanel : MonoBehaviour
{
    // inserito all interno di un panel per farlo scomparire
    // con un effetto di dissolvenza a inizio scena con parametri sul colore e durata da gestire da inspector

    [SerializeField] private Color fadeColor = Color.black;
    [SerializeField] private float fadeDuration = 1.5f;

    private Image panelImage;
    private float timer = 0f;
    private bool isFading = true;
    private Color startColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        panelImage = GetComponent<Image>();
        if (panelImage == null)
        {
            Debug.LogError("FadeOut_forPanel: Nessun componente Image trovato sul pannello!");
            enabled = false;
            return;
        }
        startColor = fadeColor;
        startColor.a = 1f;
        panelImage.color = startColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isFading) return;

        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
        Color newColor = fadeColor;
        newColor.a = alpha;
        panelImage.color = newColor;

        if (timer >= fadeDuration)
        {
            panelImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
            isFading = false;
            // Se vuoi disattivare il pannello dopo la dissolvenza:
            // gameObject.SetActive(false);
        }
    }
}
