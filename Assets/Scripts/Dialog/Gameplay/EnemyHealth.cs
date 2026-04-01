using UnityEngine;
using UnityEngine.Events;
using System;

public class EnemyHealth : MonoBehaviour
{
    [System.Serializable]
    public class DeathEvent : UnityEvent<GameObject> { }
    [System.Serializable]
    public class HealthChangedEvent : UnityEvent<float> { }

    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private bool isBoss = false;
    [SerializeField] private GameObject deathEffect;

    [Header("Events")]
    public DeathEvent OnDeath;
    public HealthChangedEvent OnHealthChanged;

    // C# событие для подписки из кода
    public event Action<GameObject> OnEnemyDied;

    private int currentHealth;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        float healthPercent = (float)currentHealth / maxHealth;
        OnHealthChanged?.Invoke(healthPercent);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;

        // Вызываем UnityEvent
        OnDeath?.Invoke(gameObject);

        // Вызываем C# событие
        OnEnemyDied?.Invoke(gameObject);

        // Глобальное событие
        GameEvents.InvokeEnemyDestroyed(gameObject);

        // Уведомляем ObjectiveManager
        if (ObjectiveManager.Instance != null)
        {
            ObjectiveManager.Instance.OnEnemyDestroyed(gameObject);
        }

        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject, 0.1f);
    }

    public void SetHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
    }

    public float GetHealthPercent()
    {
        return (float)currentHealth / maxHealth;
    }

    public bool IsBoss()
    {
        return isBoss;
    }
}