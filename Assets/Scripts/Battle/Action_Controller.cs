using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;

public enum ActionType
{
    Move,
    Attack,
    Skill1,
    Skill2,
    Ultimate,
    Guard
}

/// <summary>
/// 유닛의 행동 컨트롤러
/// </summary>
public class Action_Controller : MonoBehaviour
{
    public Unit currentUnit; // 컨트롤하는 유닛
    public Action_Menu_UI actionMenuUI; // 액션 메뉴 UI 참조
    private Tile hoveredTile;
    public List<Tile> availableTiles = new List<Tile>();
    public List<Tile> highlightedTiles = new List<Tile>(); 
    private Camera mainCamera;
    private LayerMask tileLayerMask;
    public Turn_Manager turnManager;
    private ActionType currentMode = ActionType.Move;

    void Awake()
    {
        mainCamera = Camera.main;
        tileLayerMask = LayerMask.GetMask("Tile");
    }

    void Update()
    {
        if (currentUnit == null)
            return;

        Tile currentHoveredTile = GetTileUnderMouse();

        // 호버 하이라이트 처리
        if (hoveredTile != currentHoveredTile)
        {
            // 이전 타일 하이라이트 복원
            if (hoveredTile != null)
            {
                RestoreTileHighlight(hoveredTile);
            }

            hoveredTile = currentHoveredTile;

            // 새 타일 하이라이트
            if (hoveredTile != null && highlightedTiles.Contains(hoveredTile))
            {
                hoveredTile.SetHighlight(HighlightType.Deployment);
            }
        }

        // 마우스 클릭 감지 (새 Input System)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            Tile clickedTile = GetTileUnderMouse();
            if (clickedTile != null)
            {
                Debug.Log($"[Update] Tile clicked at {clickedTile.GridPosition}");
                HandleTileClick(clickedTile);
            }
        }
    }

    private Tile GetTileUnderMouse()
    {
        if (Mouse.current == null) return null;

        Vector2 screenPos = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, tileLayerMask))
        {
            return hit.collider.GetComponent<Tile>();
        }

        return null;
    }

    public void TurnStart(Unit unit)
    {
        currentUnit = unit;
        actionMenuUI = currentUnit.actionMenuUI;

        actionMenuUI.moveButton.onClick.AddListener(OnMoveButtonClick);
        actionMenuUI.attackButton.onClick.AddListener(OnAttackButtonClick);
        actionMenuUI.skillButton.onClick.AddListener(OnSkillButtonClick);
        actionMenuUI.guardButton.onClick.AddListener(OnGuardButtonClick);

        actionMenuUI.skill1Button.onClick.AddListener(OnSkill1ButtonClick);
        actionMenuUI.skill2Button.onClick.AddListener(OnSkill2ButtonClick);

        // actionMenuUI.ultimateButton.onClick.AddListener(OnUltimateButtonClick);
        actionMenuUI.skillReturnButton.onClick.AddListener(OnCancelButtonClick);

        actionMenuUI.returnButton.onClick.AddListener(OnCancelButtonClick);
        currentUnit.actionMenuUI.ShowActionMenu();
    }
    private void EndTurn()
    {
        currentUnit = null;

        // Turn_Manager에게 턴 종료 신호
        if (turnManager != null)
        {
            turnManager.isTurnEnd = true;
        }
    }
    private void ShowMovableTiles()
    {
        ClearHighlights();
        List<Tile> tiles = Method.GetTilesSoloTarget(currentUnit, currentUnit.stats.AGI);
        foreach (var tile in tiles)
        {
            tile.SetHighlight(HighlightType.Movable);
            highlightedTiles.Add(tile);
        }
    }
    private void ShowAttackableTiles()
    {
        ClearHighlights();
        List<Tile> tiles = Method.GetTilesSoloTarget(currentUnit, currentUnit.attackAction.attackSkillData.range, true); // 예시: 공격 범위는 이동 범위 + 1
        foreach (var tile in tiles)
        {
            tile.SetHighlight(HighlightType.Attackable);
            highlightedTiles.Add(tile);
        }
    }
    private void HandleTileClick(Tile tile)
    {
        Debug.Log($"[HandleTileClick] Tile clicked: {tile?.GridPosition}, currentMode: {currentMode}");

        switch (currentMode)
        {
            case ActionType.Move:
                StartCoroutine(HandleMoveTo(tile));
                break;
            case ActionType.Attack:
                StartCoroutine(HandleAttack(tile));
                break;
            case ActionType.Skill1:
                StartCoroutine(HandleSkill1(tile));
                break;
            case ActionType.Skill2:
                StartCoroutine(HandleSkill2(tile));
                break;
            case ActionType.Guard:
                StartCoroutine(HandleGuard(tile));
                break;
            case ActionType.Ultimate:
                StartCoroutine(HandleUltimate(tile));
                break;
        }
    }
    private IEnumerator HandleMoveTo(Tile tile)
    {
        if (tile == null)
        {
            Debug.LogWarning("[HandleMoveTo] Tile is null!");
            yield break;
        }

        if (!highlightedTiles.Contains(tile))
        {
            Debug.LogWarning($"[HandleMoveTo] Tile {tile.GridPosition} not in highlightedTiles!");
            yield break;
        }

        if (currentUnit.moveAction == null)
        {
            Debug.LogError("[HandleMoveTo] currentUnit.moveAction is NULL! Check Inspector.");
            yield break;
        }
        ClearHighlights();
        actionMenuUI.HideAllMenus();

        currentUnit.moveAction.MoveToTile(tile);
        yield return new WaitForSeconds(currentUnit.moveAction.animationDuration);
        turnManager.isTurnEnd = true;
    }
    private IEnumerator HandleAttack(Tile tile)
    {
        if (tile == null || !highlightedTiles.Contains(tile))
            yield break;

        ClearHighlights();
        actionMenuUI.HideAllMenus();

        currentUnit.attackAction.ExecuteAttack(tile.occupant.GetComponent<Unit>());
        yield return new WaitForSeconds(currentUnit.attackAction.animationDuration);
        turnManager.isTurnEnd = true;
    }
    private IEnumerator HandleSkill1(Tile tile)
    {
        if (tile == null || !highlightedTiles.Contains(tile))
            yield break;

        ClearHighlights();
        actionMenuUI.HideReturnMenu();
        actionMenuUI.ShowActionMenu();
        yield return null;
    }
    private IEnumerator HandleSkill2(Tile tile)
    {
        if (tile == null || !highlightedTiles.Contains(tile))
            yield break;

        ClearHighlights();
        actionMenuUI.HideReturnMenu();
        actionMenuUI.ShowActionMenu();
        yield return null;
    }
    private IEnumerator HandleGuard(Tile tile)
    {
        ClearHighlights();
        actionMenuUI.HideReturnMenu();
        actionMenuUI.ShowActionMenu();
        yield return null;
    }
    private IEnumerator HandleUltimate(Tile tile)
    {
        if (tile == null || !highlightedTiles.Contains(tile))
            yield break;

        ClearHighlights();
        actionMenuUI.HideReturnMenu();
        actionMenuUI.ShowActionMenu();
        yield return null;
    }

    private void ClearHighlights()
    {
        foreach (var tile in availableTiles)
        {
            tile.SetHighlight(HighlightType.None);
        }
        foreach (var tile in highlightedTiles)
        {
            tile.SetHighlight(HighlightType.None);
        }
        availableTiles.Clear();
        highlightedTiles.Clear();
    }
    /// <summary>
    /// 타일의 원래 하이라이트 복원
    /// </summary>
    private void RestoreTileHighlight(Tile tile)
    {
        if (!highlightedTiles.Contains(tile))
        {
            tile.SetHighlight(HighlightType.None);
            return;
        }

        // currentMode에 따라 원래 색상 복원
        switch (currentMode)
        {
            case ActionType.Move:
                tile.SetHighlight(HighlightType.Movable);
                break;
            case ActionType.Attack:
            case ActionType.Skill1:
            case ActionType.Skill2:
            case ActionType.Ultimate:
                tile.SetHighlight(HighlightType.Attackable);
                break;
        }
    }
    // 이동 버튼
    public void OnMoveButtonClick()
    {
        if (currentUnit == null) return;

        Debug.Log("Move button clicked");

        currentMode = ActionType.Move;  // 이동 모드 설정
        actionMenuUI.HideActionMenu();
        actionMenuUI.ShowReturnMenu();

        ShowMovableTiles();
    }

    // 공격 버튼
    public void OnAttackButtonClick()
    {
        if (currentUnit == null) return;

        currentMode = ActionType.Attack;  // 공격 모드 설정
        actionMenuUI.HideActionMenu();
        actionMenuUI.ShowReturnMenu();

        ShowAttackableTiles();
    }

    // 스킬 메뉴 버튼
    public void OnSkillButtonClick()
    {
        actionMenuUI.HideActionMenu();
        actionMenuUI.ShowSkillMenu();
    }

    // 스킬1 버튼
    public void OnSkill1ButtonClick()
    {
        if (currentUnit == null) return;

        currentMode = ActionType.Skill1;  // 스킬1 모드 설정
        actionMenuUI.HideSkillMenu();
        actionMenuUI.ShowReturnMenu();

        ShowAttackableTiles();  // 스킬도 공격 범위와 동일하게
    }

    // 스킬2 버튼
    public void OnSkill2ButtonClick()
    {
        if (currentUnit == null) return;

        currentMode = ActionType.Skill2;  // 스킬2 모드 설정
        actionMenuUI.HideSkillMenu();
        actionMenuUI.ShowReturnMenu();

        ShowAttackableTiles();
    }

    // 궁극기 버튼
    public void OnUltimateButtonClick()
    {
        if (currentUnit == null) return;

        // UP 체크
        if (currentUnit.currentUP < currentUnit.stats.UP)
        {
            Debug.Log("궁극기 포인트 부족!");
            return;
        }

        currentMode = ActionType.Ultimate;  // 궁극기 모드 설정
        actionMenuUI.HideSkillMenu();
        actionMenuUI.ShowReturnMenu();

        ShowAttackableTiles();
    }

    // 방어 버튼 (즉시 실행)
    public void OnGuardButtonClick()
    {
        if (currentUnit == null) return;

        currentMode = ActionType.Guard;  // 방어 모드 설정
        actionMenuUI.HideActionMenu();
        // StartCoroutine(ExecuteGuard());
    }

    // 취소 버튼
    public void OnCancelButtonClick()
    {
        ClearHighlights();

        actionMenuUI.HideReturnMenu();
        actionMenuUI.HideSkillMenu();
        actionMenuUI.ShowActionMenu();
    }
}
