using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    public static EntityManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] EnemyController _enemyPrefab;
    List<EnemyController> _enemies;

    EnemyController SpawnEnemy(ScriptableEnemy s, Vector2 position)
    {
        EnemyController enemy = Instantiate(_enemyPrefab, position, Quaternion.identity, transform);
        enemy.Load(s);
        return enemy;
    }
}
