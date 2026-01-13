using System;
using System.Collections.Generic;
using UnityEngine;

public enum SkillTargetType
{
    Solo,             // 단일 대상
    Area,             // 범위 대상
    Global            // 전역 대상
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
    public List<Vector2Int> area = new List<Vector2Int>();   // 범위 (2D 배열로 표현)
}