using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Уничтожаемый обьект на сцене. То что может иметь хит поинты.
/// </summary>
public class Destructible : Entity
{
    #region Properties
    /// <summary>
    /// Объект игнорирует повреждения.
    /// </summary>
    [SerializeField] private bool m_Indestructible;
    public bool IsIndestructible => m_Indestructible;

    /// <summary>
    /// Объект игнорирует повреждения.
    /// </summary>
    [SerializeField] private int m_HitPoints;
    public int MaxHitPoints => m_HitPoints;

    /// <summary>
    /// Объект игнорирует повреждения.
    /// </summary>
    private int m_CurrentHitPoints;
    public int HitPoints => m_CurrentHitPoints;

    #endregion

    #region Unity Events

    protected virtual void Start()
    {
        m_CurrentHitPoints = m_HitPoints;
    }

    #endregion

    #region Public API

    /// <summary>
    /// Применение дамага к объекту.
    /// </summary>
    /// <param name="damage">Урон наносимый объекту</param>
    public virtual void ApplyDamage(int damage)
    {
            
        if (m_Indestructible) return;

        m_CurrentHitPoints -= damage;

        if (m_CurrentHitPoints <= 0)
            OnDeath();
    }

    #endregion
    /// <summary>
    /// Переопределяемое событие уничтожения объекта, когда хит поинты ниже нуля.
    /// </summary>
    protected virtual void OnDeath()
    {
        Destroy(gameObject);

        m_EventOnDeath?.Invoke();
    }

    private static HashSet<Destructible> m_AllDestructibles;

    public static IReadOnlyCollection<Destructible> AllDestructibles => m_AllDestructibles;

    protected virtual void OnEnable()
    {
        if (m_AllDestructibles == null)
            m_AllDestructibles = new HashSet<Destructible>();

        m_AllDestructibles.Add(this);
    }

    public const int TeamIdNeutral = 0;

    [SerializeField] private int m_TeamId;
    public int TeamId => m_TeamId;

    [SerializeField] private UnityEvent m_EventOnDeath;
    public UnityEvent EventOnDeath => m_EventOnDeath;

    public void AddHitPoints(int amount)
    {
        if (m_Indestructible) return; // Не лечим неразрушаемые объекты

        // Увеличиваем текущие HP, но не больше максимального значения
        m_CurrentHitPoints = Mathf.Clamp(m_CurrentHitPoints + amount, 0, m_HitPoints);
    }

    protected virtual void OnDestroy()
    {
        m_AllDestructibles.Remove(this);

        // Удаляем все эффекты при уничтожении объекта
        FireEffect fireEffect = GetComponent<FireEffect>();
        if (fireEffect != null) Destroy(fireEffect);
    }


    [SerializeField] private int m_ScoreValue;
    public int ScoreValue => m_ScoreValue;
}

