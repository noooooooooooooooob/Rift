using System;
using System.Collections.Generic;
using UnityEngine;

public enum SkillTargetType
{
    Solo,             // 단일 대상
    SelfCentered,      // 자기 중심 (주변 공격, 버프)
    TargetCentered,    // 타겟 중심 (파이어볼 같은 원거리 AOE)
    Directional,       // 방향성 (샷건, 부채꼴)
    Line              // 직선 (레이저, 관통)
}

public enum targetType
{
    Enemy,
    Ally,
    Self,
    All
}

[CreateAssetMenu(fileName = "SkillData", menuName = "Scriptable Objects/SkillData")]
public class SkillData : ScriptableObject
{
    [Header("Basic Info")]
    public string skillName;
    public string description;

    [Header("Costs & Cooldown")]
    public int cost;           // SP 소모량

    [Header("Range & Target")]
    public SkillTargetType skillTargetType; // 스킬 사거리 타입
    public targetType targetType; // 스킬 대상 타입
    public int range;            // 스킬 사거리
    public int minRange;         // 최소 사거리
    public List<Vector2Int> area = new List<Vector2Int>();   // 범위 (2D 배열로 표현)
}