using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Courier : MonoBehaviour
{
    public static Courier ActiveInstance;
    public void SetActive(bool state = true)
    {
        if (state) ActiveInstance = this;
        else if (ActiveInstance == this) ActiveInstance = null;
    }

    [SerializeField] float _speed = 1f;
    [SerializeField] Transform _target;

    [Header("Animation")]
    [SerializeField] GameObject _spriteContainer;
    [SerializeField] SpriteRenderer _spriteRenderer;
    [SerializeField] Animator _animator;
    [SerializeField] private Color _color;

    public bool IsMoving = false;
    public bool _stopAtNextTarget = false;

    public PathManager PathManager;

    public void SetColor(Color color)
    {
        _color = color;
        PathManager.SetColor(color);
        //_spriteRenderer.color = color;
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
            MapManager.Instance.TriggerMapEntityAtPosition(transform.position, this);

            if (!_stopAtNextTarget)
            {
                PathManager.NextTarget();
                SetTarget(PathManager.GetCurrentTarget());
            }
            else
            {
                _target = null;
                PathManager.Clear();
                PathManager.UpdateInteractableTiles();
                Courier.ActiveInstance = null;

            }
            if (!_target)
            {
                _animator.SetBool("IsWalking", false);
                PathManager.Clear();
                //GameManager.Instance.SetState(GameState.Pathing);
            }
        }
    }

    public void EnableInteractableTiles()
    {
        Tile tile = MapManager.Instance.GetTileAtPosition(transform.position);
        if(tile!= null) tile.IsInteractable = true;
        PathManager.EnablePlayModeInteractableTiles();
    }

    public bool PathContains(Vector2 position)
    {
        return PathManager.PathContains(position);
    }

    public void GainResource(ScriptableResource resource)
    {
        Player.Instance.GainResource(resource);
        UIManager.Instance.AddStatusText(transform.position, "+1", resource.Sprite);
    }

    public void StopAtNextTarget(bool state=true)
    {
        _stopAtNextTarget = state;
    }

    public void Destroy()
    {
        PathManager.Clear();
        Destroy(gameObject);
    }
}
