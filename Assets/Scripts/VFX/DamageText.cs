using UnityEngine;
using TMPro;
using DG.Tweening;

public enum DamageTextType
{
    Normal,     // 일반 대미지 (흰색)
    Critical,   // 크리티컬 (노란색, 크게)
    Heal,       // 힐 (녹색)
    Miss        // 회피 (회색)
}

/// <summary>
/// 대미지 텍스트 컴포넌트
/// VFX_Manager에서 풀링되어 사용됨
/// </summary>
public class DamageText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Transform canvasTransform; // Canvas Transform (흔들림용)

    [Header("Animation Settings")]
    [SerializeField] private float floatDistance = 0.8f;
    [SerializeField] private float duration = 0.8f;
    [SerializeField] private float horizontalRange = 0.3f; // 좌우 랜덤 범위
    [SerializeField] private float shakeStrength = 0.15f;  // 흔들림 강도
    [SerializeField] private float shakeDuration = 0.3f;   // 흔들림 지속시간

    // 타입별 색상
    private static readonly Color NormalColor = Color.white;
    private static readonly Color CriticalColor = new Color(1f, 0.8f, 0f);
    private static readonly Color HealColor = new Color(0.3f, 1f, 0.3f);
    private static readonly Color MissColor = new Color(0.6f, 0.6f, 0.6f);

    private VFX_Manager manager;
    private Sequence currentSequence;

    public void Initialize(VFX_Manager manager)
    {
        this.manager = manager;
    }

    public void Play(int value, DamageTextType type)
    {
        // 이전 애니메이션 정리
        currentSequence?.Kill();

        // 초기 상태 설정
        canvasGroup.alpha = 1f;
        transform.localScale = Vector3.one;

        // 타입별 설정
        switch (type)
        {
            case DamageTextType.Normal:
                text.text = value.ToString();
                text.color = NormalColor;
                PlayNormalAnimation();
                break;

            case DamageTextType.Critical:
                text.text = value.ToString();
                text.color = CriticalColor;
                PlayCriticalAnimation();
                break;

            case DamageTextType.Heal:
                text.text = "+" + value.ToString();
                text.color = HealColor;
                PlayHealAnimation();
                break;

            case DamageTextType.Miss:
                text.text = "MISS";
                text.color = MissColor;
                PlayMissAnimation();
                break;
        }
    }

    private void PlayNormalAnimation()
    {
        Vector3 startPos = transform.position;
        float randomX = Random.Range(-horizontalRange, horizontalRange);
        Vector3 endPos = startPos + new Vector3(randomX, floatDistance, 0f);

        // Canvas 로컬 위치 초기화
        canvasTransform.localPosition = Vector3.zero;

        currentSequence = DOTween.Sequence();
        // 루트는 이동, Canvas는 흔들림
        currentSequence.Join(transform.DOMove(endPos, duration).SetEase(Ease.OutQuad));
        currentSequence.Join(canvasTransform.DOShakePosition(shakeDuration, shakeStrength, 20, 90, false, true));
        currentSequence.Insert(duration * 0.4f, canvasGroup.DOFade(0f, duration * 0.6f));
        currentSequence.OnComplete(ReturnToPool);
    }

    private void PlayCriticalAnimation()
    {
        Vector3 startPos = transform.position;
        float randomX = Random.Range(-horizontalRange, horizontalRange);
        Vector3 endPos = startPos + new Vector3(randomX, floatDistance * 1.3f, 0f);

        // Canvas 로컬 위치 초기화
        canvasTransform.localPosition = Vector3.zero;

        // 스케일 펀치 효과
        transform.localScale = Vector3.one * 1.5f;
        transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);

        currentSequence = DOTween.Sequence();
        // 루트는 이동, Canvas는 더 강하게 흔들림
        currentSequence.Join(transform.DOMove(endPos, duration).SetEase(Ease.OutQuad));
        currentSequence.Join(canvasTransform.DOShakePosition(shakeDuration, shakeStrength * 1.5f, 25, 90, false, true));
        currentSequence.Insert(duration * 0.5f, canvasGroup.DOFade(0f, duration * 0.5f));
        currentSequence.OnComplete(ReturnToPool);
    }

    private void PlayHealAnimation()
    {
        Vector3 startPos = transform.position;
        float randomX = Random.Range(-horizontalRange, horizontalRange);
        Vector3 endPos = startPos + new Vector3(randomX, floatDistance * 0.8f, 0f);

        // Canvas 로컬 위치 초기화
        canvasTransform.localPosition = Vector3.zero;

        currentSequence = DOTween.Sequence();
        // 루트는 이동, Canvas는 부드럽게 흔들림
        currentSequence.Join(transform.DOMove(endPos, duration * 1.2f).SetEase(Ease.OutSine));
        currentSequence.Join(canvasTransform.DOShakePosition(shakeDuration * 0.7f, shakeStrength * 0.7f, 15, 90, false, true));
        currentSequence.Insert(duration * 0.6f, canvasGroup.DOFade(0f, duration * 0.6f));
        currentSequence.OnComplete(ReturnToPool);
    }

    private void PlayMissAnimation()
    {
        Vector3 startPos = transform.position;
        float randomX = Random.Range(-horizontalRange, horizontalRange);
        Vector3 endPos = startPos + new Vector3(randomX, floatDistance * 0.5f, 0f);

        // Canvas 로컬 위치 초기화
        canvasTransform.localPosition = Vector3.zero;

        currentSequence = DOTween.Sequence();
        // 루트는 이동, Canvas는 흔들림
        currentSequence.Join(transform.DOMove(endPos, duration * 0.8f).SetEase(Ease.OutQuad));
        currentSequence.Join(canvasTransform.DOShakePosition(shakeDuration, shakeStrength, 20, 90, false, true));
        currentSequence.Insert(duration * 0.3f, canvasGroup.DOFade(0f, duration * 0.5f));
        currentSequence.OnComplete(ReturnToPool);
    }

    private void ReturnToPool()
    {
        currentSequence = null;
        manager?.ReturnDamageText(this);
    }

    private void LateUpdate()
    {
        // 빌보드: 항상 카메라를 바라봄
        if (Camera.main != null)
        {
            transform.rotation = Camera.main.transform.rotation;
        }
    }

    private void OnDisable()
    {
        currentSequence?.Kill();
        currentSequence = null;
    }
}
