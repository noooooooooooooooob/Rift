using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit_Ultimate : MonoBehaviour
{
    public Unit unit;
    public SkillData ultimateSkillData; // 궁극기 스킬 데이터
    protected List<Unit> targetUnits = new List<Unit>();

    private void Awake()
    {
        if (unit == null)
            unit = GetComponent<Unit>();
    }

    /// <summary>
    /// 궁극기 실행 (코루틴)
    /// </summary>
    public virtual IEnumerator ExecuteUltimate(List<Tile> tiles)
    {
        if (ultimateSkillData == null)
        {
            Debug.LogWarning("궁극기 스킬 데이터가 할당되지 않았습니다!");
            yield break;
        }

        // UP 소모
        unit.currentUP = 0;
        unit.upBar.UpdateBar(0, unit.stats.UP);
        unit.actionMenuUI.ultimateButton.interactable = false;

        // 타겟 수집
        targetUnits.Clear();
        foreach (var tile in tiles)
        {
            if (tile.occupant != null)
            {
                Unit targetUnit = tile.occupant.GetComponent<Unit>();
                if (targetUnit != null)
                    targetUnits.Add(targetUnit);
            }
        }

        // 카메라 포커스 설정
        CameraManager.instance.SetBeforeUltimate(unit, targetUnits);

        // 궁극기 Timeline 재생
        yield return StartCoroutine(unit.unitAnimation.PlayUltimateTimeline());

        // 카메라 복구
        CameraManager.instance.SetAfterUltimate();

        Debug.Log($"[Ultimate] {unit.unitData.unitName} 궁극기 사용 완료!");
    }

    /// <summary>
    /// Timeline Signal에서 호출 - 데미지 적용
    /// </summary>
    public virtual void Ultimate_Attack(float multiplier)
    {
        foreach (var targetUnit in targetUnits)
        {
            if (targetUnit != null && !targetUnit.isDead)
            {
                int damage = Method.CalculateDamage(unit, targetUnit, multiplier);
                targetUnit.TakeDamage(damage);
                Method.CalculateLifeSteal(unit, damage);
            }
        }
    }
}
