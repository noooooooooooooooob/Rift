using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 턴 순서 UI의 개별 아이콘 컴포넌트
/// UnitData의 turnGaugeIcon을 표시
/// </summary>
public class TurnIconUI : MonoBehaviour
{
    [Header("UI References")]
    public Image iconImage;

    private Unit linkedUnit;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        // CanvasGroup 컴포넌트 가져오기 또는 추가 (페이드 인 애니메이션용)
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
    }

    /// <summary>
    /// 턴 아이콘 설정
    /// </summary>
    /// <param name="unit">표시할 유닛</param>
    public void Setup(Unit unit)
    {
        if (unit == null || unit.unitData == null)
        {
            Debug.LogWarning("TurnIconUI.Setup: unit or unitData is null");
            return;
        }

        linkedUnit = unit;

        // 턴 게이지 아이콘 설정
        if (unit.unitData.turnGaugeIcon != null)
        {
            iconImage.sprite = unit.unitData.turnGaugeIcon;
        }
        else
        {
            Debug.LogWarning($"TurnIconUI.Setup: {unit.unitData.unitName} has no turnGaugeIcon");
        }
    }

    /// <summary>
    /// 다음 턴 강조 표시 (크기 변경)
    /// </summary>
    /// <param name="isNext">다음 턴인지 여부</param>
    public void SetIsNextTurn(bool isNext)
    {
        if (isNext)
        {
            // 크기 확대 (1.2배)
            transform.localScale = Vector3.one * 1.2f;
        }
        else
        {
            // 기본 크기
            transform.localScale = Vector3.one;
        }
    }

    /// <summary>
    /// 페이드 인 애니메이션
    /// </summary>
    /// <param name="delay">애니메이션 시작 딜레이</param>
    public void AnimateIn(float delay)
    {
        StartCoroutine(FadeIn(delay));
    }

    private IEnumerator FadeIn(float delay)
    {
        if (canvasGroup == null) yield break;

        // 초기 상태: 투명
        canvasGroup.alpha = 0f;

        // 딜레이 대기
        if (delay > 0f)
        {
            yield return new WaitForSeconds(delay);
        }

        // 페이드 인
        float elapsed = 0f;
        float duration = 0.3f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / duration);
            yield return null;
        }

        // 최종 상태: 완전 불투명
        canvasGroup.alpha = 1f;
    }
}
