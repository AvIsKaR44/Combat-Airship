using UnityEngine;

public class LevelCompletionScore : LevelCondition
{
    [SerializeField] private int m_RequiredScore;

    public override bool IsCompleted
    {
        get
        {
            if (Player.Instance?.ActiveShip == null) return false;

            return Player.Instance.Score >= m_RequiredScore;
        }
    }
}
