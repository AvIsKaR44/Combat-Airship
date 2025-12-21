using UnityEngine;
using UnityEngine.UI;

public class UIStartHint : MonoBehaviour, IDependency<GameStateTracker>
{
    [SerializeField] private GameObject hintPanel;
    [SerializeField] private Text hintText;

    private GameStateTracker raceStateTracker;
    public void Construct(GameStateTracker obj) => raceStateTracker = obj;

    private void Start()
    {
        hintPanel.SetActive(true);

        raceStateTracker.PreparationStarted += OnPreparationStarted;
    }
    private void OnDestroy()
    {
        raceStateTracker.PreparationStarted -= OnPreparationStarted;

    }

    private void OnPreparationStarted()
    {
            hintPanel.SetActive(false);
    }
}
