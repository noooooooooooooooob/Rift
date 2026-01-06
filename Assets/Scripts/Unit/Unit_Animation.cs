using UnityEngine;

public class Unit_Animation : MonoBehaviour
{
    public virtual void PlayMoveAnimation()
    {
        // 기본 이동 애니메이션 재생 로직
        Debug.Log("기본 이동 애니메이션 재생");
    }
    public virtual void PlayAttackAnimation()
    {
        // 기본 공격 애니메이션 재생 로직
        Debug.Log("기본 공격 애니메이션 재생");
    }
    public virtual void PlaySkill1Animation()
    {
        // 스킬1 애니메이션 재생 로직
        Debug.Log("스킬1 애니메이션 재생");
    }
    public virtual void PlaySkill2Animation()
    {
        // 스킬2 애니메이션 재생 로직
        Debug.Log("스킬2 애니메이션 재생");
    }
    public virtual void PlayGuardAnimation()
    {
        // 가드 애니메이션 재생 로직
        Debug.Log("가드 애니메이션 재생");
    }
    public virtual void PlayUltimateAnimation()
    {
        // 궁극기 애니메이션 재생 로직
        Debug.Log("궁극기 애니메이션 재생");
    }
}
