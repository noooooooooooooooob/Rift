using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossMap", menuName = "Scriptable Objects/BossMap")]
public class BossMap : ScriptableObject
{
    public List<EnemyData> enemys;
    public int awardGold;
}
