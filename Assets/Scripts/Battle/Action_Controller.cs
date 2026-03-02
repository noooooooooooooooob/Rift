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
            // Active Point 체크
            if (!actionMenuUI.CanConfirmCurrentAction())
                return;

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
            // 이전 공격 범위 하이라이트 제거
            ClearAvailableTiles();

            hoveredTile = currentHoveredTile;

            // 새 타일이 선택 가능한 타일이면 공격 범위 표시
            if (hoveredTile != null && highlightedTiles.Contains(hoveredTile))
            {
                ShowAttackRangeOnHover();
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

    private void ShowAttackAvailableTiles()
    {
        ClearHighlights();

        SkillData skillData = null;
        switch(currentMode)
        {
            case ActionType.Attack:
                skillData = currentUnit.attackAction.attackSkillData;
                break;
            case ActionType.Skill1:
                skillData = currentUnit.skill1Action.skill1Data;
                break;
            case ActionType.Skill2:
                skillData = currentUnit.skill2Action.skill2Data;
                break;
            case ActionType.Ultimate:
                skillData = currentUnit.ultimateAction.ultimateSkillData;
                break;
        }
        if(skillData.targetType == targetType.Enemy)
        {
            // 모든 플레이어 타일을 선택 가능 타일로 표시
            foreach (var tile in Battle_Manager.instance.enemyTiles)
            {
                tile.SetHighlight(HighlightType.Attackable);
                highlightedTiles.Add(tile);
            }
            return;
        }
        else if(skillData.targetType == targetType.Ally)
        {
            // 모든 적 타일을 선택 가능 타일로 표시
            foreach (var tile in Battle_Manager.instance.playerTiles)
            {
                tile.SetHighlight(HighlightType.Attackable);
                highlightedTiles.Add(tile);
            }
            return;
        }
        else if(skillData.targetType == targetType.Self)
        {
            // 자신의 타일만 선택 가능 타일로 표시
            if(currentUnit.currentTile != null)
            {
                currentUnit.currentTile.SetHighlight(HighlightType.Attackable);
                highlightedTiles.Add(currentUnit.currentTile);
            }
            return;
        }
    }

    private void ShowAttackRangeOnHover()
    {
        if (currentMode == ActionType.Move) return;

        // currentMode에 따라 적절한 스킬 데이터 선택
        SkillData skillData = null;
        switch (currentMode)
        {
            case ActionType.Attack:
                skillData = currentUnit.attackAction?.attackSkillData;
                break;
            case ActionType.Skill1:
                skillData = currentUnit.skill1Action?.skill1Data;
                break;
            case ActionType.Skill2:
                skillData = currentUnit.skill2Action?.skill2Data;
                break;
            case ActionType.Ultimate:
                skillData = currentUnit.ultimateAction?.ultimateSkillData;
                break;
        }

        if (skillData == null) return;

        // 호버된 타일 기준 실제 공격 범위 표시
        List<Tile> tiles = Method.GetAttackableTiles(currentUnit, skillData, hoveredTile);
        foreach (var tile in tiles)
        {
            tile.SetHighlight(HighlightType.Deployment);
            availableTiles.Add(tile);
        }
    }

    private void ClearAvailableTiles()
    {
        foreach (var tile in availableTiles)
        {
            // highlightedTiles에 포함되어 있으면 원래 하이라이트로 복원
            if (highlightedTiles.Contains(tile))
            {
                tile.SetHighlight(HighlightType.Attackable);
            }
            else
            {
                tile.SetHighlight(HighlightType.None);
            }
        }
        availableTiles.Clear();
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
        yield return new WaitUntil(() => !currentUnit.unitAnimation.isMoving);
        turnManager.isTurnEnd = true;
    }

    private IEnumerator HandleAttack(Tile tile)
    {
        if (tile == null || !highlightedTiles.Contains(tile))
            yield break;

        List<Tile> attackableTiles = Method.GetAttackableTiles(currentUnit, currentUnit.attackAction.attackSkillData, tile);

        // 타겟 유닛이 없으면 실행 안함
        if (!attackableTiles.Exists(t => t.occupant != null))
            yield break;

        ClearHighlights();
        actionMenuUI.HideAllMenus();

        StartCoroutine(currentUnit.attackAction.ExecuteAttack(attackableTiles));
        yield return new WaitUntil(() => !currentUnit.unitAnimation.isAttacking);
        turnManager.isTurnEnd = true;
    }

    private IEnumerator HandleSkill1(Tile tile)
    {
        if (tile == null || !highlightedTiles.Contains(tile))
            yield break;

        List<Tile> attackableTiles = Method.GetAttackableTiles(currentUnit, currentUnit.skill1Action.skill1Data, tile);

        // 타겟 유닛이 없으면 실행 안함
        if (!attackableTiles.Exists(t => t.occupant != null))
            yield break;

        ClearHighlights();
        actionMenuUI.HideAllMenus();

        StartCoroutine(currentUnit.skill1Action.ExecuteSkill1(attackableTiles));
        yield return new WaitUntil(() => !currentUnit.unitAnimation.isAttacking);
        turnManager.isTurnEnd = true;
        yield return null;
    }

    private IEnumerator HandleSkill2(Tile tile)
    {
        if (tile == null || !highlightedTiles.Contains(tile))
            yield break;

        List<Tile> attackableTiles = Method.GetAttackableTiles(currentUnit, currentUnit.skill2Action.skill2Data, tile);

        // 타겟 유닛이 없으면 실행 안함
        if (!attackableTiles.Exists(t => t.occupant != null))
            yield break;

        ClearHighlights();
        actionMenuUI.HideAllMenus();

        StartCoroutine(currentUnit.skill2Action.ExecuteAttack(attackableTiles));
        yield return new WaitUntil(() => !currentUnit.unitAnimation.isAttacking);
        turnManager.isTurnEnd = true;

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

        // 궁극기 스킬 실행
        List<Tile> attackableTiles = Method.GetAttackableTiles(
            currentUnit,
            currentUnit.ultimateAction.ultimateSkillData,
            tile
        );

        // 타겟 유닛이 없으면 실행 안함
        if (!attackableTiles.Exists(t => t.occupant != null))
            yield break;

        ClearHighlights();
        actionMenuUI.HideAllMenus();

        StartCoroutine(currentUnit.ultimateAction.ExecuteUltimate(attackableTiles));
        yield return new WaitUntil(() => !currentUnit.unitAnimation.isAttacking);

        // Return 버튼 다시 활성화
        if (actionMenuUI.returnButton != null)
            actionMenuUI.returnButton.gameObject.SetActive(true);

        // 궁극기 턴 종료 (Turn_Manager가 원래 유닛 턴 재개 처리)
        turnManager.isTurnEnd = true;
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
    /// 외부에서 호출 가능한 하이라이트 클리어
    /// </summary>
    public void ClearAllHighlights()
    {
        ClearHighlights();
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

        Battle_UI_Manager.instance.IsClickInteractable = false;

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
                ShowAttackAvailableTiles();
                break;
            case ActionType.Skill1:
                ShowAttackAvailableTiles();
                break;
            case ActionType.Skill2:
                ShowAttackAvailableTiles();
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
                ShowAttackAvailableTiles();
                break;
        }
    }

    private IEnumerator ExecuteGuard()
    {
        actionMenuUI.HideAllMenus();
        currentUnit.guardAction.ExecuteGuard();
        yield return null;
        turnManager.isTurnEnd = true;
    }

    // 취소 버튼 - 타일 선택 모드에서 메뉴로 복귀
    public void OnCancelAction()
    {
        // 궁극기 모드에서는 취소 불가
        if (currentMode == ActionType.Ultimate)
            return;

        Battle_UI_Manager.instance.IsClickInteractable = true;

        ClearHighlights();
        actionMenuUI.HideReturnMenu();
        actionMenuUI.ShowActionMenu();
    }

    /// <summary>
    /// 궁극기 전용 턴 시작 (취소 불가)
    /// </summary>
    public void StartUltimateTurn(Unit unit)
    {
        currentUnit = unit;
        actionMenuUI = currentUnit.actionMenuUI;
        currentMode = ActionType.Ultimate;

        // 메뉴 숨기고 바로 타일 선택 모드
        actionMenuUI.HideAllMenus();
        ShowAttackAvailableTiles();

        // Return 버튼 비활성화 (취소 불가)
        if (actionMenuUI.returnButton != null)
            actionMenuUI.returnButton.gameObject.SetActive(false);


    }
}
