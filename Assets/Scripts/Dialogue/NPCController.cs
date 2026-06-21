using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField] DialogueData dialogueData;
    [SerializeField] DialogueBubble dialogueBubble;
    [SerializeField] float autoCloseDistance = 5f;
    [SerializeField] float interactDistance = 3f;

    bool isDialogueActive = false;
    Transform player;
    BoggleDirector savedBoggle = null;

    private void Start()
    {
        player = FindFirstObjectByType<PlayerController>().transform;
    }

    private void Update()
    {
        if (!isDialogueActive) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > autoCloseDistance)
            EndDialogue();
    }

    private void OnMouseDown()
    {
        if (isDialogueActive) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > interactDistance) return;

        StartDialogue();
    }

    private void StartDialogue()
    {
        isDialogueActive = true;
        dialogueBubble.OnDialogueEnd += OnDialogueEnded;
        dialogueBubble.StartDialogue(dialogueData, savedBoggle, (boggle) => savedBoggle = boggle);
    }

    private void OnDialogueEnded()
    {
        isDialogueActive = false;
        dialogueBubble.OnDialogueEnd -= OnDialogueEnded;
    }

    private void EndDialogue()
    {
        if (!isDialogueActive) return;
        isDialogueActive = false;
        dialogueBubble.OnDialogueEnd -= OnDialogueEnded;
        dialogueBubble.EndDialogue();
    }
}