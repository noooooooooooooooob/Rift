using UnityEngine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;

public class Unit_Animation : MonoBehaviour
{
    Animator animator;
    [Header("Move Animation Settings")]
    public float moveDuration = 1f;
    public float jumpPower = 1f;
    public float hangTime = 0.02f; // 중간 체공 시간
    public bool isMoving { get; private set; }
    [Header("Attack Animation Settings")]
    public bool isAttacking { get; private set; }
    public Vector3 attackPositionOffset = new Vector3(-3.27f, 0.75f, 0f);
    private Vector3 originalPosition;
    private bool isAttackAnimationDone;
    [Header("Ultimate Animation Settings")]
    public Vector3 ultimatePosition = new Vector3(0f, 0f, 0f);

    [Header("Camera Shake")]
    [SerializeField] private CinemachineImpulseSource impulseSource;

    [Header("UI")]
    [SerializeField] private RectTransform uiContainer;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    void SetBeforeAnimation()
    {
        Battle_UI_Manager.instance.IsClickInteractable = false;
    }
    void SetAfterAnimation()
    {
        Battle_UI_Manager.instance.IsClickInteractable = true;
    }
    public virtual void PlayMoveAnimation(Vector3 targetPosition)
    {
        SetBeforeAnimation();

        isMoving = true;
        animator.SetTrigger("Move");

        Vector3 startPos = transform.position;
        Vector3 midPos = (startPos + targetPosition) / 2f + Vector3.up * jumpPower;

        // 착지 지점 (목표 약간 전)
        Vector3 landPos = Vector3.Lerp(startPos, targetPosition, 0.7f);

        Sequence seq = DOTween.Sequence();
        // 시작 -> 중간 (빠르게 올라감)
        seq.Append(transform.DOMove(midPos, moveDuration * 0.25f).SetEase(Ease.OutExpo));
        // 중간에서 체공
        seq.AppendInterval(hangTime);
        // 중간 -> 착지 (빠르게 내려감)
        seq.Append(transform.DOMove(landPos, moveDuration * 0.25f).SetEase(Ease.InExpo));
        // 착지 시 Idle 트리거
        seq.AppendCallback(() => {
            animator.SetTrigger("Idle");
            });
        // 착지 -> 목표 (치지직 끌려감)
        seq.Append(transform.DOMove(targetPosition, moveDuration * 0.5f).SetEase(Ease.OutCirc));
        seq.OnComplete(() => {
            isMoving = false;
            SetAfterAnimation();
        });
    }
    public virtual IEnumerator PlayAttackAnimation(Transform target)
    {
        SetBeforeAnimation();

        isAttacking = true;
        isAttackAnimationDone = false;
        originalPosition = transform.position;

        // 공격 위치로 이동
        PlayMoveAnimation(target.position + attackPositionOffset);
        yield return new WaitUntil(() => !isMoving);

        // 공격 애니메이션
        animator.SetTrigger("Attack");
        yield return new WaitUntil(() => isAttackAnimationDone);

        // 원래 위치로 복귀
        PlayMoveAnimation(originalPosition);
        yield return new WaitUntil(() => !isMoving);
        SetAfterAnimation();

        isAttacking = false;
    }
    public void EndAttackAnimation()
    {
        isAttackAnimationDone = true;
    }
    public void PlayHitAnimation(float duration = 0.2f, float strength = 0.3f)
    {
        animator.SetTrigger("Hit");
        transform.DOShakePosition(duration, new Vector3(strength, strength, 0f), 20, 90, false, true);

        if (uiContainer != null)
            uiContainer.DOShakeAnchorPos(duration, new Vector2(strength * 50f, strength * 50f), 20, 90, false, true);
    }
    public virtual void PlaySkill1Animation()
    {
        isAttacking = true;
        // 스킬1 애니메이션 재생 로직
        Debug.Log("스킬1 애니메이션 재생");
        isAttacking = false;
    }
    public virtual IEnumerator PlaySkill2Animation(Transform target)
    {
        SetBeforeAnimation();

        isAttacking = true;
        isAttackAnimationDone = false;
        originalPosition = transform.position;

        // 공격 위치로 이동
        PlayMoveAnimation(target.position + attackPositionOffset);
        yield return new WaitUntil(() => !isMoving);

        // 공격 애니메이션
        animator.SetTrigger("Skill 2");
        yield return new WaitUntil(() => isAttackAnimationDone);

        // 원래 위치로 복귀
        PlayMoveAnimation(originalPosition);
        yield return new WaitUntil(() => !isMoving);
        SetAfterAnimation();

        isAttacking = false;
    }
    public virtual void PlayGuardAnimation()
    {
        // 가드 애니메이션 재생 로직
        Debug.Log("가드 애니메이션 재생");
    }
    public virtual IEnumerator PlayUltimateAnimation()
    {
        SetBeforeAnimation();

        isAttacking = true;
        isAttackAnimationDone = false;
        originalPosition = transform.position;

        // 공격 위치로 이동
        PlayMoveAnimation(ultimatePosition);
        yield return new WaitUntil(() => !isMoving);

        // 공격 애니메이션
        animator.SetTrigger("Ultimate");
        yield return new WaitUntil(() => isAttackAnimationDone);

        // 원래 위치로 복귀
        PlayMoveAnimation(originalPosition);
        yield return new WaitUntil(() => !isMoving);
        SetAfterAnimation();

        isAttacking = false;
    }
    public void PlayVFX(string vfxName)
    {
        VFX_Manager.Instance.Play(vfxName, transform.position);
    }
    public void PlaySlowMotion(float duration)
    {
        StartCoroutine(SlowMotionCoroutine(duration));
    }

    private IEnumerator SlowMotionCoroutine(float duration)
    {
        Time.timeScale = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            float easedT = t * t * t; // InCubic: 처음엔 천천히, 후반에 빠르게
            Time.timeScale = Mathf.Lerp(0.2f, 1f, easedT);
            yield return null;
        }

        Time.timeScale = 1f;
    }
    public void ShakeCamera(float duration = 0.2f)
    {
        impulseSource.ImpulseDefinition.TimeEnvelope.DecayTime = duration;
        impulseSource.GenerateImpulse(new Vector3(0.2f, 0.2f, 0f));
    }
}
