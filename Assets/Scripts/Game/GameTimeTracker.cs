using UnityEngine;


public class GameTimeTracker : MonoBehaviour, IDependency<GameStateTracker>
{
    private GameStateTracker gameStateTracker;
    public void Construct(GameStateTracker obj) => gameStateTracker = obj;

    private float currentTime;
    public float CurrentTime => currentTime;

    private void Start()
    {
        gameStateTracker.Started += OnGameStarted; 
        gameStateTracker.Completed += OnGameCompleted; 

        enabled = false;
    }

    private void OnDestroy()
    {
        gameStateTracker.Started -= OnGameStarted;
        gameStateTracker.Completed -= OnGameCompleted;
    }
    private void OnGameStarted()
    {
        enabled = true;
        currentTime = 0;
    }

    private void OnGameCompleted()
    {
        enabled = false;           
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
    }
}
