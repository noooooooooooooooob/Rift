using System.Collections.Generic;
using UnityEngine;

public class EnemyData
{
    public GameObject enemyPrefab;
    public Vector2Int positions; // if(-1,-1) -> 랜덤 스폰
}

[CreateAssetMenu(fileName = "BattleMap", menuName = "Scriptable Objects/BattleMap")]
public class BattleMap : ScriptableObject
{
    public List<EnemyData> enemys;
    public int awardGold;
}
