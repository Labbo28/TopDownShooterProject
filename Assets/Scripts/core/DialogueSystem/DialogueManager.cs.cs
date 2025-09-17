using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

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
	public static DialogueManager Instance;

	public Image characterIcon;
	public TextMeshProUGUI characterName;
	public TextMeshProUGUI dialogueArea;

	public GameObject dialoguePanel; // UI panel da attivare/disattivare
	
	private Queue<DialogueLine> lines;
	private InputSystem_Actions inputActions;
	private bool waitingForInput = false;
	private bool isTyping = false;
	private DialogueLine currentLine;

	public bool isDialogueActive = false;

	public float typingSpeed = 0.2f;

    private void Awake()
    {
		if (Instance == null)
			Instance = this;

		lines = new Queue<DialogueLine>();
		inputActions = new InputSystem_Actions();

		if (dialoguePanel != null)
			dialoguePanel.SetActive(false); // Assicura che il pannello sia nascosto all'avvio
    }

	private void OnEnable()
	{
		inputActions?.Enable();
		inputActions.UI.Submit.performed += OnSubmitPressed;
	}

	private void OnDisable()
	{
		inputActions?.Disable();
		if (inputActions != null)
			inputActions.UI.Submit.performed -= OnSubmitPressed;
	}

	private void OnDestroy()
	{
		inputActions?.Dispose();
	}

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

	public void DisplayNextDialogueLine()
	{
		if (lines.Count == 0)
		{
			EndDialogue();
			return;
		}

		currentLine = lines.Dequeue();

		characterIcon.sprite = currentLine.character.icon;
		characterName.text = currentLine.character.name;

		StopAllCoroutines();

		StartCoroutine(TypeSentence(currentLine));
	}

	IEnumerator TypeSentence(DialogueLine dialogueLine)
	{
		isTyping = true;
		waitingForInput = false;
		dialogueArea.text = "";
		
		foreach (char letter in dialogueLine.line.ToCharArray())
		{
			dialogueArea.text += letter;
			yield return new WaitForSecondsRealtime(typingSpeed);
		}
		
		isTyping = false;
		waitingForInput = true;
	}

	private void OnSubmitPressed(InputAction.CallbackContext context)
	{
		if (!isDialogueActive) return;

		if (isTyping)
		{
			// Se sta scrivendo, completa immediatamente la linea
			CompleteCurrentLine();
		}
		else if (waitingForInput)
		{
			// Se aspetta input, vai alla prossima linea
			DisplayNextDialogueLine();
		}
	}

	private void CompleteCurrentLine()
	{
		StopAllCoroutines();
		isTyping = false;
		waitingForInput = true;
		
		// Mostra l'intera linea immediatamente
		if (currentLine != null)
		{
			dialogueArea.text = currentLine.line;
		}
	}

	void EndDialogue()
	{
		isDialogueActive = false;
		if (dialoguePanel != null)
			dialoguePanel.SetActive(false); // Nasconde il pannello

		// Riprendi il gioco
		Time.timeScale = 1f;
	}
}
