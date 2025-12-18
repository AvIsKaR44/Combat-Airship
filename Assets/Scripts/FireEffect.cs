using UnityEngine;

public class FireEffect : MonoBehaviour
{
    [SerializeField] private int m_DamagePerSecond = 10;
    [SerializeField] private float m_Duration = 3f;

    private Destructible m_Target;
    private float m_StartTime;
    private float m_LastDamageTime;

    private void Start()
    {
        m_Target = GetComponent<Destructible>();
        m_StartTime = Time.time;
        m_LastDamageTime = Time.time;
    }

    private void Update()
    {
        // Применяем урон каждую секунду
        if (Time.time - m_LastDamageTime >= 1f)
        {
            m_Target.ApplyDamage(m_DamagePerSecond);
            m_LastDamageTime = Time.time;
        }

        // Удаляем эффект по истечении времени
        if (Time.time - m_StartTime >= m_Duration)
        {
            Destroy(this);
        }
    }

    public void Initialize(int damagePerSecond, float duration)
    {
        m_DamagePerSecond = damagePerSecond;
        m_Duration = duration;
    }

    public void RefreshEffect()
    {
        m_StartTime = Time.time; // Сбрасываем таймер
        m_LastDamageTime = Time.time;
    }
}
