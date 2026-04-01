using UnityEngine;
using System.Collections.Generic;

public class ObjectiveManager : MonoBehaviour
{
    public enum LevelPhase
    {
        InitialDialogue,
        CombatPhase,
        PostCombatDialogue,
        TransitionToNextArea
    }

    [Header("Level Settings")]
    [SerializeField] private LevelPhase currentPhase = LevelPhase.InitialDialogue;
    [SerializeField] private Dialogue initialDialogue;
    [SerializeField] private Dialogue midLevelDialogue;
    [SerializeField] private Dialogue endLevelDialogue;

    [Header("Enemy Management")]
    [SerializeField] private List<GameObject> enemies = new List<GameObject>();
    [SerializeField] private int requiredKills = 3;
    private int currentKills = 0;

    [Header("Area Transition")]
    [SerializeField] private Collider areaBoundary;
    [SerializeField] private Transform cameraTargetNext;

    public event System.Action<string> OnObjectiveCompleted;

    public static ObjectiveManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (areaBoundary != null)
            areaBoundary.isTrigger = false;

        SubscribeToEvents();
    }

    void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    void SubscribeToEvents()
    {
        GameEvents.OnDialogueCompleted += HandleDialogueCompleted;
        GameEvents.OnCameraTransitionComplete += HandleCameraTransitionComplete;
    }

    void UnsubscribeFromEvents()
    {
        GameEvents.OnDialogueCompleted -= HandleDialogueCompleted;
        GameEvents.OnCameraTransitionComplete -= HandleCameraTransitionComplete;
    }

    void Start()
    {
        StartLevel();
    }

    void StartLevel()
    {
        currentPhase = LevelPhase.InitialDialogue;

        if (initialDialogue != null)
        {
            if (DialogueManager.Instance != null)
            {
                DialogueManager.Instance.StartDialogue(initialDialogue);
            }
        }
        else
        {
            StartCombatPhase();
        }
    }

    public void OnEnemyDestroyed(GameObject enemy)
    {
        if (currentPhase != LevelPhase.CombatPhase) return;

        if (enemies.Contains(enemy))
            enemies.Remove(enemy);

        currentKills++;

        if (currentKills == 1 && midLevelDialogue != null)
        {
            if (DialogueManager.Instance != null)
                DialogueManager.Instance.StartDialogue(midLevelDialogue);
        }

        if (enemies.Count <= 0 || currentKills >= requiredKills)
            EndCombatPhase();
    }

    private void EndCombatPhase()
    {
        currentPhase = LevelPhase.PostCombatDialogue;

        OnObjectiveCompleted?.Invoke("DestroyAllEnemies");
        GameEvents.InvokeObjectiveCompleted("DestroyAllEnemies");

        if (endLevelDialogue != null)
        {
            if (DialogueManager.Instance != null)
                DialogueManager.Instance.StartDialogue(endLevelDialogue);
        }
        else
        {
            HandleDialogueCompleted();
        }
    }

    private void HandleDialogueCompleted()
    {
        switch (currentPhase)
        {
            case LevelPhase.InitialDialogue:
                StartCombatPhase();
                break;

            case LevelPhase.PostCombatDialogue:
                StartCameraTransition();
                break;
        }
    }

    private void StartCombatPhase()
    {
        currentPhase = LevelPhase.CombatPhase;

        foreach (var enemy in enemies)
        {
            if (enemy != null)
                enemy.SetActive(true);
        }
    }

    private void StartCameraTransition()
    {
        currentPhase = LevelPhase.TransitionToNextArea;

        if (cameraTargetNext != null)
        {
            GameEvents.InvokeCameraTransitionRequested(cameraTargetNext);
        }
        else
        {
            HandleCameraTransitionComplete();
        }
    }

    private void HandleCameraTransitionComplete()
    {
        if (areaBoundary != null)
            areaBoundary.isTrigger = true;

        currentPhase = LevelPhase.InitialDialogue;
    }
}