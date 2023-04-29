using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] float _health = 10f;
    [SerializeField] float _maxHealth = 10f;
    [SerializeField] float _speed = 1f;
    [SerializeField] Transform _target;

    [Header("Animation")]
    [SerializeField] GameObject _spriteContainer;
    [SerializeField] Animator _animator;

    public bool IsMoving = false;
    [SerializeField] public List<ResourceCount> _baseResourceCount = new List<ResourceCount>();
    public Dictionary<ScriptableResource, int> Resources = new Dictionary<ScriptableResource, int>();

    void Start()
    {
        foreach(var resource in _baseResourceCount) {
            Resources[resource.Resource] = resource.Count;
        }
        UIManager.Instance.UpdatePlayerResources();
    }

    void Update()
    {
        
    }

    public void SetTarget(Transform target)
    { 
        _target = target; 
    }

    public void MoveTowardTarget()
    {
        if (!_target)
        {
            IsMoving = false;
            return;
        }
        IsMoving = true;
        Vector3 position = transform.position;
        Vector3 distance = (_target.position - position);
        position = Vector3.Lerp(position, position + distance.normalized, Time.deltaTime * _speed);
        _spriteContainer.transform.localScale = new Vector3(distance.x > 0 ? 1 : -1, 1, 1);
        transform.position = position;
        _animator.SetBool("IsWalking", true);
        if (distance.sqrMagnitude < 0.001f)
        {
            MapManager.Instance.TriggerMapEntityAtPosition(transform.position);
            PathManager.Instance.NextTarget();
            SetTarget(PathManager.Instance.GetCurrentTarget());
            GameManager.Instance.Tick();
            if (!_target)
            {
                _animator.SetBool("IsWalking", false);
                GameManager.Instance.SetState(GameState.Pathing);
            }
        }
    }

    public void GainResource(ScriptableResource resource) {
        Resources[resource]++;
        UIManager.Instance.UpdateResource(resource, Resources[resource]);
        UIManager.Instance.AddStatusText(transform.position, "+1", resource.Sprite);
    }
    public bool ConsumeResource(ScriptableResource resource)
    {
        if (Resources.TryGetValue(resource, out int count)) {
            if (count <= 0) return false;
            Resources[resource]--;
            UIManager.Instance.UpdateResource(resource, Resources[resource]);
            return true;
        };
        
        return false;
    }
}
