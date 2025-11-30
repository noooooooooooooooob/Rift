using UnityEngine;
using System.Collections.Generic;

public class Deployment_Controller : MonoBehaviour
{
    public List<Unit> playerUnits;

    // x 좌표 기준으로 절반만 배치 가능
    // 예: grid.Width / 2 보다 작은 x만 허용
    public bool CanPlaceOnTile(Tile tile)
    {
        return tile.GridPosition.x < BattleGrid.Width / 2 && !tile.IsOccupied;
    }

    public void BeginDeployment()
    {
    }

    public bool TryPlaceUnit(Unit unit, Tile tile)
    {
        if (!CanPlaceOnTile(tile)) return false;

        // 이미 배치된 타일에 있으면 비우기
        if (unit.CurrentTile != null)
        {
            unit.CurrentTile.occupant = null;
        }

        unit.transform.position = tile.transform.position; // 타일 중앙으로 이동
        unit.CurrentTile = tile;
        tile.occupant = unit;
        return true;
    }

    public bool IsDeploymentValid()
    {
        // 모든 유닛이 타일에 배치되었는지 확인
        foreach (Unit u in playerUnits)
        {
            if (u.CurrentTile == null) return false;
        }
        return true;
    }
}
