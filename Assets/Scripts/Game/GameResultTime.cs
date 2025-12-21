using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;



public class GameResultTime : MonoBehaviour, IDependency<GameTimeTracker>, IDependency<GameStateTracker>
{
    public const string SaveMark = "_player_best_time";

    public event UnityAction ResultUpdated;

    private float playerRecordTime;
    private float currentTime;

    public float PlayerRecordTime => playerRecordTime;
    public float CurrentTime => currentTime;
    public bool RecordWasSet => playerRecordTime != 0;

    private GameTimeTracker gameTimeTracker;
    public void Construct(GameTimeTracker obj) => gameTimeTracker = obj;
    private GameStateTracker gameStateTracker;
    public void Construct(GameStateTracker obj) => gameStateTracker = obj;

    private void Awake()
    {
        Load();
    }
    private void Start()
    {
        gameStateTracker.Completed += OnGameCompleted;
    }

    private void OnDestroy()
    {
        gameStateTracker.Completed -= OnGameCompleted;

    }
    private void OnGameCompleted()
    {            
        currentTime = gameTimeTracker.CurrentTime;

        if (currentTime < playerRecordTime || playerRecordTime == 0)
        {
            playerRecordTime = currentTime;
            Save();
        }            

        ResultUpdated?.Invoke();           
    }
    public float GetAbsoluteRecord()
    {
        return playerRecordTime;
    }
    private void Load()
    {
        playerRecordTime = PlayerPrefs.GetFloat(SceneManager.GetActiveScene().name + SaveMark, 0);
    }
    private void Save()
    {
        PlayerPrefs.SetFloat(SceneManager.GetActiveScene().name + SaveMark, playerRecordTime);
    }
}
