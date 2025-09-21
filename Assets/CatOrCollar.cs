using System.Runtime.CompilerServices;
using UnityEngine;

public class CatOrCollar : MonoBehaviour
{
    [SerializeField] private GameObject catPrefab;
    [SerializeField] private Dialogue dialogueCat;
    [SerializeField] private GameObject collarPrefab;
    [SerializeField] private Dialogue dialogueCollar;

    private bool isCatSaved;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isCatSaved = GameManager.Instance.isCatSaved();

        if (isCatSaved)
        {
            Instantiate(catPrefab, transform.position, Quaternion.identity);
            
        }
        else
        {
            Instantiate(collarPrefab, transform.position, Quaternion.identity);
            
        }
            
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (isCatSaved)
                DialogueManager.Instance.StartDialogue(dialogueCat);
            else
                DialogueManager.Instance.StartDialogue(dialogueCollar);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
