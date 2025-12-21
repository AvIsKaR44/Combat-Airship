using UnityEngine;
using UnityEngine.UI;

    public class UIGameResultsPanel : MonoBehaviour, IDependency<GameResultTime>
    {  
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private Text recordTime;      
        [SerializeField] private Text currentTime;

        private GameResultTime gameResultTime;
        public void Construct(GameResultTime obj) => gameResultTime = obj;


        private void Start()
        {
            resultPanel.SetActive(false);

            gameResultTime.ResultUpdated += OnResultUpdated;           
        }
        private void OnDestroy()
        {
            gameResultTime.ResultUpdated -= OnResultUpdated;
        }

        private void OnResultUpdated()
        {
            resultPanel.SetActive(true);

            recordTime.text = StringTime.SecondToTimeString(gameResultTime.GetAbsoluteRecord());
            currentTime.text = StringTime.SecondToTimeString(gameResultTime.CurrentTime);
        }    
    }
