using UnityEngine;
using System.Collections.Generic;

public class Deployment_Controller : MonoBehaviour
{
    public List<Unit> playerUnits;
    public Unit draggingUnit;
    Camera mainCamera;
    LayerMask tileMask;
    Tile currentHoverTile;
    public List<GameObject> previewObjects = new List<GameObject>();
    GameObject previewGO;
    SpriteRenderer previewSR;

    private void Start()
    {
        mainCamera = Camera.main;
        tileMask = LayerMask.GetMask("Tile");
    }
    public void BeginDeployment()
    {
        playerUnits = Battle_Manager.instance.playerUnits;
        foreach (Unit unit in playerUnits)
        {
            CreatePreviewObject(unit);
            previewObjects.Add(previewGO);
        }
        StartDrag(playerUnits[0]);
    }
    private void CreatePreviewObject(Unit unit)
    {
        previewGO = new GameObject("UnitPreview");
        previewSR = previewGO.AddComponent<SpriteRenderer>();
        previewSR.sprite = unit.unitData.unitDraggingSprite;
        previewSR.sortingOrder = 100; // 맨 위에 보이게
        previewSR.color = new Color(1, 1, 1, 0.7f); // 반투명

        previewGO.SetActive(false);
    }
    private void StartDrag(Unit unit)
    {
        draggingUnit = unit;
        currentHoverTile = null;
    }
    public void ReStartDrag(Unit unit)
    {
        if(draggingUnit != null) return;
        draggingUnit = unit;
        currentHoverTile = null;

        // 기존 타일 점유 해제 (다시 배치할 수 있게)
        if (unit.CurrentTile != null)
        {
            unit.CurrentTile.occupant = null;
            unit.CurrentTile = null;
        }
    }
    void Update()
    {
        if (draggingUnit == null) return;

        FollowMouse();

        if (Input.GetMouseButtonDown(0))
        {
            TryPlaceDraggingUnit();
        }
    }
    void FollowMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, tileMask))
        {
            
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile != null)
            {
                Debug.Log($"Hovering over Tile {tile.GridPosition.x}, {tile.GridPosition.y}");
                draggingUnit.transform.position = tile.WorldPosition;
                tile.SetHighlight(true);
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
            TryPlaceUnit(draggingUnit, currentHoverTile);

            ClearHoverTile();
            draggingUnit = null;
        }
    }

    bool TryPlaceUnit(Unit unit, Tile tile)
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

    // x 좌표 기준으로 절반만 배치 가능
    // 예: grid.Width / 2 보다 작은 x만 허용
    public bool CanPlaceOnTile(Tile tile)
    {
        return tile.GridPosition.x < BattleGrid.Width / 2 && !tile.IsOccupied;
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
