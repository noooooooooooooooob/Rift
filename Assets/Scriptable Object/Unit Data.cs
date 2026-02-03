using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public class UnitData : ScriptableObject
{
    [Header("Basic Info")]
    public string unitName;
    public string unitClass;
    public string unitDescription;
    public GameObject unitTurnGaugePrefab; // 턴 게이지 UI에 사용될 프리팹

    [Header("Visual")]
    public Sprite unitPortrait;      // 큰 초상화 (대화, 상세 정보용)
    public Sprite turnGaugeIcon;     // 작은 아이콘 (턴 게이지 UI용)

    [Header("Stats")]
    public UnitStat baseStat;
}
