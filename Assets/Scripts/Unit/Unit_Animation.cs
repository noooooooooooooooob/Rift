using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;

public class Unit_Animation : MonoBehaviour
{
    protected Animator animator;
    [Header("Move Animation Settings")]
    public float moveDuration = 1f;
    public float jumpPower = 1f;
    public float hangTime = 0.02f; // 중간 체공 시간
    public bool isMoving;
    [Header("Attack Animation Settings")]
    public bool isAttacking = false;
    public Vector3 attackPositionOffset = new Vector3(-3.27f, 0.75f, 0f);
    protected Vector3 originalPosition;
    protected bool isAttackAnimationDone;
    [Header("Ultimate Animation Settings")]
    public Vector3 ultimatePosition = new Vector3(0f, 0f, 0f);
    public Vector3 ultimateCameraPosition = new Vector3(0f, 0f, 0f);
    public CinemachineCamera ultimateCam;
    [SerializeField] protected PlayableDirector ultimateDirector;

    [Header("Camera Shake")]
    [SerializeField] private CinemachineImpulseSource impulseSource;

    [Header("UI")]
    [SerializeField] private RectTransform uiContainer;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    protected void SetBeforeAnimation()
    {
        Battle_UI_Manager.instance.IsClickInteractable = false;
    }
    protected void SetAfterAnimation()
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

        Audio_Manager.Instance.PlaySound("Foot Move");
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
            Audio_Manager.Instance.PlaySound("Foot Move");
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
    public virtual IEnumerator PlaySkill1Animation(Vector3 targetPosition, Vector3 lastPosition)
    {
        isAttacking = true;
        isAttackAnimationDone = false;
        animator.SetTrigger("Skill 1");
        isAttackAnimationDone = true;
        yield return new WaitUntil(() => isAttackAnimationDone);
        isAttacking = false;
        yield break;
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

    /// <summary>
    /// Timeline 기반 궁극기 애니메이션 재생
    /// </summary>
    public virtual IEnumerator PlayUltimateTimeline()
    {
        if (ultimateDirector == null)
        {
            Debug.LogWarning("UltimateDirector가 할당되지 않았습니다. 기본 애니메이션으로 재생합니다.");
            yield return PlayUltimateAnimation();
            yield break;
        }

        SetBeforeAnimation();
        isAttacking = true;
        isAttackAnimationDone = false;
        originalPosition = transform.position;

        PlayMoveAnimation(ultimatePosition);
        yield return new WaitUntil(() => !isMoving);

        // 카메라 분리 (부모에서 빠져나옴)
        Transform originalCamParent = ultimateCam.transform.parent;
        Vector3 camPos = ultimateCam.transform.position;
        Quaternion camRot = ultimateCam.transform.rotation;
        ultimateCam.transform.SetParent(null);
        ultimateCam.transform.SetPositionAndRotation(camPos, camRot);
        ultimateCam.Priority = 30;

        // Timeline 재생
        ultimateDirector.Play();

        // Timeline 종료 대기
        yield return new WaitUntil(() => ultimateDirector.state != PlayState.Playing);

        // Animator 리셋 (Timeline이 제어권 반환)
        animator.Rebind();
        animator.SetTrigger("Idle");

        // 카메라 복귀
        ultimateCam.Priority = 0;
        ultimateCam.transform.SetParent(originalCamParent);

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
    public void PlayVFX(string vfxName, Vector3 position)
    {
        VFX_Manager.Instance.Play(vfxName, position);
    }
    public void PlaySound(string soundName)
    {
        Audio_Manager.Instance.PlaySound(soundName);
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
        Debug.Log("카메라 쉐이크!");
        impulseSource.ImpulseDefinition.TimeEnvelope.DecayTime = duration;
        impulseSource.GenerateImpulse(new Vector3(0.2f, 0.2f, 0f));
    }
}
