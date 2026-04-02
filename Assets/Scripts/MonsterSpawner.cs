using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    public List<Monster> monsterPrefabs;
    public List<SpawnPoint> spawnPoints;

    private Dictionary<Monster.MonsterType, Queue<Monster>> monsterPool;
    private Dictionary<Monster.MonsterType, Monster> monsterPrefabDict;
    private Dictionary<Monster.MonsterType, List<Vector3>> monsterSpawnPoints;

    private SpawnTime[] spawnQueue;
    private float spawnTimer;

    struct SpawnTime
    {
        public Monster.MonsterType type;
        public float time;
    }

    #region Unity Message
    private void Awake()
    {
        monsterPool = new();
        monsterPrefabDict = new();
        monsterSpawnPoints = new();
        spawnQueue = new SpawnTime[monsterPrefabs.Count];
        spawnTimer = 0f;

        int queueIndex = 0;
        foreach (Monster monster in monsterPrefabs)
        {
            if (!monsterPrefabDict.ContainsKey(monster.data.monsterType))
            {
                monsterPrefabDict[monster.data.monsterType] = monster;
                monsterPool[monster.data.monsterType] = new Queue<Monster>();
                spawnQueue[queueIndex++] = new SpawnTime { type = monster.data.monsterType, time = monster.data.spawnInterval };
            }
            else
            {
                Debug.LogError("Duplicate monster type in monsterPrefabs: " + monster.data.monsterType);
            }
        }

        foreach (SpawnPoint spawnPoint in spawnPoints)
        {
            if (!monsterSpawnPoints.ContainsKey(spawnPoint.spawnPointType))
            {
                monsterSpawnPoints[spawnPoint.spawnPointType] = new List<Vector3>();
            }

            monsterSpawnPoints[spawnPoint.spawnPointType].Add(spawnPoint.transform.position);
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;

        for (int i = 0; i < spawnQueue.Length; i++)
        {
            if (spawnQueue[i].time <= spawnTimer)
            {
                Vector3 spawnPos= monsterSpawnPoints[spawnQueue[i].type][Random.Range(0, monsterSpawnPoints[spawnQueue[i].type].Count)];
                SpawnMonster(spawnQueue[i].type, spawnPos);
                spawnQueue[i].time += monsterPrefabDict[spawnQueue[i].type].data.spawnInterval;
            }
        }
    }
    #endregion

    public void SpawnMonster(Monster.MonsterType type, Vector3 position)
    {
        if (!monsterPrefabDict.ContainsKey(type))
        {
            Debug.LogError($"Monster prefab for type {type} not found.");
            return;
        }

        Monster monster;
        if (monsterPool[type].Count > 0)
        {
            monster = monsterPool[type].Dequeue();
        }
        else
        {
            monster = Instantiate(monsterPrefabDict[type]);
            monster.ReturnToPool += ReturnToPool;
            monster.gameObject.SetActive(false);
        }

        monster.transform.position = position;
        monster.gameObject.SetActive(true);
    }

    void ReturnToPool(Monster monster)
    {
        monster.gameObject.SetActive(false);
        monsterPool[monster.data.monsterType].Enqueue(monster);
    }
}
