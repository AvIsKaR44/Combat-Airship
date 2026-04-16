using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] protected float m_Velocity;

    [SerializeField] protected float m_Lifetime;

    [SerializeField] protected int m_Damage;

    [SerializeField] protected ImpactEffect m_ImpactEffectPrefab;

    protected float m_Timer;
        
    protected virtual void Start()
    {
        if (m_Parent != null)
        {
            Collider2D parentCollider = m_Parent.GetComponent<Collider2D>();
            Collider2D myCollider = GetComponent<Collider2D>();

            if (parentCollider != null && myCollider != null)
            {
                Physics2D.IgnoreCollision(myCollider, parentCollider, true);
                StartCoroutine(EnableCollisionAfterDelay(0.2f));
            }
        }
    }

    private IEnumerator EnableCollisionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (m_Parent != null)
        {
            Collider2D parentCollider = m_Parent.GetComponent<Collider2D>();
            Collider2D myCollider = GetComponent<Collider2D>();
            if (parentCollider != null && myCollider != null)
            {
                Physics2D.IgnoreCollision(myCollider, parentCollider, false);
            }
        }
    }

    protected virtual void Update()
    {
        float stepLength = Time.deltaTime * m_Velocity;
        Vector2 step = transform.up * stepLength;


        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, stepLength);

        if (hit)
        {
            Destructible dest = hit.collider.transform.root.GetComponent<Destructible>();

            if (dest != null && dest != m_Parent)
            {
                dest.ApplyDamage(m_Damage);

                if (m_Parent == Player.Instance.ActiveShip)
                {
                    Player.Instance.AddScore(dest.ScoreValue);

                    if (dest is AirShip)
                    {
                        if (dest.HitPoints <= 0)
                            Player.Instance.AddKill();
                    }

                }
            }

            OnProjectileLifeEnd(hit.collider, hit.point);
        }

        m_Timer += Time.deltaTime;

        if (m_Timer > m_Lifetime)
            Destroy(gameObject);

        transform.position += new Vector3(step.x, step.y, 0);
    }

    protected virtual void OnProjectileLifeEnd(Collider2D col, Vector2 pos)
    {
        Destroy(gameObject);
    }

    protected Destructible m_Parent;

    public void SetParentShooter(Destructible parent)
    {
        m_Parent = parent;
    }
}

