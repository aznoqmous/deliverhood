using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Burst.Intrinsics.Arm;

public class Player : MonoBehaviour
{
    public static Player Instance;
    private void Awake()
    {
        Instance = this;
    }

    [SerializeField] public List<ResourceCount> _baseResourceCount = new List<ResourceCount>();
    public Dictionary<ScriptableResource, int> Resources = new Dictionary<ScriptableResource, int>();
    public ScriptableResource CoinResource;

    [Header("Courier")]
    [SerializeField] List<Courier> _Couriers = new List<Courier>();
    [SerializeField] Courier _CourierPrefab;
    [SerializeField] List<Color> _CourierColors = new List<Color>();

    [Header("Level")]
    public int Level = 0;
    public float CurrentSatisfaction = 0;
    public float MaxSatisfaction = 5;
    public List<ScriptableResource> AvailableResources;
    [SerializeField] private List<Level> _levels;

    public void Reset()
    {
        AvailableResources.Clear();
        Resources.Clear();
        Level = 0;
        CurrentSatisfaction = 0;
        MaxSatisfaction = 5;
        foreach (Courier courier in _Couriers)
        {
            courier.Destroy();
        }
        _Couriers.Clear();
        SetSatisfaction(MaxSatisfaction / 2f);
        foreach (var resource in _baseResourceCount)
        {
            Resources[resource.Resource] = resource.Count;
        }
        HandleLevel(0);
    }

    void Start()
    {
        SetSatisfaction(MaxSatisfaction / 2f);
        foreach (var resource in _baseResourceCount) {
            Resources[resource.Resource] = resource.Count;
        }
        HandleLevel(0);
    }

    public void GainResource(ScriptableResource resource, int amount=1) {
        Resources[resource]+=amount;
        UIManager.Instance.UpdateResource(resource, Resources[resource]);
    }
    public bool ConsumeResource(ScriptableResource resource, int amount=1)
    {
        if (Resources.TryGetValue(resource, out int count)) {
            if (count <= 0) return false;
            Resources[resource] -= amount;
            if (Resources[resource] < 0) return false;
            UIManager.Instance.UpdateResource(resource, Resources[resource]);
            return true;
        };
        
        return false;
    }

    public void MoveCouriers()
    {
        foreach (Courier Courier in _Couriers) Courier.MoveTowardTarget();
    }

    public void EnableInteractableTiles()
    {
        foreach (Courier Courier in _Couriers) Courier.EnableInteractableTiles();
    }

    public Courier GetCourierByTile(Vector2 position)
    {
        foreach (Courier Courier in _Couriers) if (Vector2Int.RoundToInt(Courier.transform.position) == Vector2Int.RoundToInt(position)) return Courier;

        foreach (Courier Courier in _Couriers) if (Courier.PathContains(position)) return Courier;

        return null;
    }

    public Courier AddCourier(Vector2 position)
    {
        Courier Courier = Instantiate(_CourierPrefab, position, Quaternion.identity, transform);
        Courier.transform.position = (Vector3)position;
        Courier.SetColor(_CourierColors[_Couriers.Count % _CourierColors.Count]);
        _Couriers.Add(Courier);
        return Courier;
    }

    public void AddSatisfaction(float amount)
    {
        SetSatisfaction(CurrentSatisfaction + amount);
    }
    public void RemoveSatisfaction(float amount)
    {
        SetSatisfaction(CurrentSatisfaction - amount);
    }
    public void SetSatisfaction(float value)
    {
        CurrentSatisfaction = Mathf.Max(0, value);
        if (CurrentSatisfaction >= MaxSatisfaction) LevelUp();
    }

    public void LevelUp()
    {
        Level++;
        MaxSatisfaction *= 2;
        UIManager.Instance.ResetSatisfaction();
        UIManager.Instance.UpdatePlayerLevel();
        HandleLevel(Level);
    }
    public void HandleLevel(int index)
    {
        Level level = _levels[index%_levels.Count];
        foreach(ScriptableMapEntity entity in level.NewEntities)
        {
            if(entity.GainedResources != null)
            {
                foreach(ScriptableResource resource in entity.GainedResources)
                {
                    if(!AvailableResources.Contains(resource))
                    {
                        AvailableResources.Add(resource);
                        Resources[resource] = 0;
                    }
                }
            }
        }
        MapManager.Instance.SpawnMapEntities(level.NewEntities);
        if(level.AddCourrier) AddCourier(Vector2.zero);
        UIManager.Instance.UpdatePlayerResources();
    }

    public float GetLevelRadius()
    {
        return (8 + Level);
    }
    public bool IsInRadius(Vector2 position, float modifier=0f)
    {
        return position.magnitude * 2f + modifier < GetLevelRadius();
    }

    public float GetLevelMaxSatisfaction() {
        return Mathf.Pow(1.5f, Level+1);
    }

    public bool RemoveCoins(int amount) {
        return ConsumeResource(CoinResource, amount);
    }
    public int GetCurrentCoins()
    {
        return Resources[CoinResource];
    }
}


[Serializable]
public class Level
{
    public List<ScriptableMapEntity> NewEntities;
    public bool AddCourrier;
}