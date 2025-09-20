using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

/// <summary>
/// DialogueManager gestisce la visualizzazione dei dialoghi a schermo con effetto macchina da scrivere.
/// Mostra una riga alla volta, con supporto per cambio riga manuale (\n) e cambio automatico se il testo supera la larghezza.
/// Blocca il gioco durante il dialogo e gestisce input per avanzare.
/// </summary>
public class DialogueManager : MonoBehaviour
{
		/*
		============================================
			DIALOGUE MANAGER - USO E LOGICA
		============================================

		Questo script gestisce il sistema di dialoghi nel gioco.

		COMPONENTI DA COLLEGARE NELL'INSPECTOR:
		------------------------------------------------------------
		1. characterIcon: Image UI che mostra l'icona del personaggio che parla.
		2. characterName: TextMeshProUGUI che mostra il nome del personaggio.
		3. dialogueArea: TextMeshProUGUI che mostra la frase del dialogo.
		4. dialoguePanel: GameObject UI che rappresenta il box del dialogo da mostrare/nascondere.

		LOGICA DI FUNZIONAMENTO:
		------------------------------------------------------------
		• DialogueManager è un singleton (Instance) accessibile da altri script.
		• Quando viene chiamato StartDialogue(dialogue), il box dialogo (dialoguePanel) si attiva e vengono messe in coda tutte le linee del dialogo.
		• DisplayNextDialogueLine() mostra la prossima linea, aggiorna icona/nome e avvia la scrittura "a macchina" del testo.
		• TypeSentence() scrive la frase lettera per lettera con la velocità impostata da typingSpeed.
		• Quando le linee finiscono, EndDialogue() disattiva il box dialogo e imposta isDialogueActive a false.

		COME IMPOSTARE I TRIGGER:
		------------------------------------------------------------
		• Usa DialogueTrigger (vedi altro script) su un oggetto con Collider2D.
		• Imposta il campo "dialogue" con le linee e i personaggi desiderati.
		• Quando il player entra nel trigger (OnTriggerEnter2D), viene chiamato DialogueManager.Instance.StartDialogue(dialogue).

		• Puoi anche chiamare DialogueManager.Instance.StartDialogue(dialogue) da altri script per avviare dialoghi in modo personalizzato.

		NOTE:
		• Assicurati che il Canvas UI abbia i riferimenti corretti a characterIcon, characterName, dialogueArea e dialoguePanel.
		• Il sistema supporta personaggi multipli e linee personalizzate.
		*/

	/// <summary>
	/// Singleton per accedere facilmente al DialogueManager da altri script.
	/// </summary>
	public static DialogueManager Instance;

	[Header("Riferimenti UI (assegna nell'Inspector)")]
	[Tooltip("Image UI che mostra l'icona del personaggio che parla.")]
	public Image characterIcon;

	[Tooltip("TextMeshProUGUI che mostra il nome del personaggio.")]
	public TextMeshProUGUI characterName;

	[Tooltip("TextMeshProUGUI che mostra la frase del dialogo.")]
	public TextMeshProUGUI dialogueArea;

	[Tooltip("GameObject UI che rappresenta il box del dialogo da mostrare/nascondere.")]
	public GameObject dialoguePanel;

	// Coda delle linee di dialogo da mostrare
	private Queue<DialogueLine> lines;
	// Gestione input (nuovo sistema InputSystem)
	private InputSystem_Actions inputActions;
	// True se si aspetta input per avanzare
	private bool waitingForInput = false;
	// True se sta scrivendo la riga
	private bool isTyping = false;
	// Linea attualmente visualizzata
	private DialogueLine currentLine;

	[Header("Stato del dialogo (runtime)")]
	[Tooltip("True se un dialogo è attivo e il gioco è in pausa.")]
	public bool isDialogueActive = false;


	[Header("Impostazioni effetto macchina da scrivere")]
	[Tooltip("Velocità di scrittura (secondi per carattere). Più basso = più veloce.")]
	public float typingSpeed = 0.2f;

	[Tooltip("Secondi di attesa dopo la scrittura della riga, prima che venga cancellata e appaia la successiva.")]
	public float holdTime = 1.0f;

	// RectTransform del campo testo (per calcolare larghezza)
	private RectTransform dialogueRect;

