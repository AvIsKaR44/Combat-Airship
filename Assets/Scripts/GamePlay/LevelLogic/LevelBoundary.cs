using UnityEngine;

public class LevelBoundary : SingletonBase<LevelBoundary>
{
    [SerializeField] private Vector2 m_BoundsSize = new Vector2(20f,15f);
    [SerializeField] private Vector2 m_Center = Vector2.zero;

    public float Left => m_Center.x - m_BoundsSize.x/2;
    public float Right => m_Center.x + m_BoundsSize.x/2;
    public float Bottom => m_Center.y - m_BoundsSize.y/2;
    public float Top => m_Center.y + m_BoundsSize.y/2;

    public enum Mode{Limit,Teleport}

    [SerializeField] private Mode m_LimitMode;

    public Vector2 ClampPosition(Vector2 position)
    {
        position.x = Mathf.Clamp(position.x,Left,Right);
        position.y = Mathf.Clamp(position.y,Bottom,Top);        
        return position;
        
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Vector3 center = new Vector3(m_Center.x, m_Center.y, 0);
        Vector3 size = new Vector3(m_BoundsSize.x, m_BoundsSize.y, 0);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(center, size);
    }   
#endif
}