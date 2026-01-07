using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class DialogueTrigger : MonoBehaviour
{
    public enum TriggerType
    {
        OnEnter,
        OnExit,
        OnDestroyEnemy,
        OnObjectiveComplete,
        Manual
    }

    [Header("Trigger Settings")]
    [SerializeField] private TriggerType triggerType = TriggerType.OnEnter;
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private bool oneTimeUse = true;
    [SerializeField] private bool showDebug = false;

    [Header("Dialogue Configuration")]
    [SerializeField] private Dialogue dialogueToPlay;
    [SerializeField] private string dialogueEventID;
    [SerializeField] private float startDelay = 0f;

    [Header("Conditions")]
    [SerializeField] private int requiredEnemyKills = 0;
    [SerializeField] private GameObject specificEnemyToDestroy;
    [SerializeField] private bool waitForPreviousDialogue = false;

    [Header("Events")]
    [SerializeField] private UnityEvent onDialogueStart;
    [SerializeField] private UnityEvent onDialogueEnd;

    private bool hasBeenTriggered = false;
    private int enemiesDestroyedCount = 0;
    private bool isSubscribed = false;

    void Start()
    {
        SubscribeToEvents();
    }

    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    void SubscribeToEvents()
    {
        if (isSubscribed) return;

        if (triggerType == TriggerType.OnDestroyEnemy)
            GameEvents.OnEnemyDestroyed += HandleEnemyDestroyed;

        if (triggerType == TriggerType.OnObjectiveComplete)
            GameEvents.OnObjectiveCompleted += HandleObjectiveComplete;

        isSubscribed = true;
    }

    void UnsubscribeFromEvents()
    {
        if (!isSubscribed) return;

        GameEvents.OnEnemyDestroyed -= HandleEnemyDestroyed;
        GameEvents.OnObjectiveCompleted -= HandleObjectiveComplete;

        isSubscribed = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggerType != TriggerType.OnEnter) return;
        if (!other.CompareTag(targetTag)) return;
        if (hasBeenTriggered && oneTimeUse) return;

        TriggerDialogue();
    }

    void OnTriggerExit(Collider other)
    {
        if (triggerType != TriggerType.OnExit) return;
        if (!other.CompareTag(targetTag)) return;
        if (hasBeenTriggered && oneTimeUse) return;

        TriggerDialogue();
    }

    void HandleEnemyDestroyed(GameObject enemy)
    {
        if (triggerType != TriggerType.OnDestroyEnemy) return;
        if (hasBeenTriggered && oneTimeUse) return;

        if (specificEnemyToDestroy != null && enemy != specificEnemyToDestroy)
            return;

        enemiesDestroyedCount++;

        if (enemiesDestroyedCount >= requiredEnemyKills)
            TriggerDialogue();
    }

    void HandleObjectiveComplete(string objectiveID)
    {
        if (triggerType != TriggerType.OnObjectiveComplete) return;
        if (hasBeenTriggered && oneTimeUse) return;

        TriggerDialogue();
    }

    public void ManualTrigger()
    {
        if (triggerType != TriggerType.Manual)
        {
            Debug.LogWarning("This trigger is not set to Manual type!");
            return;
        }

        TriggerDialogue();
    }

    private void TriggerDialogue()
    {
        if (hasBeenTriggered && oneTimeUse) return;

        if (waitForPreviousDialogue && DialogueUI.Instance != null && DialogueUI.Instance.IsDialogueActive())
        {
            StartCoroutine(WaitForPreviousDialogue());
            return;
        }

        hasBeenTriggered = true;
        onDialogueStart?.Invoke();

        if (startDelay > 0)
            StartCoroutine(StartDialogueWithDelay());
        else
            StartDialogue();
    }

    private IEnumerator StartDialogueWithDelay()
    {
        if (showDebug) Debug.Log($"[DialogueTrigger] Waiting {startDelay} seconds before dialogue...", this);
        yield return new WaitForSeconds(startDelay);
        StartDialogue();
    }

    private IEnumerator WaitForPreviousDialogue()
    {
        if (showDebug) Debug.Log($"[DialogueTrigger] Waiting for previous dialogue to finish...", this);

        while (DialogueUI.Instance.IsDialogueActive())
            yield return null;

        yield return new WaitForSeconds(0.5f);
        TriggerDialogue();
    }

    private void StartDialogue()
    {
        if (showDebug) Debug.Log($"[DialogueTrigger] Starting dialogue: {gameObject.name}", this);

        if (dialogueToPlay != null)
        {
            if (DialogueManager.Instance != null)
                DialogueManager.Instance.StartDialogue(dialogueToPlay);
            else if (DialogueUI.Instance != null)
                DialogueUI.Instance.StartDialogue(dialogueToPlay);
        }
        else if (!string.IsNullOrEmpty(dialogueEventID))
        {
            if (DialogueManager.Instance != null)
                DialogueManager.Instance.TriggerDialogue(dialogueEventID);
        }
        else
        {
            Debug.LogError($"[DialogueTrigger] No dialogue configured for trigger: {gameObject.name}", this);
            return;
        }

        if (oneTimeUse)
        {
            GetComponent<Collider>().enabled = false;
            UnsubscribeFromEvents();
        }
    }

    public void SetDialogue(Dialogue newDialogue)
    {
        dialogueToPlay = newDialogue;
    }

    public void ResetTrigger()
    {
        hasBeenTriggered = false;
        enemiesDestroyedCount = 0;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = true;

        SubscribeToEvents();
    }
}