using UnityEngine;

public class LevelCompletionPosition : LevelCondition
{
    [SerializeField] private Vector2 m_BoundsSize = new Vector2(5f, 5f);

    public override bool IsCompleted
    {
        get
        {
            if (Player.Instance?.ActiveShip == null) return false;

            Vector2 shipPos = Player.Instance.ActiveShip.transform.position;
            Vector2 exitPos = transform.position;

            bool isInBounds = Mathf.Abs(shipPos.x - exitPos.x) <= m_BoundsSize.x/2 &&
                              Mathf.Abs(shipPos.y - exitPos.y) <= m_BoundsSize.y / 2;

            return isInBounds;

        }
    }
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.3f);
        Gizmos.DrawCube(transform.position, new Vector3(m_BoundsSize.x, m_BoundsSize.y, 0));

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, new Vector3(m_BoundsSize.x, m_BoundsSize.y, 0));
    }
#endif
}