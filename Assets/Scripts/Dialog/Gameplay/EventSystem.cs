using UnityEngine;
using System;

public static class GameEvents
{
    // События диалогов
    public static event Action<Dialogue> OnDialogueStarted;
    public static event Action OnDialogueCompleted;

    // События боя
    public static event Action<GameObject> OnEnemyDestroyed;
    public static event Action OnAllEnemiesDefeated;

    // События камеры
    public static event Action<Transform> OnCameraTransitionRequested;
    public static event Action OnCameraTransitionComplete;

    // События заданий
    public static event Action<string> OnObjectiveCompleted;

    // Методы для вызова событий
    public static void InvokeDialogueStarted(Dialogue dialogue)
    {
        OnDialogueStarted?.Invoke(dialogue);
    }

    public static void InvokeDialogueCompleted()
    {
        OnDialogueCompleted?.Invoke();
    }

    public static void InvokeEnemyDestroyed(GameObject enemy)
    {
        OnEnemyDestroyed?.Invoke(enemy);
    }

    public static void InvokeAllEnemiesDefeated()
    {
        OnAllEnemiesDefeated?.Invoke();
    }

    public static void InvokeCameraTransitionRequested(Transform target)
    {
        OnCameraTransitionRequested?.Invoke(target);
    }

    public static void InvokeCameraTransitionComplete()
    {
        OnCameraTransitionComplete?.Invoke();
    }

    public static void InvokeObjectiveCompleted(string objectiveID)
    {
        OnObjectiveCompleted?.Invoke(objectiveID);
    }
}