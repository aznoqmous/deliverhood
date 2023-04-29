using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Map Entity", menuName = "Map Entity")]
public class ScriptableMapEntity : ScriptableObject
{
    public Sprite Sprite;
    public MapEntityType Type;

    public List<ScriptableResource> ConsumedResources;
    public List<ScriptableResource> GainedResources;

    public float Cooldown;
}

public enum MapEntityType
{
    City,
    Resource,
    Trade,
    Consumer
}