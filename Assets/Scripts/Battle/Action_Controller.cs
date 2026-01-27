using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.InputSystem;

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

    // Input System
    [Header("Input")]
    public InputActionAsset inputActions;
    private InputAction actionUp;
    private InputAction actionDown;
    private InputAction actionEnter;
    private InputAction actionCancel;

    void Awake()
    {
        mainCamera = Camera.main;
        tileLayerMask = LayerMask.GetMask("Tile");

        // Input Actions 찾기
        if (inputActions != null)
        {
            actionUp = inputActions.FindAction("Action Up");
            actionDown = inputActions.FindAction("Action Down");
            actionEnter = inputActions.FindAction("Action Enter");
            actionCancel = inputActions.FindAction("Action Cancel");
        }
    }

    void OnEnable()
    {
        if (inputActions != null)
        {
            inputActions.Enable();

            // 액션 콜백 등록
            if (actionUp != null) actionUp.performed += OnActionUp;
            if (actionDown != null) actionDown.performed += OnActionDown;
            if (actionEnter != null) actionEnter.performed += OnActionEnter;
            if (actionCancel != null) actionCancel.performed += OnActionCancel;
        }
    }

    void OnDisable()
    {
        if (inputActions != null)
        {
            // 액션 콜백 해제
            if (actionUp != null) actionUp.performed -= OnActionUp;
            if (actionDown != null) actionDown.performed -= OnActionDown;
            if (actionEnter != null) actionEnter.performed -= OnActionEnter;
            if (actionCancel != null) actionCancel.performed -= OnActionCancel;

            inputActions.Disable();
        }
    }

    private void OnActionUp(InputAction.CallbackContext context)
    {
        if (currentUnit == null || actionMenuUI == null) return;
        if (actionMenuUI.IsMenuActive())
        {
            actionMenuUI.SelectPrevious();
        }
    }

    private void OnActionDown(InputAction.CallbackContext context)
    {
        if (currentUnit == null || actionMenuUI == null) return;
        if (actionMenuUI.IsMenuActive())
        {
            actionMenuUI.SelectNext();
        }
    }

    private void OnActionEnter(InputAction.CallbackContext context)
    {
        if (currentUnit == null || actionMenuUI == null) return;
        if (actionMenuUI.IsMenuActive())
        {
            OnConfirmAction();
        }
    }

    private void OnActionCancel(InputAction.CallbackContext context)
    {
        if (currentUnit == null || actionMenuUI == null) return;
        OnCancelAction();
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

        // 마우스 좌클릭 감지 (타일 클릭용)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            // UI 위에 있으면 무시
            if (EventSystem.current.IsPointerOverGameObject())
                return;

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

        // 기존 리스너 제거 후 새로 등록 (중복 방지)
        ClearButtonListeners();

        // 네비게이션 버튼 리스너 등록
        if (actionMenuUI.upButton != null)
            actionMenuUI.upButton.onClick.AddListener(OnUpButton);
        if (actionMenuUI.downButton != null)
            actionMenuUI.downButton.onClick.AddListener(OnDownButton);
        if (actionMenuUI.confirmButton != null)
            actionMenuUI.confirmButton.onClick.AddListener(OnConfirmAction);
        if (actionMenuUI.cancelButton != null)
            actionMenuUI.cancelButton.onClick.AddListener(OnCancelAction);

        // Return 패널의 버튼 리스너
        if (actionMenuUI.returnButton != null)
            actionMenuUI.returnButton.onClick.AddListener(OnCancelAction);

        currentUnit.actionMenuUI.ShowActionMenu();
    }

    private void ClearButtonListeners()
    {
        if (actionMenuUI == null) return;

        if (actionMenuUI.upButton != null)
            actionMenuUI.upButton.onClick.RemoveListener(OnUpButton);
        if (actionMenuUI.downButton != null)
            actionMenuUI.downButton.onClick.RemoveListener(OnDownButton);
        if (actionMenuUI.confirmButton != null)
            actionMenuUI.confirmButton.onClick.RemoveListener(OnConfirmAction);
        if (actionMenuUI.cancelButton != null)
            actionMenuUI.cancelButton.onClick.RemoveListener(OnCancelAction);
        if (actionMenuUI.returnButton != null)
            actionMenuUI.returnButton.onClick.RemoveListener(OnCancelAction);
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
        List<Tile> tiles = Method.GetMovableTiles(currentUnit, currentUnit.stats.AGI);
        foreach (var tile in tiles)
        {
            tile.SetHighlight(HighlightType.Movable);
            highlightedTiles.Add(tile);
        }
    }

    private void ShowAttackableTiles()
    {
        ClearHighlights();
        foreach (var tile in Battle_Manager.instance.enemyTiles)
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

        List<Tile> attackableTiles = Method.GetAttackableTiles(currentUnit, currentUnit.attackAction.attackSkillData, tile);

        currentUnit.attackAction.ExecuteAttack(attackableTiles);
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
                tile.SetHighlight(HighlightType.Attackable);
                break;
            case ActionType.Skill1:
            case ActionType.Skill2:
            case ActionType.Ultimate:
                tile.SetHighlight(HighlightType.Attackable);
                break;
        }
    }

    // 위 버튼 클릭
    public void OnUpButton()
    {
        if (actionMenuUI != null)
        {
            actionMenuUI.SelectPrevious();
        }
    }

    // 아래 버튼 클릭
    public void OnDownButton()
    {
        if (actionMenuUI != null)
        {
            actionMenuUI.SelectNext();
        }
    }

    // 확인 버튼 - 현재 선택된 액션 실행
    public void OnConfirmAction()
    {
        if (currentUnit == null || actionMenuUI == null) return;

        ActionType selectedAction = actionMenuUI.GetCurrentActionType();
        Debug.Log($"Confirm action: {selectedAction}");

        currentMode = selectedAction;
        actionMenuUI.HideActionMenu();
        actionMenuUI.ShowReturnMenu();

        switch (selectedAction)
        {
            case ActionType.Move:
                ShowMovableTiles();
                break;
            case ActionType.Attack:
                ShowAttackableTiles();
                break;
            case ActionType.Skill1:
                ShowAttackableTiles();
                break;
            case ActionType.Skill2:
                ShowAttackableTiles();
                break;
            case ActionType.Guard:
                // 방어는 즉시 실행 (타일 선택 불필요)
                StartCoroutine(ExecuteGuard());
                break;
            case ActionType.Ultimate:
                // UP 체크
                if (currentUnit.currentUP < currentUnit.stats.UP)
                {
                    Debug.Log("궁극기 포인트 부족!");
                    actionMenuUI.HideReturnMenu();
                    actionMenuUI.ShowActionMenu();
                    return;
                }
                ShowAttackableTiles();
                break;
        }
    }

    private IEnumerator ExecuteGuard()
    {
        actionMenuUI.HideAllMenus();
        // 방어 로직 (추후 구현)
        Debug.Log("방어 실행");
        yield return null;
        turnManager.isTurnEnd = true;
    }

    // 취소 버튼 - 타일 선택 모드에서 메뉴로 복귀
    public void OnCancelAction()
    {
        ClearHighlights();
        actionMenuUI.HideReturnMenu();
        actionMenuUI.ShowActionMenu();
    }
}
