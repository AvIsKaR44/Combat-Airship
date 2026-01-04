using UnityEngine;

[CreateAssetMenu(fileName = "LevelDialogs", menuName = "Dialogs/Dialog Database")]
public class DialogDatabaseSO : ScriptableObject
{
    [System.Serializable]
    public class LevelDialogSet
    {
        public int levelNumber;
        public DialogDataSO levelStartDialog;
        public DialogDataSO[] eventDialogs; // диалоги по событиям
        public DialogDataSO levelCompleteDialog;
    }

    public LevelDialogSet[] levelDialogs;
}