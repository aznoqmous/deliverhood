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

    [SerializeField] private Vector2 _startPosition;
    [Header("Tiles")]
    [SerializeField] private GameObject _tilesContainer;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Dictionary<Vector2, Tile> _tiles;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap _floorTilemap;
    [SerializeField] private Tilemap _offGroundTilemap;
    [SerializeField] private TileBase _offGroundTile;
    [SerializeField] private Tilemap _otherTilemap;
    private List<Vector3> _availablePlaces;
    
    [SerializeField] MapEntityController _mapEntityPrefab;
    [SerializeField] Dictionary<Vector2, MapEntityController> _mapEntities = new Dictionary<Vector2, MapEntityController>();

    [SerializeField] Transform _pathParent;
    [SerializeField] GameObject _pathContainerPrefab;

    private float _width
    {
        get
        {
            return _floorTilemap.cellBounds.xMax - _floorTilemap.cellBounds.xMin;
        }
    }
    private float _height
    {
        get
        {
            return _floorTilemap.cellBounds.yMax - _floorTilemap.cellBounds.yMin;
        }
    }

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
                    _offGroundTilemap.SetTile(localPlace, _offGroundTile);
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
        _tiles = new Dictionary<Vector2, Tile>();
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

    public Tile GetTileAtPosition(Vector2 position)
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

    public Tile GetStartTile()
    {
        return GetTileAtPosition(_startPosition);
    }

    public List<Tile> GetNeighbourTiles(Vector2 position)
    {
        List<Tile> tiles = new List<Tile>();
        Tile leftTile = GetTileAtPosition(new Vector2(position.x - 1, position.y));
        Tile rightTile = GetTileAtPosition(new Vector2(position.x + 1, position.y));
        Tile topTile = GetTileAtPosition(new Vector2(position.x, position.y + 1));
        Tile bottomTile = GetTileAtPosition(new Vector2(position.x, position.y - 1));
        if (leftTile != null) tiles.Add(leftTile);
        if(rightTile !=null) tiles.Add(rightTile);
        if(topTile!=null) tiles.Add(topTile);
        if(bottomTile!=null) tiles.Add(bottomTile);
        return tiles;
    }
    public List<Tile> GetNeighbourWalkableTiles(Vector2 position)
    {
        List<Tile> tiles = GetNeighbourTiles(position);
        return tiles.Where(t => t.IsWalkable).ToList();
    }
    public Tile GetPlayerTile()
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
