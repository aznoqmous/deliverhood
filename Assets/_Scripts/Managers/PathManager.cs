using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public static PathManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] private List<Vector2> _path = new List<Vector2>();
    [SerializeField] private List<PathTile> _pathTiles = new List<PathTile>();
    [SerializeField] private PathTile _pathTilePrefab;
    private int _currentIndex = 0;

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

        PathTile lastPathTile = _pathTiles.Count > 0 ? _pathTiles[_pathTiles.Count-1] : null;
        _path.Add(position);
        PathTile newPathTile = Instantiate(_pathTilePrefab, position, Quaternion.identity, transform);
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
        /*foreach(PathTile pathTile in _pathTiles)
        {
            Tile tile = MapManager.Instance.GetTileAtPosition((Vector2)pathTile.transform.position);
            tile.IsInteractable= true;
        }*/
        if(_pathTiles.Count <= 0)
        {
            Tile startTile = MapManager.Instance.GetPlayerTile();
            startTile.IsInteractable = true;
        }
        else
        {
            List<Tile> tiles = MapManager.Instance.GetNeighbourWalkableTiles(_path[_path.Count-1]);
            foreach(Tile tile in tiles)
            {
                tile.IsInteractable = true;
            }
        }
    }

    public void Clear()
    {
        _currentIndex = 0;
        foreach(Transform t in transform) Destroy(t.gameObject);
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
        if (_currentIndex != 0 && _currentIndex == _pathTiles.Count - 1 && _path[0] == _path[_currentIndex]) _currentIndex = 0;

        if (_currentIndex + 1 > _pathTiles.Count) return false;
        _currentIndex++;
        return true;
    }


}
