using UnityEngine;

public class Unit_Attack : MonoBehaviour
{
    public Unit unit;
    public SkillData attackSkillData; // 기본 공격 스킬 데이터
    public float animationDuration;

    private void Awake()
    {
        if (unit == null)
            unit = GetComponent<Unit>();
    }

    public virtual void ExecuteAttack(Unit targetUnit)
    {
        if (attackSkillData == null)
        {
            Debug.LogWarning("기본 공격 스킬 데이터가 할당되지 않았습니다!");
            return;
        }

        unit.unitAnimation.PlayAttackAnimation();
        unit.Attack(targetUnit, 10);
    }
}
