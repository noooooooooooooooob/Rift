using UnityEngine;

/// <summary>
/// 유닛의 전투 스탯을 정의하는 구조체
/// UnitData ScriptableObject에 기본값으로 저장되며, 전투 중에는 각 유닛이 독립적인 복사본을 가짐
/// </summary>
[System.Serializable]  // Unity Inspector에서 편집 가능하도록
public class UnitStat
{
    public int HP;   // Health Points - 체력
    public int UP;   // Ultimate Points - 궁극기 포인트
    public int ATK;  // Attack - 공격력
    public int DEF;  // Defense - 방어력
    public int SPD;  // Speed - 속도 (턴 게이지 증가 속도에 영향)
    public int AGI;  // Agility - 민첩성 (턴 게이지 및 회피에 영향)

    [Range(0f, 1f)] public float LS;        // Life Steal - 생명력 흡수 (최종 데미지의 비율만큼 회복)
    [Range(0f, 1f)] public float RST;       // Resistance - 저항력 (상태이상 저항)
    [Range(0f, 1f)] public float EV;        // Evasion - 회피 확률 (0~1, 1 = 100%)
    [Range(0f, 1f)] public float CA;        // Counter Attack - 반격 확률 (0~1)
    [Range(0f, 0.99f)] public float DR;     // Damage Reduction - 피해 감소율 (0~0.99, 최종 피해에 적용)
    public float AP;                        // Armor Penetration - 방어구 관통 (상대 DEF를 무시)

    /// <summary>
    /// 깊은 복사 메서드
    /// 각 유닛이 독립적인 스탯을 가지도록 복사본 생성
    /// 버프/디버프 적용 시 원본 데이터를 보존하기 위해 필요
    /// </summary>
    public UnitStat Clone()
    {
        return new UnitStat
        {
            HP = this.HP,
            UP = this.UP,
            ATK = this.ATK,
            DEF = this.DEF,
            SPD = this.SPD,
            AGI = this.AGI,
            LS = this.LS,
            RST = this.RST,
            EV = this.EV,
            CA = this.CA,
            DR = this.DR,
            AP = this.AP
        };
    }
}