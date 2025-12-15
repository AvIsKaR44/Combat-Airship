using UnityEngine;
using UnityEngine.UI;



public class UICountDownTimer : MonoBehaviour, IDependency<GameStateTracker>
{
    [SerializeField] private Text text;
    [SerializeField] private Timer  countDownTimer;
        
    private GameStateTracker gameStateTracker;

    public void Construct(GameStateTracker obj)
    {
        gameStateTracker = obj;

        gameStateTracker.PreparationStarted += OnPreparationStarted;
        gameStateTracker.Started += OnGameStarted;
    }
    

    private void Start()
    {

        gameStateTracker.PreparationStarted += OnPreparationStarted;
        gameStateTracker.Started += OnGameStarted;
      
    }

    private void OnDestroy()
    {
       
        gameStateTracker.PreparationStarted -= OnPreparationStarted;
        gameStateTracker.Started -= OnGameStarted;
        
    }
    private void OnPreparationStarted()
    {
        text.enabled = true;
        enabled = true;
    }

    private void OnGameStarted()
    {
        text.enabled = false;
        enabled = false;
    }

    private void Update()
    {
        text.text = gameStateTracker.CountDownTimer.Value.ToString("F0");

        if (text.text == "0")
            text.text = "Fly!";
    }
}
