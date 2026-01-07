using UnityEngine;
using System.Collections.Generic;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public class DialogueEvent
    {
        public string eventID;
        public Dialogue dialogue;
    }

    [Header("Dialogue Database")]
    [SerializeField] private List<DialogueEvent> dialogueEvents = new List<DialogueEvent>();

    private Dictionary<string, Dialogue> dialogueDictionary = new Dictionary<string, Dialogue>();

    public static DialogueManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        InitializeDialogueDictionary();
    }

    void InitializeDialogueDictionary()
    {
        foreach (var dialogueEvent in dialogueEvents)
        {
            if (!dialogueDictionary.ContainsKey(dialogueEvent.eventID))
            {
                dialogueDictionary.Add(dialogueEvent.eventID, dialogueEvent.dialogue);
            }
        }
    }

    public void TriggerDialogue(string eventID)
    {
        if (dialogueDictionary.TryGetValue(eventID, out Dialogue dialogue))
        {
            StartDialogue(dialogue);
        }
        else
        {
            Debug.LogWarning($"Dialogue event '{eventID}' not found!");
        }
    }

    public void StartDialogue(Dialogue dialogue)
    {
        if (dialogue == null)
        {
            Debug.LogError("Attempted to start null dialogue!");
            return;
        }

        GameEvents.InvokeDialogueStarted(dialogue);

        if (DialogueUI.Instance != null)
        {
            DialogueUI.Instance.StartDialogue(dialogue);
        }
        else
        {
            Debug.LogError("DialogueUI instance not found!");
        }
    }
}