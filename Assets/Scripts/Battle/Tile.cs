using System;
using UnityEngine;

public enum HighlightType
{
    None,
    Movable,
    Attackable,
    Deployment
}

/// <summary>
/// 전투 그리드의 개별 타일 컴포넌트
/// 그리드 위치, 점유 상태, 하이라이트 기능 제공
/// </summary>
public class Tile : MonoBehaviour
{
    SpriteRenderer spriteRenderer;  // 타일 스프라이트 렌더러
    [SerializeField] private Vector2Int gridPosition;  // 그리드 상의 좌표 (0~9, 0~4)

    /// <summary>
    /// 그리드 좌표 프로퍼티
    /// </summary>
    public Vector2Int GridPosition
    {
        get { return gridPosition; }
        set { gridPosition = value; }
    }

    /// <summary>
    /// 현재 타일을 점유하고 있는 오브젝트 (Unit 또는 UnitPreview)
    /// </summary>
    public GameObject occupant;

    /// <summary>
    /// 타일이 점유되어 있는지 확인
    /// </summary>
    public bool IsOccupied
    {
        get { return occupant != null; }
    }

    /// <summary>
    /// 타일이 점유되어 있는지 확인
    /// </summary>
    public bool isPlayerTile = false;

    /// <summary>
    /// 타일의 월드 좌표
    /// </summary>
    public Vector3 WorldPosition
    {
        get { return transform.position; }
    }

    /// <summary>
    /// 초기화: SpriteRenderer 컴포넌트 캐싱
    /// </summary>
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 타일 하이라이트 설정/해제
    /// </summary>
    public void SetHighlight(HighlightType type)
    {
        switch (type)
        {
            case HighlightType.None:
                spriteRenderer.color = Color.white;
                break;
            case HighlightType.Movable:
                spriteRenderer.color = new Color(0.3f, 0.5f, 1f, 1f);  // 파란색
                break;
            case HighlightType.Attackable:
                spriteRenderer.color = new Color(1f, 0.3f, 0.3f, 1f);  // 빨간색
                break;
            case HighlightType.Deployment:
                spriteRenderer.color = Color.yellow;
                break;
        }
    }
}
