using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Resource", menuName = "Resource")]
public class ScriptableResource : ScriptableObject
{
    public Sprite Sprite;
    public float Cost;
}

[Serializable]
public class ResourceCount
{
    public ScriptableResource Resource;
    public int Count = 0;
}