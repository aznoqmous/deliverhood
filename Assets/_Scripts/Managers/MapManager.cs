using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] Vector2 _startPosition;
    [SerializeField] int _width = 10;
    [SerializeField] int _height = 10;

    [Header("Tiles")]
    [SerializeField] GameObject _tilesContainer;
    [SerializeField] MapTile _tilePrefab;
    Dictionary<Vector2, MapTile> _tiles;
    [SerializeField] ScriptableMapTile _grassTile;
    [SerializeField] ScriptableMapTile _offGroundTile;
    [SerializeField] ScriptableMapTile _waterTile;

    [Header("Tilemaps")]
    [SerializeField] Tilemap _floorTilemap;
    [SerializeField] Tilemap _offGroundTilemap;
    [SerializeField] Tilemap _otherTilemap;
    List<Vector3> _availablePlaces;

    [Header("MapEntities")]
    [SerializeField] MapEntityController _mapEntityPrefab;
    [SerializeField] Dictionary<Vector2, MapEntityController> _mapEntities = new Dictionary<Vector2, MapEntityController>();

    [Header("Paths")]
    [SerializeField] Transform _pathParent;
    [SerializeField] GameObject _pathContainerPrefab;


    public void LoadAvailablePlaces()
    {
        _availablePlaces = new List<Vector3>();

        for (int n = _floorTilemap.cellBounds.xMin; n < _floorTilemap.cellBounds.xMax; n++)
        {
            for (int p = _floorTilemap.cellBounds.yMin; p < _floorTilemap.cellBounds.yMax; p++)
            {
                Vector3Int localPlace = (new Vector3Int(n, p, (int)_floorTilemap.transform.position.y));
                Vector3 place = _floorTilemap.CellToWorld(localPlace);
                if (_floorTilemap.HasTile(localPlace))
                {
                    //Tile at "place"
                    _availablePlaces.Add(place);
                }
                else
                {
                    //No tile at "place"
                }
            }
        }
    }
    public void DrawOffGround()
    {
        for (int n = _floorTilemap.cellBounds.xMin; n < _floorTilemap.cellBounds.xMax; n++)
        {
            for (int p = _floorTilemap.cellBounds.yMin; p < _floorTilemap.cellBounds.yMax; p++)
            {
                Vector3Int localPlace = (new Vector3Int(n, p, (int)_floorTilemap.transform.position.y));
                Vector3 place = _floorTilemap.CellToWorld(localPlace);
                if (_floorTilemap.HasTile(localPlace))
                {
                }
                else
                {
                    _offGroundTilemap.SetTile(localPlace, _offGroundTile.Tiles[0]);
                }
            }
        }
    }
    public void GenerateGrid()
    {
        _floorTilemap.tileAnchor = Vector3.zero;
        _offGroundTilemap.tileAnchor = Vector3.zero;
        _otherTilemap.tileAnchor = Vector3.zero;
        LoadAvailablePlaces();
        DrawOffGround();

        _startPosition = _availablePlaces[Random.Range(0, _availablePlaces.Count)];
        _tiles = new Dictionary<Vector2, MapTile>();
        foreach(Vector2 position in _availablePlaces)
        {
            var tile = Instantiate(_tilePrefab, position, Quaternion.identity, _tilesContainer.transform);
            tile.name = $"Tile {position.x} {position.y}";
            var isOffset = (position.x % 2 == 0 && position.y % 2 != 0) || (position.x % 2 != 0 && position.y % 2 == 0);
            tile.Init(isOffset);
            _tiles[new Vector2(position.x, position.y)] = tile;
        }
    }

    public void ClearInteractable()
    {
        foreach(var item in _tiles)
        {
            item.Value.IsInteractable = false;
        }
    }

    public MapTile GetTileAtPosition(Vector2 position)
    {
        return _tiles.TryGetValue(new Vector2(
            Mathf.RoundToInt(position.x),
            Mathf.RoundToInt(position.y)
        ), out var tile) ? tile : null;
    }

    public Vector2 GetStartPosition()
    {
        return _startPosition;
    }

    public MapTile GetStartTile()
    {
        return GetTileAtPosition(_startPosition);
    }

    public List<MapTile> GetNeighbourTiles(Vector2 position)
    {
        List<MapTile> tiles = new List<MapTile>();
        MapTile leftTile = GetTileAtPosition(new Vector2(position.x - 1, position.y));
        MapTile rightTile = GetTileAtPosition(new Vector2(position.x + 1, position.y));
        MapTile topTile = GetTileAtPosition(new Vector2(position.x, position.y + 1));
        MapTile bottomTile = GetTileAtPosition(new Vector2(position.x, position.y - 1));
        if (leftTile != null) tiles.Add(leftTile);
        if(rightTile !=null) tiles.Add(rightTile);
        if(topTile!=null) tiles.Add(topTile);
        if(bottomTile!=null) tiles.Add(bottomTile);
        return tiles;
    }
    public List<MapTile> GetNeighbourWalkableTiles(Vector2 position)
    {
        List<MapTile> tiles = GetNeighbourTiles(position);
        return tiles.Where(t => t.IsWalkable).ToList();
    }
    public MapTile GetPlayerTile()
    {
        return GetTileAtPosition(Player.Instance.transform.position);
    }

    public void SpawnMapEntities(List<ScriptableMapEntity> entities)
    {
        List<Vector3> availablePositions = new List<Vector3>(_availablePlaces);
        availablePositions =availablePositions.Where(position => 
        Player.Instance.IsInRadius(position, 1f) 
        && GetMapEntityAtPosition(position) == null
        && position.magnitude > Player.Instance.Level
        ).ToList();
        foreach (ScriptableMapEntity sme in entities)
        {
            Vector3 position = availablePositions[Random.Range(0, availablePositions.Count)];
            availablePositions.Remove(position);
            MapEntityController newMapEntity = Instantiate(_mapEntityPrefab, position, Quaternion.identity, transform);
            newMapEntity.Load(sme);
            _mapEntities[(Vector2)position] = newMapEntity;
        }
    }
    public void TickMapEntities()
    {
        foreach(var me in _mapEntities)
        {
            me.Value.Tick();
        }
    }

    public MapEntityController GetMapEntityAtPosition(Vector2 position)
    {
        _mapEntities.TryGetValue(new Vector2(
            Mathf.RoundToInt(position.x),
            Mathf.RoundToInt(position.y)
        ), out var mapEntity);
        return mapEntity;
    }

    public void TriggerMapEntityAtPosition(Vector2 position, Courier Courier) 
    {
        MapEntityController mapEntity = GetMapEntityAtPosition(position);
        if(mapEntity!= null)
        {
            mapEntity.Activate(Courier);
        }
    }

    public Transform GenerateNewPathContainer()
    {
        return Instantiate(_pathContainerPrefab, _pathParent).transform;
    }
    public void Clear()
    {
        ClearInteractable();
        foreach (var me in _mapEntities) Destroy(me.Value.gameObject);
        foreach(var te in _tiles) Destroy(te.Value.gameObject);
        _mapEntities.Clear();
        _tiles.Clear();
    }
}
