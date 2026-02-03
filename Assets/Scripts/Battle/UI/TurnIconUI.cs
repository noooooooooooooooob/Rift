using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 턴 순서 UI의 개별 아이콘 컴포넌트
/// UnitData의 turnGaugeIcon을 표시하고 게이지에 따라 위치를 업데이트
/// </summary>
public class TurnIconUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;       // 아이콘 이미지
    [SerializeField] private Image borderImage;     // 테두리 이미지 (색상 구분용)

    private Unit linkedUnit;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        // 자동으로 Image 컴포넌트 찾기 (할당 안 됐을 경우)
        if (iconImage == null)
        {
            iconImage = GetComponent<Image>();
        }
    }

    /// <summary>
    /// 아이콘 초기화
    /// 유닛 연결 및 아이콘/색상 설정
    /// </summary>
    /// <param name="unit">연결할 유닛</param>
    /// <param name="borderColor">테두리 색상 (플레이어/적 구분)</param>
    public void Initialize(Unit unit, Color borderColor)
    {
        linkedUnit = unit;

        // 아이콘 이미지 설정
        if (iconImage != null && unit.unitData?.turnGaugeIcon != null)
        {
            iconImage.sprite = unit.unitData.turnGaugeIcon;
        }

        // 테두리 색상 설정
        if (borderImage != null)
        {
            borderImage.color = borderColor;
        }
    }

    /// <summary>
    /// 게이지 값에 따른 위치 업데이트
    /// 오른쪽(게이지 0) → 왼쪽(게이지 100) 이동
    /// </summary>
    /// <param name="normalizedGauge">정규화된 게이지 값 (0.0 ~ 1.0)</param>
    /// <param name="barWidth">게이지 바의 너비</param>
    public void UpdatePosition(float normalizedGauge, float barWidth)
    {
        if (rectTransform == null)
        {
            rectTransform = GetComponent<RectTransform>();
            if (rectTransform == null) return;
        }

        // 오른쪽(게이지 0)에서 왼쪽(게이지 100)으로 이동
        // xPos = barWidth * (1 - normalizedGauge) - (barWidth / 2)
        // normalizedGauge = 0 → xPos = barWidth/2 (오른쪽 끝)
        // normalizedGauge = 1 → xPos = -barWidth/2 (왼쪽 끝)
        float xPos = barWidth * (1f - normalizedGauge) - (barWidth / 2f);

        rectTransform.anchoredPosition = new Vector2(xPos, rectTransform.anchoredPosition.y);
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
    /// 연결된 유닛 반환
    /// </summary>
    public Unit GetLinkedUnit()
    {
        return linkedUnit;
    }
}