	/// <summary>
	/// Inizializza singleton, input e riferimenti UI.
	/// </summary>
	private void Awake()
    {
		if (Instance == null)
			Instance = this;

		lines = new Queue<DialogueLine>();
		inputActions = new InputSystem_Actions();

		// Nascondi il pannello dialogo all'avvio
		if (dialoguePanel != null)
			dialoguePanel.SetActive(false);

		// Prendi il RectTransform del dialogueArea per calcolare la larghezza
		if (dialogueArea != null)
			dialogueRect = dialogueArea.GetComponent<RectTransform>();
	}

	/// <summary>
	/// Abilita il sistema di input e collega il submit.
	/// </summary>
	private void OnEnable()
	{
		inputActions?.Enable();
		inputActions.UI.Submit.performed += OnSubmitPressed;
	}

	/// <summary>
	/// Disabilita il sistema di input e scollega il submit.
	/// </summary>
	private void OnDisable()
	{
		inputActions?.Disable();
		if (inputActions != null)
			inputActions.UI.Submit.performed -= OnSubmitPressed;
	}

	/// <summary>
	/// Libera le risorse dell'input system.
	/// </summary>
	private void OnDestroy()
	{
		inputActions?.Dispose();
	}

	/// <summary>
	/// Avvia un nuovo dialogo: mette in coda tutte le linee, mostra il pannello e blocca il gioco.
	/// </summary>
	public void StartDialogue(Dialogue dialogue)
	{
		isDialogueActive = true;

		// Blocca il gioco
		Time.timeScale = 0f;

		if (dialoguePanel != null)
			dialoguePanel.SetActive(true); // Mostra il pannello

		lines.Clear();

		foreach (DialogueLine dialogueLine in dialogue.dialogueLines)
		{
			lines.Enqueue(dialogueLine);
		}

		DisplayNextDialogueLine();
	}

	/// <summary>
	/// Mostra la prossima linea di dialogo, aggiorna UI e avvia la scrittura.
	/// </summary>
	public void DisplayNextDialogueLine()
	{
		if (lines.Count == 0)
		{
			EndDialogue();
			return;
		}

		currentLine = lines.Dequeue();

		if (currentLine.character.icon != null)
		{
			characterIcon.gameObject.SetActive(true);
			characterIcon.sprite = currentLine.character.icon;
		}
		else
		{
			characterIcon.gameObject.SetActive(false);
		}
		characterName.text = currentLine.character.name;

		StopAllCoroutines();

		StartCoroutine(TypeSentence(currentLine));
	}

	/// <summary>
	/// Effetto macchina da scrivere: scrive una riga alla volta, gestendo accapo manuale (\n) e cambio automatico.
	/// Ogni riga viene mostrata da sola, la precedente scompare.
	/// </summary>
	IEnumerator TypeSentence(DialogueLine dialogueLine)
	{
    isTyping = true;
    waitingForInput = false;
    dialogueArea.text = "";

    string[] lines = dialogueLine.line.Replace("\\n", "\n").Split('\n');
    for (int l = 0; l < lines.Length; l++)
    {
        dialogueArea.text = "";
        if (dialogueRect != null)
            dialogueRect.anchoredPosition = new Vector2(0, dialogueRect.anchoredPosition.y);

        string line = lines[l];
        foreach (char letter in line)
        {
            dialogueArea.text += letter;
            yield return new WaitForSecondsRealtime(typingSpeed);
        }
        // Attendi il tempo impostato dopo la scrittura della riga, prima di cancellarla
        float timer = 0f;
        while (timer < holdTime)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
    }

    isTyping = false; // <-- Spostato qui, DOPO la pausa!
    waitingForInput = true;
}

	/// <summary>
	/// Gestisce l'input di submit: se sta scrivendo completa la riga, altrimenti avanza.
	/// </summary>
	private void OnSubmitPressed(InputAction.CallbackContext context)
	{
    if (!isDialogueActive) return;

    // Se sta scrivendo, ignora l'input!
    if (isTyping)
    {
        // Ignora input durante la scrittura
        return;
    }
    else if (waitingForInput)
    {
        // Solo ora si può andare avanti
        DisplayNextDialogueLine();
    }
}

	public void OnDialogueAdvanceButton()
	{
		if (!isDialogueActive) return;
		if (isTyping) return;
		if (waitingForInput)
			DisplayNextDialogueLine();
	}

	/// <summary>
	/// Termina il dialogo: nasconde il pannello e riprende il gioco.
	/// </summary>
	void EndDialogue()
	{
		isDialogueActive = false;
		if (dialoguePanel != null)
			dialoguePanel.SetActive(false); // Nasconde il pannello

		// Nessun reset necessario: il testo resta fermo

		// Riprendi il gioco
		Time.timeScale = 1f;
	}
}
