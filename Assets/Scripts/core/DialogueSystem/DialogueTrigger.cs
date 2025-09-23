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
    NUOVO: Può anche attivare un gatto per seguire il player!

    COME COLLEGARE E USARE:
    ------------------------------------------------------------
    1. Aggiungi questo script a un GameObject con Collider2D (es. NPC, oggetto interattivo).
    2. Imposta il campo "dialogue" nell'Inspector con le linee e i personaggi desiderati.
    3. OPZIONALE: Trascina il gatto nel campo "petToActivate" se vuoi attivarlo.
    4. Assicurati che il Collider2D abbia "Is Trigger" attivo.
    5. Quando il player entra nel trigger: dialogo + gatto attivato!

    LOGICA DI FUNZIONAMENTO:
    ------------------------------------------------------------
    • Dialogo: come prima
    • PetToActivate: se assegnato, il gatto inizia a seguire il player
    • OnlyOnce: se true, il trigger funziona solo la prima volta
    */

    [Header("Dialogue")]
    public Dialogue dialogue;

    [Header("Pet Activation (Opzionale)")]
    [SerializeField] private PetFollower petToActivate; // Trascina qui il gatto

    [Header("Trigger Settings")]
    [SerializeField] private bool onlyOnce = true; // Trigger solo una volta?

    private bool hasBeenTriggered = false;

    public void TriggerDialogue()
    {
        // Se deve funzionare solo una volta e è già stato attivato, esci
        if (onlyOnce && hasBeenTriggered)
        {
            return;
        }

        // Avvia il dialogo
        DialogueManager.Instance.StartDialogue(dialogue);

        // Attiva il gatto se assegnato
        if (petToActivate != null)
        {
            petToActivate.StartFollowing();
            Debug.Log("Dialogo avviato e gatto attivato!");
        }
        else
        {
            Debug.Log("Dialogo avviato!");
        }

        // Segna come già attivato
        hasBeenTriggered = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            TriggerDialogue();
        }
    }

    // Metodo per resettare il trigger (utile per testing)
    public void ResetTrigger()
    {
        hasBeenTriggered = false;
        Debug.Log("Trigger resettato");
    }
    
    public void SetPetToActivate(PetFollower pet)
    {
        petToActivate = pet;
    }
}