using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEntityData", menuName = "Data/Entity", order = 0)]
public class EntityData : ScriptableObject
{
    public int actionPoints, attack, range, health, armor, speed;
}
