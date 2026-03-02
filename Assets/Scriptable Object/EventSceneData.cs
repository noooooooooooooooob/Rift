using UnityEngine;

public enum EventEffectType
{
    None,
    HealPercent,    // 비율 회복 (value = 30이면 30%)
    HealFixed,      // 고정값 회복
    DamagePercent,  // 비율 대미지 (value = 30이면 30%)
    Gold,
    DamageFixed     // 고정값 대미지
}

[System.Serializable]
public class EventChoice
{
    public string choiceText;           // 선택지 텍스트

    [Header("Cost (선택 시 무조건 적용)")]
    public EventEffectType costType = EventEffectType.None;
    public int costValue;

    [Header("Success")]
    [Range(0f, 1f)]
    public float successRate = 1f;      // 성공 확률 (1 = 100%)
    public EventEffectType successEffectType;
    public int successValue;
    [TextArea(2, 3)]
    public string successText;          // 성공 시 텍스트

    [Header("Fail")]
    public EventEffectType failEffectType = EventEffectType.None;
    public int failValue;
    [TextArea(2, 3)]
    public string failText;             // 실패 시 텍스트
}

[CreateAssetMenu(fileName = "EventSceneData", menuName = "Scriptable Objects/EventSceneData")]
public class EventSceneData : ScriptableObject
{
    public Sprite image;                // 이벤트 이미지
    [TextArea(3, 5)]
    public string description;          // 이벤트 설명
    public EventChoice[] choices;       // 선택지들
}
