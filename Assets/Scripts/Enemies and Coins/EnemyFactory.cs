using System.Collections.Generic;
using UnityEngine;
using TMPro;


public enum Difficulty
{
    Easy,
    Medium,
    Hard
}


public class EnemyFactory : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private EnemyAIController scoutPrefab;
    [SerializeField] private EnemyAIController guardianPrefab;
    [SerializeField] private Transform waypointsRoot;

    [Header("UI")]
    [SerializeField] private TMP_Dropdown difficultyDropdown;

  
    [Header("Spawn Points")]
    [SerializeField] private List<Transform> scoutSpawnPoints = new();
    [SerializeField] private List<Transform> guardianSpawnPoints = new();

    [Header("Coins")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private List<Transform> coinSpawnPoints = new();

    private readonly List<EnemyAIController> spawned = new();

    public void SpawnEnemies()
    {
        // LIMPIAR MONEDAS 
        foreach (var coin in GameObject.FindGameObjectsWithTag("Coin"))
        {
            Destroy(coin);
        }

        // LIMPIAR ENEMIGOS
        for (int i = spawned.Count - 1; i >= 0; i--)
        {
            if (spawned[i] != null)
                Destroy(spawned[i].gameObject);
        }
        spawned.Clear();

        Difficulty difficulty = (Difficulty)difficultyDropdown.value;

        switch (difficulty)
        {
            case Difficulty.Easy:
                SpawnMany(scoutPrefab, scoutSpawnPoints, 1);
                SpawnMany(guardianPrefab, guardianSpawnPoints, 1);
                break;

            case Difficulty.Medium:
                SpawnMany(scoutPrefab, scoutSpawnPoints, 2);
                SpawnMany(guardianPrefab, guardianSpawnPoints, 1);
                break;

            case Difficulty.Hard:
                SpawnMany(scoutPrefab, scoutSpawnPoints, 2);
                SpawnMany(guardianPrefab, guardianSpawnPoints, 2);
                break;
        }

        // ---------- MONEDAS SEGÚN DIFICULTAD ----------
        int coinsToSpawn = difficulty switch
        {
            Difficulty.Easy => 3,
            Difficulty.Medium => 4,
            Difficulty.Hard => 5,
            _ => 3
        };

        // Informar al GameManager
        GameManager.Instance.totalCoins = coinsToSpawn;

        // Crear monedas
        for (int i = 0; i < coinsToSpawn; i++)
        {
            Instantiate(
                coinPrefab,
                coinSpawnPoints[i].position,
                Quaternion.identity
            );
        }

    }


    private void SpawnMany(EnemyAIController prefab, List<Transform> points, int count)
    {
        if (prefab == null || points == null || points.Count == 0 || count <= 0) return;

        for (int i = 0; i < count; i++)
        {
            Transform p = points[i % points.Count];

            EnemyAIController enemy =
                Instantiate(prefab, p.position, p.rotation);

            enemy.graph = FindFirstObjectByType<NavGraph>();


            enemy.enemyType = prefab == scoutPrefab
                ? EnemyType.Scout
                : EnemyType.Guardian;

            // APLICAR CONFIGURACIÓN SEGÚN TIPO
            enemy.ConfigureByType();

            Transform[] wp = new Transform[waypointsRoot.childCount];
            for (int j = 0; j < waypointsRoot.childCount; j++)
            {
                wp[j] = waypointsRoot.GetChild(j);
            }

            enemy.patrolPoints = wp;

            //OPCIONAL
            var patrol = enemy.GetComponent<DronePatrol>();
            if (patrol != null)
            {
                patrol.SetWaypoints(wp);
            }



            spawned.Add(enemy);
        }

    }

}
