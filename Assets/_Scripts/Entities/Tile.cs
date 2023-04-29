using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject _highLight;
    
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
        _highLight.SetActive(true);
        if(Input.GetMouseButton(0)) Select();
    }
    private void OnMouseExit()
    {
        _highLight.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (!IsInteractable) return;
        Select();
    }

    private void Select()
    {
        PathManager.Instance.AddToPath((Vector2)transform.position);
    }
}
