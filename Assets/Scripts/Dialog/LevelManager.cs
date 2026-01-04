using UnityEngine;
using static DialogDataSO;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private DialogDatabaseSO dialogDatabase;
    [SerializeField] private int currentLevel = 1;

    private void Start()
    {
        InitializeLevelDialogs();
        StartLevel();
    }

    private void InitializeLevelDialogs()
    {
        if (dialogDatabase == null) return;

        // Найти диалоги для текущего уровня
        foreach (var levelSet in dialogDatabase.levelDialogs)
        {
            if (levelSet.levelNumber == currentLevel)
            {
                // Создать триггеры для диалогов уровня
                CreateDialogTriggers(levelSet);
                break;
            }
        }
    }

    private void CreateDialogTriggers(DialogDatabaseSO.LevelDialogSet levelSet)
    {
        // Диалог начала уровня
        if (levelSet.levelStartDialog != null)
        {
            GameObject startDialogObj = new GameObject("StartDialogTrigger");
            var trigger = startDialogObj.AddComponent<DialogTrigger>();
            trigger.dialogToPlay = levelSet.levelStartDialog;
            trigger.triggerType = DialogTriggerType.OnLevelStart;
        }

        // Диалоги по событиям
        foreach (var eventDialog in levelSet.eventDialogs)
        {
            // Здесь нужно привязать к конкретным событиям
            // Например, через GameEventManager
        }

        // Диалог завершения уровня
        if (levelSet.levelCompleteDialog != null)
        {
            // Можно активировать через GameEventManager.OnLevelCompleted
        }
    }

    private void StartLevel()
    {
        // Запустить событие начала уровня
        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.OnLevelStarted?.Invoke();
        }
    }

    // Вызывается при уничтожении аэростата
    public void OnBalloonDestroyed()
    {
        if (currentLevel == 1)
        {
            GameEventManager.Instance.OnLevelEvent?.Invoke("FirstBalloonDestroyed");
        }
    }

    // Вызывается при уничтожении башни
    public void OnTowerDestroyed(int towerNumber)
    {
        if (currentLevel == 1)
        {
            GameEventManager.Instance.OnLevelEvent?.Invoke($"Tower{towerNumber}Destroyed");
        }
    }

    // Вызывается при завершении уровня
    public void CompleteLevel()
    {
        if (GameEventManager.Instance != null)
        {
            GameEventManager.Instance.OnLevelCompleted?.Invoke();
        }
    }
}
