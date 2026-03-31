using UnityEngine;

public class LevelCompletionKills : LevelCondition
{
    [SerializeField] private int m_RequiredKills;

    public override bool IsCompleted
    {
        get
        {
            if (Player.Instance?.ActiveShip == null) return false;

            return Player.Instance.NumKills >= m_RequiredKills;
        }
    }
}
