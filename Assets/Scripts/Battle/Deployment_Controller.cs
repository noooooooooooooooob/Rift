using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

/// <summary>
/// 유닛 배치를 관리하는 컨트롤러
/// 드래그 앤 드롭 방식으로 파티 유닛을 전투 그리드에 배치
/// Preview 시스템 없이 실제 Unit GameObject를 직접 사용
/// </summary>
public class Deployment_Controller : MonoBehaviour
{
    private List<Unit> unitsToDeployData;  // Party_Manager로부터 받아온 유닛들
    private List<Unit> placedUnits = new List<Unit>();  // 배치된 유닛들
    private Queue<Unit> unitQueue = new Queue<Unit>();  // 배치 대기 중인 유닛 큐
    private Unit draggingUnit;  // 현재 드래그 중인 유닛

    Camera mainCamera;  // 메인 카메라 캐싱
    LayerMask tileMask;  // 타일 레이어 마스크 (레이캐스트용)
    Tile currentHoverTile;  // 현재 마우스가 올라간 타일

    /// <summary>
    /// 초기화: 카메라와 레이어 마스크 설정
    /// </summary>
    private void Start()
    {
        mainCamera = Camera.main;
        tileMask = LayerMask.GetMask("Tile");
    }

    /// <summary>
    /// 배치 프로세스 시작
    /// Party_Manager로부터 파티 유닛을 가져와 큐에 넣고 첫 번째 유닛 드래그 시작
    /// </summary>
    public void BeginDeployment()
    {
        Battle_UI_Manager.instance.OnDeploymentPhaseStart();
        // Party_Manager로부터 파티 유닛 가져오기
        if (Party_Manager.instance == null)
        {
            Debug.LogError("Party_Manager.instance is null! 파티 유닛을 불러올 수 없습니다.");
            return;
        }

        unitsToDeployData = Party_Manager.instance.GetPartyUnits();

        if (unitsToDeployData.Count == 0)
        {
            Debug.LogWarning("파티가 비어있습니다! 배치할 유닛이 없습니다.");
            return;
        }

        Debug.Log($"[Deployment] Party_Manager에서 받아온 유닛: {unitsToDeployData.Count}명");
        Debug.Log($"[Deployment] 배치 시작 전 Battle_Manager.playerUnits: {Battle_Manager.instance.playerUnits.Count}명");

        // 배치할 유닛들을 큐에 추가
        foreach (Unit unit in unitsToDeployData)
        {
            // 유닛을 화면 밖에 숨김
            unit.transform.position = new Vector3(-1000, -1000, 0);
            unit.gameObject.SetActive(true);

            unitQueue.Enqueue(unit);
        }

        // 첫 번째 유닛 드래그 시작
        if (unitQueue.Count > 0)
        {
            StartDrag(unitQueue.Dequeue());
        }
    }

    /// <summary>
    /// 유닛 드래그 시작
    /// 활성화하고 마우스를 따라다니도록 설정
    /// </summary>
    private void StartDrag(Unit unit)
    {
        draggingUnit = unit;
        draggingUnit.GetComponent<UnitClickable>().enabled = false; // 드래그 중 클릭 비활성화
        draggingUnit.gameObject.SetActive(true);

        // 드래그 중에는 반투명하게 표시
        SpriteRenderer sr = draggingUnit.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = new Color(1, 1, 1, 0.7f);
        }

