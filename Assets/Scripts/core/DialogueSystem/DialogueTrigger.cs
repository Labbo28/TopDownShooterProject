using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueCharacter
{
    public string name;
    public Sprite icon;
}

[System.Serializable]
public class DialogueLine
{
    public DialogueCharacter character;
    [TextArea(3, 10)]
    public string line;
}

[System.Serializable]
public class Dialogue
{
    public List<DialogueLine> dialogueLines = new List<DialogueLine>();
}

public class DialogueTrigger : MonoBehaviour
{
    /*
    ============================================
      DIALOGUE TRIGGER - USO E LOGICA
    ============================================

    Questo script permette di avviare un dialogo quando il player entra in un trigger.

    COME COLLEGARE E USARE:
    ------------------------------------------------------------
    1. Aggiungi questo script a un GameObject con Collider2D (es. NPC, oggetto interattivo).
    2. Imposta il campo "dialogue" nell'Inspector con le linee e i personaggi desiderati.
       - Puoi creare Dialogue e DialogueLine direttamente nell'Inspector.
       - Ogni DialogueLine ha un personaggio (nome, icona) e una frase.
    3. Assicurati che il Collider2D abbia "Is Trigger" attivo.
    4. Quando il player (tag "Player") entra nel trigger, viene avviato il dialogo tramite DialogueManager.

    LOGICA DI FUNZIONAMENTO:
    ------------------------------------------------------------
    • DialogueCharacter: contiene nome e icona del personaggio che parla.
    • DialogueLine: contiene il personaggio e la frase da mostrare.
    • Dialogue: contiene una lista di DialogueLine che compongono il dialogo.
    • DialogueTrigger: gestisce l'attivazione del dialogo quando il player entra nel trigger.

    • Puoi chiamare TriggerDialogue() anche da altri script per avviare il dialogo manualmente.

    NOTE:
    • Puoi avere più DialogueTrigger nella scena per NPC diversi.
    • Personalizza le linee e i personaggi per ogni trigger.
    */
    public Dialogue dialogue;

    public void TriggerDialogue()
    {
        DialogueManager.Instance.StartDialogue(dialogue);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            TriggerDialogue();
        }
    }
}
