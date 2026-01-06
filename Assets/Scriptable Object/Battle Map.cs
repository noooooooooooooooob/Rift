using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BattleMap", menuName = "Scriptable Objects/BattleMap")]
public class BattleMap : ScriptableObject
{
    public List<GameObject> monsterPrefab;
}
