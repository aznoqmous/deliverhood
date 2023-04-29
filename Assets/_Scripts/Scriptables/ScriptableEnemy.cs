using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName ="Enemy")]
public class ScriptableEnemy : ScriptableObject
{
    public Sprite Sprite;
    public float Speed = 1f;
    public float Health = 1f;
}
