using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapTile : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject _highLight;
    [SerializeField] private GameObject _cross;

    public bool IsInteractable = false;
    public bool IsWalkable = true;

    public void Init(bool isOffset)
    {
        _spriteRenderer.color = isOffset ? _offsetColor : _baseColor;
        
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        if (!IsInteractable) return;

        if (Courier.ActiveInstance != null) _highLight.SetActive(true);
        else {
            Courier courier = Player.Instance.GetCourierByTile(transform.position);
            if(courier != null && courier.IsMoving) _cross.SetActive(true);
            else _highLight.SetActive(true);
        }
        if (Input.GetMouseButton(0)) Select();
    }

    private void OnMouseExit()
    {
        _highLight.SetActive(false);
        _cross.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (!IsInteractable) return;
        Select();
    }
    private void OnMouseUp()
    {
        if (GameManager.Instance.State == GameState.Pathing)
        {
            Courier.ActiveInstance.SetTarget(Courier.ActiveInstance.PathManager.GetCurrentTarget());
            GameManager.Instance.SetState(GameState.Playing);
            Courier.ActiveInstance = null;
        }
    }

    private void Select()
    {
        _highLight.SetActive(false);
        _cross.SetActive(false);
        if (Courier.ActiveInstance == null)
        {
            Courier courier = Player.Instance.GetCourierByTile(transform.position);
            if (courier == null) return;
            courier.SetActive();
            if(courier.IsMoving)
            {
                courier.StopAtNextTarget();
                Courier.ActiveInstance = null;
                return;
            }
            GameManager.Instance.SetState(GameState.Pathing);
            courier.StopAtNextTarget(false);
        }
        if (GameManager.Instance.State == GameState.Pathing)
        {
            if(Courier.ActiveInstance) Courier.ActiveInstance.PathManager.AddToPath((Vector2)transform.position);
        }
    }
}
