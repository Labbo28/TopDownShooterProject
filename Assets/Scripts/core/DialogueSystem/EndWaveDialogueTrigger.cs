using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

/*
============================================
  END WAVE DIALOGUE TRIGGER
============================================

Questo script gestisce il dialogo alla fine di tutte le wave e il successivo cambio scena.
Aggiungi questo script a un GameObject nella GameScene.

IMPOSTA DA INSPECTOR:
- dialogue: Il Dialogue da mostrare alla fine delle wave
- nextSceneName: Nome della scena successiva (es. "PlainToForest")
*/

public class EndWaveDialogueTrigger : MonoBehaviour
{   
    [Header("Riferimento allo Spawner")]
    [SerializeField] private Spawner spawner;
    [Header("Dialogo di fine wave")]
    public Dialogue dialogue;

    [Header("Scena successiva")]
    public string nextSceneName = "PlainToForest";

    [Header("Delay prima del cambio scena (secondi)")]
    public float sceneChangeDelay = 1f;

    private bool hasTriggered = false;

    void Start()
    {
        // Sottoscrivi all'evento di fine wave
        if (GameManager.Instance != null)
        {
            spawner.OnAllWavesCompleted += OnAllWavesCompleted;
        }
    }

    void OnDestroy()
    {
        // Rimuovi la sottoscrizione per evitare memory leak
        if (GameManager.Instance != null)
        {
            spawner.OnAllWavesCompleted -= OnAllWavesCompleted;
        }
    }

    private void OnAllWavesCompleted()
    {
        // Evita che si triggerri più volte
        if (hasTriggered) return;
        hasTriggered = true;

        // Avvia il dialogo se è configurato
        if (dialogue != null && DialogueManager.Instance != null)
        {
            // Avvia la coroutine che gestisce dialogo + cambio scena
            StartCoroutine(HandleEndWaveSequence());
        }
        else
        {
            // Se non c'è dialogo, cambia scena direttamente
            ChangeScene();
        }
    }

    private IEnumerator HandleEndWaveSequence()
    {
        // Avvia il dialogo
        DialogueManager.Instance.StartDialogue(dialogue);

        // Aspetta che il dialogo finisca
        while (DialogueManager.Instance.isDialogueActive)
        {
            yield return null; // Aspetta un frame
        }

        // Piccolo delay opzionale prima del cambio scena
        if (sceneChangeDelay > 0)
        {
            yield return new WaitForSeconds(sceneChangeDelay);
        }

        // Cambia scena
        ChangeScene();
    }

    private void ChangeScene()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogWarning("EndWaveDialogueTrigger: nextSceneName non è impostato.");
        }
    }
}