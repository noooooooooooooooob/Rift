using UnityEngine;

public class Unit_Move : MonoBehaviour
{
    public Unit unit;
    public float animationDuration = 0.5f;

    private void Awake()
    {
        if (unit == null)
            unit = GetComponent<Unit>();
    }

    public virtual void MoveToTile(Tile targetTile)
    {
        // 타일이 점유되어 있는지 확인
        if (targetTile.IsOccupied)
        {
            Debug.LogWarning("타일이 이미 점유되어 있어 이동할 수 없습니다!");
            return;
        }

        // 현재 타일에서 점유 해제
        if (unit.currentTile != null)
        {
            unit.currentTile.occupant = null;
        }

        // 타일의 점유자로 유닛 설정
        targetTile.occupant = unit.gameObject;

        // 유닛의 현재 타일 업데이트
        unit.currentTile = targetTile;

        // 점프 이동 애니메이션 재생
        if (unit.unitAnimation != null)
        {
            unit.unitAnimation.PlayMoveAnimation(targetTile.WorldPosition);
        }
    }
}
