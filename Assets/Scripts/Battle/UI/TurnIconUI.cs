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
    private Unit linkedUnit;

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
}
