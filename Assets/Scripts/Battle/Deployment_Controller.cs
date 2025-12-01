using UnityEngine;
using System.Collections.Generic;

public class Deployment_Controller : MonoBehaviour
{
    public List<Unit> playerUnits;
    Unit draggingUnit;
    Camera mainCamera;
    LayerMask tileMask;
    Tile currentHoverTile;

    private void Start()
    {
        mainCamera = Camera.main;
        tileMask = LayerMask.GetMask("Tile");
    }
    // x 좌표 기준으로 절반만 배치 가능
    // 예: grid.Width / 2 보다 작은 x만 허용
    public bool CanPlaceOnTile(Tile tile)
    {
        return tile.GridPosition.x < BattleGrid.Width / 2 && !tile.IsOccupied;
    }

    public void BeginDeployment()
    {
        playerUnits = Battle_Manager.instance.playerUnits;

    }
    private void Update()
    {
        if (draggingUnit == null) return;

        FollowMouse();

        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceDraggingUnit();
        }
    }

    private void FollowMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, tileMask))
        {
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile != null)
            {
                // 유닛을 타일 위치 위로 살짝 띄워서 위치
                draggingUnit.transform.position = tile.transform.position;
                UpdateHoverTile(tile);
                return;
            }
        }

        // 타일이 아닌 곳이면, 그냥 바닥 쪽으로만 이동 시키거나 그대로 두기
        ClearHoverTile();
    }

    private void UpdateHoverTile(Tile tile)
    {
        if (currentHoverTile == tile) return;

        // 이전 타일 하이라이트 해제
        if (currentHoverTile != null)
        {
            currentHoverTile.SetHighlight(false);
        }

        currentHoverTile = tile;
        currentHoverTile.SetHighlight(true);
    }

    private void ClearHoverTile()
    {
        if (currentHoverTile != null)
        {
            currentHoverTile.SetHighlight(false);
            currentHoverTile = null;
        }
    }

    private void TryPlaceDraggingUnit()
    {
        if (currentHoverTile != null && CanPlaceOnTile(currentHoverTile))
        {
            // 실제 배치
            draggingUnit.transform.position = currentHoverTile.transform.position;
            draggingUnit.CurrentTile = currentHoverTile;
            currentHoverTile.occupant = draggingUnit;

            ClearHoverTile();
            draggingUnit = null;
        }
        else
        {
            // 배치 불가능한 위치 클릭했을 때 처리
            // 1) 그냥 계속 들고 있게 두거나
            // 2) 원래 자리로 되돌리거나 (원래 위치 저장해뒀다가)
            // 지금은 일단 계속 들고 있게 놔두자
        }
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
