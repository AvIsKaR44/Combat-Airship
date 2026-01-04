using UnityEngine;

[CreateAssetMenu(fileName = "NewDialog", menuName = "Dialogs/Dialog Data")]
public class DialogDataSO : ScriptableObject
{
    [System.Serializable]
    public class DialogLine
    {
        public string speakerName;
        public string lineText;
        public AudioClip voiceClip; // опционально
        public float displayDuration = 3f; // автоскрытие через N секунд
        public bool waitForPlayerInput = false; // ждать нажатия клавиши
    }

    public DialogLine[] dialogLines;
    public DialogTriggerType triggerType = DialogTriggerType.Manual;

    public enum DialogTriggerType
    {
        Manual,
        OnLevelStart,
        OnEvent
    }
}
