using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class SimpleBomb : Projectile
{
    public enum MissileBehavior { Homing, Static }

    [Header("Bomb settings")]
    [SerializeField] private float activationDelay = 0.3f;
    [SerializeField] private float armingTime = 0.5f;
    [SerializeField] private float maxLifetime = 10f;

    [Header("Phisycs")]
    [SerializeField] private float gravityScale = 2f;
    [SerializeField] private float initialDownwardForce = 3f;
    [SerializeField] private float maxFallSpeed = 12f;

    [Header("Explosion")]
    [SerializeField] private GameObject explosionPrefab;    
    [SerializeField] private float explosionRadius = 4f;
    [SerializeField] private bool useFalloff = true;
    [SerializeField] private float minDamageMultiplier = 0.3f;
    [SerializeField] private bool explodeOnGroundHit = true;
    [SerializeField] private LayerMask groundLayer;

    [Header("Visuals")]
    [SerializeField] private GameObject smokeTrail;
    [SerializeField] private float trailActivationDelay = 0.2f;

    [Header("Sound")]
    [SerializeField] private AudioClip fallWhistle;
    [SerializeField] private AudioClip explosionSound;
    
    private Rigidbody2D rb;
    private Collider2D bombCollider;
    private bool isActive = false;
    private bool isArmed = false;
    private float lifeTimer = 0f;
    private AudioSource audioSource;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        bombCollider = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();

        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody2D>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        rb.gravityScale = gravityScale;
        rb.freezeRotation = true;

        rb.linearVelocity = Vector2.down * initialDownwardForce;

        StartCoroutine(ActivationRoutine());
    }

    private IEnumerator ActivationRoutine()
    {
        if (m_Parent != null && bombCollider != null)
        {
            IgnoreParentCollisions(true);
        }

        yield return new WaitForSeconds(activationDelay);

        if (m_Parent != null && bombCollider != null)
        {
            IgnoreParentCollisions(false);
        }

        isActive = true;
            
        yield return new WaitForSeconds(armingTime);
        isArmed = true;

        if (smokeTrail != null)
        {
            yield return new WaitForSeconds(trailActivationDelay);
            smokeTrail.SetActive(true);
        }

        if (fallWhistle != null && audioSource != null)
        {
            audioSource.PlayOneShot(fallWhistle);
        }
    }

    private void IgnoreParentCollisions(bool ignore)
    {
        var parentColliders = m_Parent.GetComponentsInChildren<Collider2D>();
        foreach (var col in parentColliders)
        {
            if (col != null && bombCollider != null)
            {
                Physics2D.IgnoreCollision(bombCollider, col, ignore);
            }
        }
    }
    
    protected override void Update()
    {
        if (!isActive) return;

        lifeTimer += Time.deltaTime;
                
        if (lifeTimer > maxLifetime)
        {
            Explode();
            return;
        }

        if (rb.linearVelocity.y < -maxFallSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -maxFallSpeed);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isActive || !isArmed) return;

        if (m_Parent != null && collision.transform.IsChildOf(m_Parent.transform))
            return;

        bool isGround = ((1 << collision.gameObject.layer) & groundLayer) != 0;

        if (isGround && explodeOnGroundHit)
        {
            Explode();
        }
        else if (!isGround)
        {
            Explode();
        }
        else if (isGround)
        {
            rb.linearVelocity = Vector2.zero;
            rb.gravityScale = 0f;
            rb.bodyType = RigidbodyType2D.Static;
        }
    }
    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isActive || !isArmed) return;

        if (collider.isTrigger) return;
        if (m_Parent != null && collider.transform.IsChildOf(m_Parent.transform)) return;

        Explode();
    }

    private void Explode()
    {
        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }

        ApplyExplosionDamage();

        Destroy(gameObject);
    }

    private void ApplyExplosionDamage()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D hit in hits)
        {
            if (m_Parent != null && hit.transform.IsChildOf(m_Parent.transform)) continue;

            Destructible dest = hit.transform.root.GetComponent<Destructible>();
            if (dest == null) continue;

            float distance = Vector2.Distance(transform.position, hit.transform.position);
            float damageMultiplier = 1f;

            if (useFalloff && explosionRadius > 0)
            {
                damageMultiplier = Mathf.Lerp(1f, minDamageMultiplier, Mathf.Clamp01(distance / explosionRadius));
            }

            int finalDamage = Mathf.RoundToInt(m_Damage * damageMultiplier);
            dest.ApplyDamage(finalDamage);
        }
    }

    // Вспомогательный метод для сброса бомбы с дирижабля
    public void DropFromAirship(float additionalForce = 0f)
    {
        // Можно добавить дополнительную силу вниз для большей скорости
        if (additionalForce > 0)
        {
            rb.linearVelocity = Vector2.down * (initialDownwardForce + additionalForce);
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);

        if (Application.isPlaying && rb != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + (Vector3)rb.linearVelocity * 0.5f);
        }
    }
}
