using UnityEngine;
using UnityEngine.UI;



public class UIGameTime : MonoBehaviour, IDependency<GameTimeTracker>, IDependency<GameStateTracker>
{
    [SerializeField] private Text text;
        
    private GameTimeTracker timeTracker;
    public void Construct(GameTimeTracker obj) => timeTracker = obj;
    private GameStateTracker gameStateTracker;
    public void Construct(GameStateTracker obj) => gameStateTracker = obj;


       
    private void Start()
    {
        gameStateTracker.Started += OnGameStarted;
        gameStateTracker.Completed += OnGameCompleted;

        text.enabled = false;
    }

    private void OnDestroy()
    {
        gameStateTracker.Started -= OnGameStarted;
        gameStateTracker.Completed -= OnGameCompleted;
    }
    private void OnGameStarted()
    {
        text.enabled = true;
        enabled = true;
    }

    private void OnGameCompleted()
    {
        text.enabled = false;
        enabled = false;
    }

    private void Update()
    {
        text.text = StringTime.SecondToTimeString(timeTracker.CurrentTime);
    }
}
