using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

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
	4. animator: Animator che gestisce l'apertura/chiusura del box dialogo (animazioni "show" e "hide").

	LOGICA DI FUNZIONAMENTO:
	------------------------------------------------------------
	• DialogueManager è un singleton (Instance) accessibile da altri script.
	• Quando viene chiamato StartDialogue(dialogue), il box dialogo si apre e vengono messe in coda tutte le linee del dialogo.
	• DisplayNextDialogueLine() mostra la prossima linea, aggiorna icona/nome e avvia la scrittura "a macchina" del testo.
	• TypeSentence() scrive la frase lettera per lettera con la velocità impostata da typingSpeed.
	• Quando le linee finiscono, EndDialogue() chiude il box dialogo e imposta isDialogueActive a false.

	COME IMPOSTARE I TRIGGER:
	------------------------------------------------------------
	• Usa DialogueTrigger (vedi altro script) su un oggetto con Collider2D.
	• Imposta il campo "dialogue" con le linee e i personaggi desiderati.
	• Quando il player entra nel trigger (OnTriggerEnter2D), viene chiamato DialogueManager.Instance.StartDialogue(dialogue).

	• Puoi anche chiamare DialogueManager.Instance.StartDialogue(dialogue) da altri script per avviare dialoghi in modo personalizzato.

	NOTE:
	• Assicurati che il Canvas UI abbia i riferimenti corretti a characterIcon, characterName, dialogueArea e animator.
	• Il sistema supporta personaggi multipli e linee personalizzate.
	*/
    public static DialogueManager Instance;

	public Image characterIcon;
	public TextMeshProUGUI characterName;
    public TextMeshProUGUI dialogueArea;

    private Queue<DialogueLine> lines;
    
	public bool isDialogueActive = false;

	public float typingSpeed = 0.2f;

	public Animator animator;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

		lines = new Queue<DialogueLine>();
    }

	public void StartDialogue(Dialogue dialogue)
	{
		isDialogueActive = true;

		animator.Play("show");

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

		DialogueLine currentLine = lines.Dequeue();

		characterIcon.sprite = currentLine.character.icon;
		characterName.text = currentLine.character.name;

		StopAllCoroutines();

		StartCoroutine(TypeSentence(currentLine));
	}

	IEnumerator TypeSentence(DialogueLine dialogueLine)
	{
		dialogueArea.text = "";
		foreach (char letter in dialogueLine.line.ToCharArray())
		{
			dialogueArea.text += letter;
			yield return new WaitForSeconds(typingSpeed);
		}
	}

	void EndDialogue()
	{
		isDialogueActive = false;
		animator.Play("hide");
	}
}
