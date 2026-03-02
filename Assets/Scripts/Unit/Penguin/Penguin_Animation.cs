using UnityEngine;
using System.Collections;
using DG.Tweening;

public class Penguin_Animation : Unit_Animation
{
    public float skill1MoveDuration = 0.5f;
    public void Skill1_Move(Vector3 targetPosition)
    {
        transform.DOMove(targetPosition + Vector3.left * 1.0f, skill1MoveDuration)
        .SetEase(Ease.InOutExpo);
    }
    public override IEnumerator PlaySkill1Animation(Vector3 targetPosition, Vector3 lastPosition)
    {
        SetBeforeAnimation();

        isAttacking = true;
        isAttackAnimationDone = false;
        originalPosition = transform.position;

        // 공격 위치로 이동
        PlayMoveAnimation(targetPosition + attackPositionOffset);
        yield return new WaitUntil(() => !isMoving);

        // 공격 애니메이션
        animator.SetTrigger("Skill 1");
        Skill1_Move(lastPosition);
        yield return new WaitUntil(() => isAttackAnimationDone);

        // 원래 위치로 복귀
        PlayMoveAnimation(originalPosition);
        yield return new WaitUntil(() => !isMoving);
        SetAfterAnimation();

        isAttacking = false;
        yield break;
    }
}
