using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;


// Questa classe rappresenta una singola "tappa" della sequenza: un video, un dialogo e (opzionale) una scena da caricare dopo
[System.Serializable]
public class VideoDialogueSequence
{
    [Header("Video da riprodurre in questa tappa")]
    public VideoClip videoClip;

    [Header("Dialogo da mostrare sopra il video")]
    public Dialogue dialogue;

    [Header("Nome della scena da caricare dopo questo step (opzionale)")]
    public string nextSceneName; // Lascia vuoto se non vuoi cambiare scena dopo questo step

    [Header("Il video deve essere in loop?")]
    [Tooltip("Se abilitato, questo video verrà riprodotto in loop fino alla fine del dialogo. ATTENZIONE: Se questo video è in loop e non ci sono dialoghi, la sequenza non avanzerà mai automaticamente!")]
    public bool loopVideo = false;

    [Header("Avanza al prossimo step alla fine del dialogo?")]
    [Tooltip("Se abilitato e il video NON è in loop, la sequenza avanzerà subito dopo la fine del dialogo. Se disabilitato, avanzerà solo alla fine del video.")]
    public bool advanceOnDialogueEnd = false;
}


/// <summary>
/// Controller per gestire una sequenza di video e dialoghi in modo parametrico.
/// Da collegare a un GameObject nella scena (es: "VideoDialogueSequenceController").
/// 
/// Gerarchia consigliata:
/// - Un GameObject con questo script (es: "VideoDialogueSequenceController")
/// - Un componente VideoPlayer (sullo stesso GameObject o referenziato)
/// - Un Canvas UI con il DialogueManager già configurato
/// </summary>
public class VideoDialogueSequenceController : MonoBehaviour
{
    [Header("VideoPlayer da usare per la riproduzione dei video")]
    [Tooltip("Trascina qui il componente VideoPlayer che riproduce i video sullo schermo")]
    public VideoPlayer videoPlayer;

    // RIMOSSO: la logica ora è per-sequenza, vedi advanceOnDialogueEnd in VideoDialogueSequence

    [Header("⚠️ Attenzione ai possibili bug!")]
    [Tooltip("Se una sequenza ha il video in loop e non ha dialoghi, la sequenza non avanzerà mai automaticamente. Inoltre, 'Avanza al prossimo step alla fine del dialogo' viene ignorato per le sequenze con video in loop.")]
    [SerializeField] private string inspectorBugNote = "Se una sequenza ha video in loop e nessun dialogo, non avanzerà mai. 'Avanza al prossimo step alla fine del dialogo' viene ignorato per i video in loop.";

    [Header("Lista delle sequenze Video+Dialogo")]
    [Tooltip("Aggiungi qui tutte le tappe della sequenza: per ogni step, scegli il video, il dialogo e (opzionale) la scena da caricare dopo")]
    public List<VideoDialogueSequence> sequences;

    [Header("Indice della sequenza da cui partire (default 0)")]
    public int startSequenceIndex = 0;

    [Header("Nota importante")]
    [Tooltip("Se l'ultimo video della sequenza è impostato in loop e non ha dialoghi associati, la sequenza non avanzerà automaticamente alla scena successiva.")]
    [SerializeField] private string inspectorNote = "Se l'ultimo video è in loop e non ci sono dialoghi, non si va alla scena successiva.";

    // Indice della sequenza attuale
    private int currentIndex = 0;

    // Stato di pausa
    private bool isPaused = false;

    void Start()
    {
        // Se non è stato assegnato un VideoPlayer, prova a prenderlo dallo stesso GameObject
        if (videoPlayer == null)
            videoPlayer = GetComponent<VideoPlayer>();

        // Inizia dalla sequenza desiderata
        currentIndex = startSequenceIndex;
        PlayCurrentSequence();
    }

    void Update()
    {
        // Gestione input ESC per pausa
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        if (isPaused)
            ResumeSequence();
        else
            PauseSequence();
    }

    // Metodo pubblico per mettere in pausa la sequenza (usabile da altri script)
    public void PauseSequence()
    {
        isPaused = true;
        if (videoPlayer != null && videoPlayer.isPlaying)
            videoPlayer.Pause();
        // DialogueManager.Instance.PauseDialogue(); // Da implementare se serve
    }

    // Metodo pubblico per riprendere la sequenza (usabile da altri script)
    public void ResumeSequence()
    {
        isPaused = false;
        if (videoPlayer != null && !videoPlayer.isPlaying)
            videoPlayer.Play();
        // DialogueManager.Instance.ResumeDialogue(); // Da implementare se serve
    }

    /// <summary>
    /// Avvia la sequenza corrente: riproduce il video e, dopo 2 secondi, mostra il dialogo associato
    /// </summary>
    void PlayCurrentSequence()
    {
        if (currentIndex >= sequences.Count)
            return;

        var seq = sequences[currentIndex];
        videoPlayer.clip = seq.videoClip;
        videoPlayer.isLooping = seq.loopVideo;
        videoPlayer.Play();
        // Dopo 2 secondi dall'inizio del video, parte il dialogo
        StartCoroutine(StartDialogueAfterDelay(2f));
    }

    /// <summary>
    /// Attende un certo delay, poi avvia il dialogo sopra il video
    /// </summary>
    IEnumerator StartDialogueAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        var seq = sequences[currentIndex];
        // Avvia il dialogo tramite DialogueManager (deve essere già presente in scena)
        DialogueManager.Instance.StartDialogue(seq.dialogue);
        // Attende la fine del dialogo prima di proseguire
        StartCoroutine(WaitForDialogueEnd());
    }

    /// <summary>
    /// Attende che il dialogo sia terminato, poi si mette in ascolto della fine del video
    /// </summary>
    IEnumerator WaitForDialogueEnd()
    {
        while (DialogueManager.Instance.isDialogueActive)
            yield return null;

        var seq = sequences[currentIndex];
        bool isLastSequence = (currentIndex >= sequences.Count - 1);

        if (seq.loopVideo)
        {
            // Se il video è in loop, termina solo alla fine del dialogo
            videoPlayer.isLooping = false;
            videoPlayer.Stop();
            AdvanceOrLoadScene(seq, isLastSequence);
        }
        else if (seq.advanceOnDialogueEnd)
        {
            AdvanceOrLoadScene(seq, isLastSequence);
        }
        else
        {
            // Quando il dialogo è finito, aspetta la fine del video (solo se NON è in loop)
            videoPlayer.loopPointReached += OnVideoEnd;
        }
    }

    /// <summary>
    /// Chiamato automaticamente quando il video termina
    /// </summary>
    void OnVideoEnd(VideoPlayer vp)
    {
        videoPlayer.loopPointReached -= OnVideoEnd;
        var seq = sequences[currentIndex];
        bool isLastSequence = (currentIndex >= sequences.Count - 1);
        // Se il video è in loop, non deve mai avanzare da qui (ma solo da fine dialogo)
        if (!seq.loopVideo)
        {
            AdvanceOrLoadScene(seq, isLastSequence);
        }
        // Se è in loop, non fa nulla: l'avanzamento avviene solo alla fine del dialogo
    }

    // Gestisce avanzamento sequenza o caricamento scena
    void AdvanceOrLoadScene(VideoDialogueSequence seq, bool isLastSequence)
    {
        if (!string.IsNullOrEmpty(seq.nextSceneName))
        {
            SceneManager.LoadScene(seq.nextSceneName);
        }
        else if (!isLastSequence)
        {
            currentIndex++;
            PlayCurrentSequence();
        }
        // Se è l'ultima sequenza e non c'è scena, termina semplicemente
    }
}
