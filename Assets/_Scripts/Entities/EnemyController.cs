using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] SpriteRenderer _spriteRenderer;
    float _speed = 1f;
    float _health = 1f;

    public void Load(ScriptableEnemy e)
    {
        _speed = e.Speed;
        _health = e.Health;
        _spriteRenderer.sprite = e.Sprite;
    }

    void Update()
    {
        
    }
}
