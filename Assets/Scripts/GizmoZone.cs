using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GizmoZone : MonoBehaviour
{
    [Header("Gizmo Settings")]
    public GameObject[] cannibalPrefabs;
    public GameObject[] boarPrefabs;
    public float triggerRadius = 500f;
    public float gizmoSpacing = 1000f;
    public int maxLevel = 10;

    [Header("Spawn Control")]
    public int baseCannibalCount = 7;
    public int baseBoarCount = 3;
    public int cannibalCountIncreasePerLevel = 3;
    public int boarCountIncreasePerLevel = 3;
    public float minimumSpawnDistance = 5f;

    [Header("Base Settings")]
    public Vector3 basePosition = Vector3.zero;
    public float baseNoSpawnRadius = 300f;

    private int currentActiveLevel = -1;
    private Transform player;
    private Dictionary<int, List<GameObject>> activeEnemies = new Dictionary<int, List<GameObject>>();

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            else
                return;
        }

        Vector3 playerPos = player.position;
        float distanceFromCenter = Vector3.Distance(playerPos, transform.position);

        int playerLevel = -1;
        for (int level = 1; level <= maxLevel; level++)
        {
            float ringMin = (level - 1) * gizmoSpacing;
            float ringMax = level * gizmoSpacing;

            if (distanceFromCenter <= ringMax && distanceFromCenter > ringMin)
            {
                playerLevel = level;
                break;
            }
        }

        if (playerLevel == -1) return;

        if (playerLevel != currentActiveLevel)
        {
            foreach (var key in new List<int>(activeEnemies.Keys))
            {
                if (key != playerLevel)
                    DestroyEnemies(key);
            }

            if (!activeEnemies.ContainsKey(playerLevel))
                StartCoroutine(SpawnEnemies(playerLevel, player.position));

            currentActiveLevel = playerLevel;
        }
    }

    private IEnumerator SpawnEnemies(int level, Vector3 center)
    {
        List<GameObject> enemies = new List<GameObject>();
        List<Vector3> usedPositions = new List<Vector3>();

        int cannibalCount = baseCannibalCount + cannibalCountIncreasePerLevel * (level - 1);
        int boarCount = baseBoarCount + boarCountIncreasePerLevel * (level - 1);

        // --- Cannibal Spawn ---
        for (int i = 0; i < cannibalCount; i++)
        {
            GameObject prefab = cannibalPrefabs[UnityEngine.Random.Range(0, cannibalPrefabs.Length)];
            float spawnRadius = 200f + (level * 30f);
            Vector3? pos = GetValidSpawnPosition(center, 0f, spawnRadius, usedPositions);

            if (pos == null || IsVisibleToCamera(pos.Value)) continue;

            Vector3 fixedPos = new Vector3(pos.Value.x, 0f, pos.Value.z);
            GameObject cannibal = Instantiate(prefab, fixedPos, Quaternion.identity);
            cannibal.name = $"Cannibal_Lv{level}_{i + 1}";
            SetEnemyLevel(cannibal, level);
            ForceShowHealthBar(cannibal);
            enemies.Add(cannibal);
            usedPositions.Add(fixedPos);
            yield return null;
        }

        // --- Boar Spawn ---
        for (int i = 0; i < boarCount; i++)
        {
            GameObject prefab = boarPrefabs[UnityEngine.Random.Range(0, boarPrefabs.Length)];
            float spawnRadius = 200f + (level * 30f);

            Vector3? pos = GetValidSpawnPosition(center, 0f, spawnRadius, usedPositions);
            if (pos == null || IsVisibleToCamera(pos.Value)) continue;

            Vector3 fixedPos = new Vector3(pos.Value.x, 0f, pos.Value.z);
            GameObject boar = Instantiate(prefab, fixedPos, Quaternion.identity);
            boar.name = $"Boar_Lv{level}_{i + 1}";
            SetEnemyLevel(boar, level);
            ForceShowHealthBar(boar);
            enemies.Add(boar);
            usedPositions.Add(fixedPos);
            yield return null;
        }

        activeEnemies[level] = enemies;
    }

    private Vector3? GetValidSpawnPosition(Vector3 center, float minRadius, float maxRadius, List<Vector3> existingPositions)
    {
        int maxAttempts = 50;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector2 dir = UnityEngine.Random.insideUnitCircle.normalized;
            float radius = UnityEngine.Random.Range(minRadius + 50f, maxRadius - 50f);
            Vector3 pos = center + new Vector3(dir.x, 0f, dir.y) * radius;

            if (Vector3.Distance(pos, basePosition) < baseNoSpawnRadius)
                continue;

            bool tooClose = false;
            foreach (var used in existingPositions)
            {
                if (Vector3.Distance(pos, used) < minimumSpawnDistance)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose)
                return pos;
        }
        return null;
    }

    private bool IsVisibleToCamera(Vector3 pos)
    {
        if (Camera.main == null) return false;
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(pos);
        return viewportPoint.z > 0 &&
               viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
               viewportPoint.y >= 0 && viewportPoint.y <= 1;
    }

    private void DestroyEnemies(int level)
    {
        if (activeEnemies.TryGetValue(level, out List<GameObject> enemies))
        {
            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                GameObject enemy = enemies[i];
                if (enemy != null)
                {
                    if (IsVisibleToCamera(enemy.transform.position))
                        continue; // Görünüyorsa silme

                    Destroy(enemy);
                    enemies.RemoveAt(i);
                }
            }
            if (enemies.Count == 0)
                activeEnemies.Remove(level);
        }
    }

    private void ForceShowHealthBar(GameObject go)
    {
        // HealthEnemy health = go.GetComponentInChildren<HealthEnemy>();
        // if (health != null)
        //     health.gameObject.SetActive(true);
    }

    private void SetEnemyLevel(GameObject go, int level)
    {
        EnemyController enemy = go.GetComponent<EnemyController>();
        if (enemy != null)
        {
            // level'e göre stat güncellemesi
        }
    }

    private void OnDrawGizmosSelected()
    {
        for (int level = 1; level <= maxLevel; level++)
        {
            float radius = level * gizmoSpacing;
            Gizmos.color = Color.Lerp(Color.green, Color.red, level / (float)maxLevel);
            DrawCircle(transform.position, radius);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(basePosition, baseNoSpawnRadius);
    }

    private void DrawCircle(Vector3 center, float radius, int segments = 60)
    {
        Vector3 prevPoint = center + new Vector3(Mathf.Cos(0), 0f, Mathf.Sin(0)) * radius;
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 newPoint = center + new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius;
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }

    private void OnDrawGizmos()
    {
        for (int level = 1; level <= maxLevel; level++)
        {
            float radius = level * gizmoSpacing;
            Gizmos.color = (level == currentActiveLevel)
                ? new Color(1f, 0f, 0f, 0.3f)
                : new Color(0.3f, 0.3f, 0.3f, 0.1f);

            DrawCircle(transform.position, radius);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(basePosition, baseNoSpawnRadius);
    }
}
