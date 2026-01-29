using UnityEngine;

public enum EventEffectType
{
    None,
    HealPercent,    // 비율 회복 (value = 30이면 30%)
    HealFixed,      // 고정값 회복
    DamagePercent,  // 비율 대미지 (value = 30이면 30%)
    DamageFixed     // 고정값 대미지
}

[System.Serializable]
public class EventChoice
{
    public string choiceText;           // 선택지 텍스트
    public EventEffectType effectType;  // 효과 타입
    public int value;                   // 비율(%)이면 정수로, 고정값이면 그대로
}

[CreateAssetMenu(fileName = "EventSceneData", menuName = "Scriptable Objects/EventSceneData")]
public class EventSceneData : ScriptableObject
{
    public Sprite image;                // 이벤트 이미지
    public Sprite endImage;             // 이벤트 종료 후 이미지
    [TextArea(3, 5)]
    public string description;          // 이벤트 설명
    public string endDescription;       // 이벤트 종료 후 설명
    public EventChoice[] choices;       // 선택지들
}
