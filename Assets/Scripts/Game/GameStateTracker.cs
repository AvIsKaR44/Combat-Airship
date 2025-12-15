using UnityEngine;
using UnityEngine.Events;

public enum GameState
{
    Preparation,
    CountDown,
    Game,
    Passed
}
public class GameStateTracker : MonoBehaviour
{
    public event UnityAction PreparationStarted;
    public event UnityAction Started;
    public event UnityAction Completed;
    public event UnityAction<int> AppleCollected;


    [SerializeField] private Timer countDownTimer;
    [SerializeField] private int applesToComplete;

    private int applesCollected;

    public Timer CountDownTimer => countDownTimer;

    private GameState state;
    public GameState State => state;
        
    private void StartState(GameState state)
    {
        this.state = state;
    }   
        
    private void Start()
    {
        StartState(GameState.Preparation);

        countDownTimer.enabled = false;
        countDownTimer.Finished += OnCountDownTimerFinished;

    }


    private void OnDestroy()
    {
        countDownTimer.Finished -= OnCountDownTimerFinished;

    }


    private void OnCountDownTimerFinished()
    {
        StartGame();
    }


    public void LaunchPreparationStart()
    {
        if (state != GameState.Preparation) return;
        StartState(GameState.CountDown);

        countDownTimer.enabled = true;
        PreparationStarted?.Invoke();
    }

    public void CollectApple()
    {
        if(state != GameState.Game) return;

        applesCollected++;

        if (applesCollected >= applesToComplete)
        {
            CompleteGame();
        }
    }    

    private void StartGame()
    {
        if (state != GameState.CountDown) return;
        StartState(GameState.Game);


        Started?.Invoke();
    }

    private void CompleteGame()
    {
        if (state != GameState.Game) return;
        StartState(GameState.Passed);


        Completed?.Invoke();
    }

    private void CompleteApple(int appleAmount)
    {
        AppleCollected?.Invoke(appleAmount);
    }
}
