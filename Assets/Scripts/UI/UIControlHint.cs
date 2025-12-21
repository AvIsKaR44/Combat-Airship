using UnityEngine;
using UnityEngine.UI;


public class UIControlHint : MonoBehaviour, IDependency<GameStateTracker>
{
    [SerializeField] private GameObject controlPanel;
    [SerializeField] private Text controlText;
    [SerializeField] private Image image;

    private GameStateTracker gameStateTracker;
    public void Construct(GameStateTracker obj) => gameStateTracker = obj;

    private void Start()
    {
        controlPanel.SetActive(true);
        gameStateTracker.PreparationStarted += OnPreparationStarted;

    }

    private void OnDestroy()
    {
        gameStateTracker.PreparationStarted -= OnPreparationStarted;
    }

    private void OnPreparationStarted()
    {
        controlPanel.SetActive(false);
    }
}
