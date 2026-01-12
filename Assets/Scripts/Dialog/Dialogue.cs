using UnityEngine;
using System;

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/Dialogue")]
public class Dialogue : ScriptableObject
{
    [Serializable]
    public class DialogueLine
    {
        public Speaker speaker;
        [TextArea(3, 5)] public string text;
        public AudioClip voiceLine;
    }

    public enum Speaker { Dispatcher, EnemyBoss, Player }

    public DialogueLine[] lines;
    public Dialogue nextDialogue;
}