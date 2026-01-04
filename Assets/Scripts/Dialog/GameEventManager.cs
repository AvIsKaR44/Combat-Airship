using UnityEngine;
using UnityEngine.Events;

public class GameEventManager : MonoBehaviour
{
    public static GameEventManager Instance { get; private set; }

    [System.Serializable] public class UnityStringEvent : UnityEvent<string> { }

    // События уровня
    public UnityEvent OnLevelStarted;
    public UnityEvent OnLevelCompleted;
    public UnityStringEvent OnLevelEvent; // "FirstBalloonDestroyed", "SecondTowerDestroyed", etc.

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
