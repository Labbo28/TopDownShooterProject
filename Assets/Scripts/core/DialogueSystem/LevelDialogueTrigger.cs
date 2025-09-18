using UnityEngine;
using System.Collections.Generic;

/*
============================================
  LEVEL DIALOGUE TRIGGER - USO E COLLEGAMENTO
============================================

Aggiungi questo script a un GameObject vuoto nella scena (es. "LevelDialogueTrigger").

IMPOSTA DA INSPECTOR: 
- dialogue: Il Dialogue da mostrare (puoi crearne uno nuovo nell'Inspector).
- triggerEveryNLevels: Ogni quanti livelli mostrare il dialogo (es. 5).

Il dialogo verrà mostrato automaticamente ogni volta che il player raggiunge un livello multiplo di N.
*/

// Classe per gestire i dialoghi per livello direttamente da Inspector
// Classe per gestire i dialoghi per livello direttamente da Inspector
[System.Serializable]
public class LevelDialogue
{
    public int level;
    public List<Dialogue> dialogues; // Usa Dialogue invece di string
}

public class LevelDialogueTrigger : MonoBehaviour
{
    [Header("Lista di dialoghi per livello")]
    [SerializeField]
    private List<LevelDialogue> levelDialogues = new List<LevelDialogue>();

    private int lastTriggeredLevel = 0;

    void Start()
    {
        PlayerUpgradeSystem.OnUpgradePanelClosed += OnUpgradePanelClosed;
    }

    void OnDestroy()
    {
        PlayerUpgradeSystem.OnUpgradePanelClosed -= OnUpgradePanelClosed;
    }

    private void OnUpgradePanelClosed(int newLevel)
    {
        if (newLevel != lastTriggeredLevel)
        {
            foreach (var ld in levelDialogues)
            {
                if (ld.level == newLevel && ld.dialogues != null && ld.dialogues.Count > 0)
                {
                    lastTriggeredLevel = newLevel;
                    // Qui puoi mostrare tutti i dialoghi della lista, oppure solo il primo
                    foreach (var dialogue in ld.dialogues)
                    {
                        DialogueManager.Instance.StartDialogue(dialogue);
                    }
                    break;
                }
            }
        }
    }
}
