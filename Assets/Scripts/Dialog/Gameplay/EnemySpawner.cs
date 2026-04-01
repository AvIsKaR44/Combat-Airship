using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public GameObject enemyPrefab;
        public int count;
        public float spawnInterval;
        public Transform[] spawnPoints;
    }

    [SerializeField] private Wave[] waves;
    [SerializeField] private List<GameObject> activeEnemies = new List<GameObject>();

    private int currentWave = 0;
    private bool isSpawning = false;

    void Start()
    {
        SpawnWave(currentWave);
    }

    public void SpawnWave(int waveIndex)
    {
        if (waveIndex >= waves.Length || isSpawning) return;

        StartCoroutine(SpawnWaveCoroutine(waves[waveIndex]));
    }

    IEnumerator SpawnWaveCoroutine(Wave wave)
    {
        isSpawning = true;

        for (int i = 0; i < wave.count; i++)
        {
            if (wave.spawnPoints.Length == 0) break;

            Transform spawnPoint = wave.spawnPoints[Random.Range(0, wave.spawnPoints.Length)];
            GameObject enemy = Instantiate(wave.enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            activeEnemies.Add(enemy);

            // Настраиваем врага для оповещения при смерти
            EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                // Способ 1: Подписываемся на C# событие (рекомендуется для кода)
                enemyHealth.OnEnemyDied += (deadEnemy) =>
                {
                    HandleEnemyDeath(deadEnemy);
                };

                // Способ 2: Через UnityEvent (для инспектора)
                enemyHealth.OnDeath.AddListener(HandleEnemyDeath);
            }

            yield return new WaitForSeconds(wave.spawnInterval);
        }

        isSpawning = false;
    }

    private void HandleEnemyDeath(GameObject deadEnemy)
    {
        if (activeEnemies.Contains(deadEnemy))
        {
            activeEnemies.Remove(deadEnemy);
        }

        ObjectiveManager.Instance?.OnEnemyDestroyed(deadEnemy);

        // Проверяем, все ли враги уничтожены
        if (activeEnemies.Count == 0 && !isSpawning)
        {
            OnWaveCompleted();
        }
    }

    private void OnWaveCompleted()
    {
        Debug.Log($"Wave {currentWave + 1} completed!");

        // Можно добавить дополнительные действия при завершении волны
        // Например: выдать награду, запустить диалог и т.д.
    }

    public void SpawnNextWave()
    {
        currentWave++;
        if (currentWave < waves.Length)
        {
            SpawnWave(currentWave);
        }
        else
        {
            Debug.Log("All waves completed!");
        }
    }

    public int GetActiveEnemyCount()
    {
        return activeEnemies.Count;
    }

    public bool AreAllWavesCompleted()
    {
        return currentWave >= waves.Length && activeEnemies.Count == 0;
    }

    public void ClearAllEnemies()
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null)
            {
                // Отписываемся от событий перед уничтожением
                EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.OnEnemyDied -= HandleEnemyDeath;
                    enemyHealth.OnDeath.RemoveListener(HandleEnemyDeath);
                }

                Destroy(enemy);
            }
        }
        activeEnemies.Clear();
    }

    void OnDestroy()
    {
        ClearAllEnemies();
    }

    // Метод для ручного добавления врага (например, для босса)
    public GameObject SpawnEnemy(GameObject enemyPrefab, Transform spawnPoint)
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        activeEnemies.Add(enemy);

        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.OnEnemyDied += (deadEnemy) =>
            {
                HandleEnemyDeath(deadEnemy);
            };
        }

        return enemy;
    }
}