using Unity.Hierarchy;
using UnityEngine;

[CreateAssetMenu(fileName = "MobData", menuName = "Scriptable Objects/MobData")]
public class MonsterData : ScriptableObject
{
    public Monster.MonsterType monsterType;

    public float maxHealth = 40f;
    public float moveSpeed = 4f;
    public float damage = 6f;
    public int score = 10;
    public float attackInterval = 1f;
    public float traceDistance = 10f;
    public float spawnInterval = 4f;

    public AudioClip damagedClip;
    public AudioClip deadClip;

}
