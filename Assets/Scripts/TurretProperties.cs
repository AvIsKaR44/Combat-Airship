using UnityEngine;

public enum TurretMode
{
    Primary,
    Secondary
}

[CreateAssetMenu]
public class TurretProperties : ScriptableObject
{
    [SerializeField] private TurretMode m_Mode;
    public TurretMode Mode => m_Mode;

    [SerializeField] private Projectile m_ProjectilePrefab;
    public Projectile ProjectilePrefab => m_ProjectilePrefab;

    [SerializeField] private float m_RateOfFire;
    public float RateOfFire => m_RateOfFire;

    [SerializeField] private int m_UsagePrimaryAmmo;
    public int UsagePrimaryAmmo => m_UsagePrimaryAmmo;

    [SerializeField] private int m_UsageSecondaryAmmo;
    public int UsageSecondaryAmmo => m_UsageSecondaryAmmo;

    [SerializeField] private AudioClip m_LaunchSFX;
    public AudioClip LaunchSFX => m_LaunchSFX;

    [SerializeField] private float m_ProjectileSpeed = 15f;
    public float ProjectileSpeed => m_ProjectileSpeed;
}