        currentHoverTile = null;
    }

    /// <summary>
    /// 이미 배치된 유닛을 다시 드래그 시작 (재배치)
    /// 기존 타일 점유를 해제하고 다시 드래그 가능 상태로 변경
    /// </summary>
    public void ReStartDrag(Unit unit)
    {
        if (draggingUnit != null) return;

        // 기존 타일 점유 해제
        if (unit.currentTile != null)
        {
            unit.currentTile.occupant = null;
            unit.currentTile = null;
        }

        // placedUnits에서 제거 (다시 배치할 것이므로)
        placedUnits.Remove(unit);

        // Battle_Manager에서도 제거
        Battle_Manager.instance.playerUnits.Remove(unit);

        StartDrag(unit);
        draggingUnit.GetComponent<UnitClickable>().enabled = false; // 드래그 중 클릭 비활성화
    }

    /// <summary>
    /// 매 프레임 드래그 처리
    /// 마우스를 따라다니며, 좌클릭 시 배치 시도
    /// </summary>
    void Update()
    {
        if (draggingUnit == null) return;

        FollowMouse();

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            TryPlaceDraggingUnit();
        }
    }

    /// <summary>
    /// 유닛이 마우스 커서를 따라다니도록 처리
    /// 타일 위에 있으면 타일 위치로 스냅, 아니면 레이 상의 고정 거리에 위치
    /// </summary>
    void FollowMouse()
    {
        Vector3 mousePos = Mouse.current.position.ReadValue();
        Ray ray = mainCamera.ScreenPointToRay(mousePos);

        // 기본 위치: 마우스 따라다님
        draggingUnit.transform.position = ray.origin + ray.direction * 10f;

        // Raycast로 타일 감지
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, tileMask))
        {
            Tile tile = hit.collider.GetComponent<Tile>();

            if (tile != null && CanPlaceOnTile(tile))
            {
                // 타일 위치로 스냅 (유닛 오프셋 적용)
                draggingUnit.transform.position = tile.WorldPosition + draggingUnit.unitOffset;
                UpdateHoverTile(tile);
                return;
            }
        }

        // 타일 위가 아님
        ClearHoverTile();
    }

    /// <summary>
    /// 현재 hover 중인 타일 업데이트
    /// 이전 타일 하이라이트 해제하고 새 타일 하이라이트 활성화
    /// </summary>
    private void UpdateHoverTile(Tile tile)
    {
        if (currentHoverTile == tile) return;

        // 이전 타일 하이라이트 해제
        if (currentHoverTile != null)
        {
            currentHoverTile.SetHighlight(HighlightType.None);
        }

        currentHoverTile = tile;
        currentHoverTile.SetHighlight(HighlightType.Deployment);
    }

    /// <summary>
    /// hover 타일 초기화 (하이라이트 해제)
    /// </summary>
    private void ClearHoverTile()
    {
        if (currentHoverTile != null)
        {
            currentHoverTile.SetHighlight(HighlightType.None);
            currentHoverTile = null;
        }
    }

    /// <summary>
    /// 현재 드래그 중인 유닛 배치 시도
    /// 유효한 타일 위에 있으면 배치하고 다음 유닛으로 이동
    /// </summary>
    private void TryPlaceDraggingUnit()
    {
        if (currentHoverTile != null && CanPlaceOnTile(currentHoverTile))
        {
            PlaceUnit(draggingUnit, currentHoverTile);
            draggingUnit.GetComponent<UnitClickable>().enabled = true; // 배치 후 클릭 활성화
            ClearHoverTile();

            // 다음 유닛 드래그 시작
            if (unitQueue.Count > 0)
            {
                StartDrag(unitQueue.Dequeue());
                draggingUnit.GetComponent<UnitClickable>().enabled = false; // 드래그 중 클릭 비활성화
            }
            else
            {
                draggingUnit = null;  // 모든 배치 완료
            }
        }
    }

    /// <summary>
    /// 유닛을 타일에 배치
    /// 타일 점유 설정 및 배치 목록에 추가, Battle_Manager에 등록
    /// </summary>
    void PlaceUnit(Unit unit, Tile tile)
    {
        // 유닛을 타일에 배치 (유닛 오프셋 적용)
        unit.transform.position = tile.WorldPosition + unit.unitOffset;
        unit.currentTile = tile;
        tile.occupant = unit.gameObject;

        // 불투명하게 복원
        SpriteRenderer sr = unit.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = new Color(1, 1, 1, 1f);
        }

        // SpriteBillboard 활성화 (전투씬에서만 필요)
        SpriteBillboard billboard = unit.GetComponent<SpriteBillboard>();
        if (billboard != null)
        {
            billboard.enabled = true;
        }

        // 배치된 유닛 목록에 추가
        placedUnits.Add(unit);

        // Battle_Manager에 등록
        Battle_Manager.instance.playerUnits.Add(unit);

        // 턴 게이지 UI에 아이콘 추가
        Battle_UI_Manager.instance?.turnGaugeUI?.AddUnitIcon(unit);

        Debug.Log($"[PlaceUnit] {unit.unitData.unitName}을(를) {tile.GridPosition}에 배치 → Battle_Manager.playerUnits: {Battle_Manager.instance.playerUnits.Count}명");
    }

    /// <summary>
    /// 타일에 유닛 배치가 가능한지 확인
    /// 조건: 그리드 왼쪽 절반 (x < 5) && 점유되지 않음
    /// </summary>
    public bool CanPlaceOnTile(Tile tile)
    {
        return tile.isPlayerTile && !tile.IsOccupied;
    }

    /// <summary>
    /// 배치가 완료되었는지 확인
    /// 모든 유닛이 배치되었으면 true 반환
    /// </summary>
    public bool IsDeploymentValid()
    {
        // 모든 유닛이 배치되었는지 확인
        return placedUnits.Count == unitsToDeployData.Count;
    }

    /// <summary>
    /// 배치 완료 처리 (더 이상 Preview 변환이 필요 없음)
    /// </summary>
    public void FinishDeployment()
    {
        Debug.Log($"[FinishDeployment] placedUnits: {placedUnits.Count}명");
        Debug.Log($"[FinishDeployment] Battle_Manager.playerUnits: {Battle_Manager.instance.playerUnits.Count}명");

        Destroy(this);
    }
}
