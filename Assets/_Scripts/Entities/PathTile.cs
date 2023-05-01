using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PathTile : MonoBehaviour
{

    [SerializeField] SpriteRenderer _lastSpriteRenderer;
    [SerializeField] SpriteRenderer _nextSpriteRenderer;

    [SerializeField] Sprite _startSprite;
    [SerializeField] Sprite _rightSprite;
    [SerializeField] Sprite _endSprite;

    public void SetColor(Color color)
    {
        _lastSpriteRenderer.color = color;
        _nextSpriteRenderer.color = color;
    }

    public void UpdateSprite(PathTile lastPath, PathTile nextPath)
    {
        if(lastPath == null)
        {
            _lastSpriteRenderer.sprite = _startSprite;
        }
        else
        {
            _lastSpriteRenderer.sprite = _rightSprite;
            switch (GetPathTilePosition(lastPath))
            {
                case PathTilePosition.Top:
                    _lastSpriteRenderer.transform.localEulerAngles = new Vector3(0, 0, -90);
                    break;
                case PathTilePosition.Bottom:
                    _lastSpriteRenderer.transform.localEulerAngles = new Vector3(0, 0, 90);
                    break;
                case PathTilePosition.Left:
                    _lastSpriteRenderer.transform.localEulerAngles = new Vector3(0, 0, 180);
                    break;
                case PathTilePosition.Right:
                    _lastSpriteRenderer.transform.localEulerAngles = new Vector3(0, 0, 0);
                    break;
                case PathTilePosition.Unknown:
                    break;
                default:
                    break;
            }
        }
        if (nextPath == null)
        {
            if(lastPath != null)
            {
                _nextSpriteRenderer.sprite = _endSprite;
                switch (GetPathTilePosition(lastPath))
                {
                    case PathTilePosition.Top:
                        _nextSpriteRenderer.transform.localEulerAngles = new Vector3(0, 0, 90);
                        break;
                    case PathTilePosition.Bottom:
                        _nextSpriteRenderer.transform.localEulerAngles = new Vector3(0, 0, -90);
                        break;
                    case PathTilePosition.Left:
                        _nextSpriteRenderer.transform.localEulerAngles = new Vector3(0, 0, 0);
                        break;
                    case PathTilePosition.Right:
                        _nextSpriteRenderer.transform.localEulerAngles = new Vector3(0, 0, 180);
                        break;
                    case PathTilePosition.Unknown:
                        break;
                    default:
                        break;
                }
            }

        }
        else
        {
            _nextSpriteRenderer.sprite = _rightSprite;
            switch (GetPathTilePosition(nextPath))
            {
                case PathTilePosition.Top:
                    _nextSpriteRenderer.transform.localEulerAngles = new Vector3(0, 0, -90);
                    break;
                case PathTilePosition.Bottom:
                    _nextSpriteRenderer.transform.localEulerAngles = new Vector3(0, 0, 90);
                    break;
                case PathTilePosition.Left:
                    _nextSpriteRenderer.transform.localEulerAngles = new Vector3(0, 0, 180);
                    break;
                case PathTilePosition.Right:
                    _nextSpriteRenderer.transform.localEulerAngles = new Vector3(0, 0, 0);
                    break;
                case PathTilePosition.Unknown:
                    break;
                default:
                    break;
            }
        }
    }

    public PathTilePosition GetPathTilePosition(PathTile pathTile)
    {
        if (pathTile.transform.position.x > transform.position.x) return PathTilePosition.Right;
        if (pathTile.transform.position.x < transform.position.x) return PathTilePosition.Left;
        if (pathTile.transform.position.y > transform.position.y) return PathTilePosition.Bottom;
        if (pathTile.transform.position.y < transform.position.y) return PathTilePosition.Top;

        return PathTilePosition.Unknown;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}

public enum PathTilePosition
{
    Top, Bottom, Left, Right, Unknown
}