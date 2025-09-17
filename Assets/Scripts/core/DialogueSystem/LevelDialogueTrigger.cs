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

public class LevelDialogueTrigger : MonoBehaviour
{
    [Header("Imposta il Dialogue da mostrare")]
    public Dialogue dialogue;

    [Header("Mostra dialogo ogni N livelli (0 = disattivato)")]
    public int triggerEveryNLevels = 0;

    [Header("Mostra dialogo su questi livelli specifici")]
    public List<int> triggerOnLevels = new List<int>();

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
        // Dialogo ogni N livelli
        if (triggerEveryNLevels > 0 && newLevel % triggerEveryNLevels == 0 && newLevel != lastTriggeredLevel)
        {
            lastTriggeredLevel = newLevel;
            DialogueManager.Instance.StartDialogue(dialogue);
            return;
        }
        // Dialogo su livelli specifici
        if (triggerOnLevels.Contains(newLevel) && newLevel != lastTriggeredLevel)
        {
            lastTriggeredLevel = newLevel;
            DialogueManager.Instance.StartDialogue(dialogue);
        }
    }
}
