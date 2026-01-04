using UnityEngine;
using static DialogDataSO;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] public DialogDataSO dialogToPlay;
    [SerializeField] public DialogTriggerType triggerType;
    [SerializeField] private string eventName; // для триггера по событию

    private bool hasBeenTriggered = false;

    private void Start()
    {
        if (triggerType == DialogTriggerType.OnLevelStart)
        {
            TriggerDialog();
        }

        // Подписка на события
        if (triggerType == DialogTriggerType.OnEvent && GameEventManager.Instance != null)
        {
            GameEventManager.Instance.OnLevelEvent.AddListener(OnGameEvent);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (triggerType == DialogTriggerType.Manual &&
            other.CompareTag("Player") &&
            !hasBeenTriggered)
        {
            TriggerDialog();
        }
    }

    private void OnGameEvent(string eventId)
    {
        if (triggerType == DialogTriggerType.OnEvent &&
            eventId == eventName &&
            !hasBeenTriggered)
        {
            TriggerDialog();
        }
    }

    private void TriggerDialog()
    {
        if (dialogToPlay != null && DialogManager.Instance != null)
        {
            DialogManager.Instance.StartDialog(dialogToPlay);
            hasBeenTriggered = true;
        }
    }

    private void OnDestroy()
    {
        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.OnLevelEvent.RemoveListener(OnGameEvent);
        }
    }
}
