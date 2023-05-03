using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public static PathManager ActiveInstance;

    private void Start()
    {
        _pathContainer = MapManager.Instance.GenerateNewPathContainer();
    }

    public void SetActive(bool state = true)
    {
        if(state) ActiveInstance= this;
        else if(ActiveInstance == this) ActiveInstance = null;
    }

    [SerializeField] private List<Vector2> _path = new List<Vector2>();
    [SerializeField] private List<PathTile> _pathTiles = new List<PathTile>();
    [SerializeField] private PathTile _pathTilePrefab;
    [SerializeField] private Transform _pathContainer;
    private int _currentIndex = 0;
    private int _direction = 1;
    private Color _color;
    private int _maxPathLength = 24;

    public void AddToPath(Vector2 position)
    {
        if (_currentIndex != 0 && _path[0] == _path[_currentIndex]) return;

        if(
            _path.Contains(position) 
            && (_path.Count > 0 && _path[0] != position)
        )
        {
            int index = _path.IndexOf(position);
            for (int i = index; i < _path.Count; i++)
            {
                Destroy(_pathTiles[i].gameObject);
            }
            _pathTiles.RemoveRange(index, _path.Count - index);
            _path.RemoveRange(index, _path.Count - index);
        }

        if (_path.Count >= _maxPathLength && (_path.Count > 0 && _path[0] != position)) return;

        PathTile lastPathTile = _pathTiles.Count > 0 ? _pathTiles[_pathTiles.Count-1] : null;
        _path.Add(position);
        PathTile newPathTile = Instantiate(_pathTilePrefab, position, Quaternion.identity, _pathContainer);
        newPathTile.SetColor(_color);
        _pathTiles.Add(newPathTile);
        UpdatePathTileSprite(newPathTile);
        if (lastPathTile != null) { UpdatePathTileSprite(lastPathTile); }

        UpdateInteractableTiles();
    }

    public void UpdatePathTileSprite(PathTile pathTile)
    {
        int index = _pathTiles.IndexOf(pathTile);
        PathTile lastPathTile = index - 1 >= 0 ? _pathTiles[index - 1] : null;
        PathTile nextPathTile = index + 1 < _pathTiles.Count ? _pathTiles[index+1] : null;
        pathTile.UpdateSprite(lastPathTile, nextPathTile);
    }

    public void UpdateInteractableTiles()
    {
        MapManager.Instance.ClearInteractable();
        if(_pathTiles.Count <= 0)
        {
            MapTile startTile = MapManager.Instance.GetPlayerTile();
            startTile.IsInteractable = true;
        }
        else
        {
            List<MapTile> tiles = MapManager.Instance.GetNeighbourWalkableTiles(_path[_path.Count-1]);
            //tiles = tiles.Where(t => Player.Instance.IsInRadius(t.transform.position)).ToList();
            foreach(MapTile tile in tiles)
            {
                tile.IsInteractable = true;
            }
        }
    }

    public void EnablePlayModeInteractableTiles()
    {
        foreach(PathTile pathTile in _pathTiles)
        {
            MapTile tile = MapManager.Instance.GetTileAtPosition((Vector2)pathTile.transform.position);
            tile.IsInteractable= true;
        }
    }

    public void Clear()
    {
        _currentIndex = 0;
        foreach(Transform t in _pathContainer) Destroy(t.gameObject);
        _path.Clear();
        _pathTiles.Clear();
    }
    public void ClearAfterIndex()
    {
        _path.RemoveRange(_currentIndex, _path.Count - _currentIndex);
        _pathTiles.RemoveRange(_currentIndex, _pathTiles.Count - _currentIndex);
    }

    public Transform GetCurrentTarget()
    {
        return _currentIndex < _pathTiles.Count ? _pathTiles[_currentIndex].transform : null;
    }

    public bool NextTarget()
    {
        // loop
        if (_currentIndex != 0 && _currentIndex == _pathTiles.Count - 1 && _path[0] == _path[_currentIndex]) _currentIndex = 0;

        if (_currentIndex + _direction >= _pathTiles.Count || _currentIndex + _direction < 0)
        {
            _direction = -_direction;
        }
        _currentIndex += _direction;
        return true;
    }

    public bool PathContains(Vector2 position)
    {
        return _path.Contains(position);
    }

    public void SetColor(Color color)
    {
        _color = color;
    }
}
