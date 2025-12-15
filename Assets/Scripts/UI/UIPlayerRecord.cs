using UnityEngine;
using UnityEngine.UI;


public class UIPlayerRecord : MonoBehaviour, IDependency<GameResultTime>
{
    [SerializeField] private GameObject playerRecordObject;
    [SerializeField] private Text playrRecordTime;

    private GameResultTime raceResultTime;
    public void Construct(GameResultTime obj) => raceResultTime = obj;

    private void Start()
    {
        if (raceResultTime.RecordWasSet)
            playrRecordTime.text = StringTime.SecondToTimeString(raceResultTime.PlayerRecordTime);
        else
            playrRecordTime.text = "NO RECORD";
    }
        
}
