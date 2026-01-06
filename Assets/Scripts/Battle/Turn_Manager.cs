using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// 전투 단계를 정의하는 열거형
/// Deployment(배치) → Battle(전투) → Result(결과) 순서로 진행
/// </summary>
public enum BattlePhase
{
    Deployment,  // 유닛 배치 단계
    Battle,      // 전투 진행 단계
    Result       // 전투 결과 단계
}

/// <summary>
/// 전투 턴 흐름을 관리하는 상태 머신
/// 코루틴 기반으로 각 단계를 순차적으로 처리
/// </summary>
public class Turn_Manager : MonoBehaviour
{
    public BattlePhase currentPhase;  // 현재 전투 단계
    public Deployment_Controller deploymentController;  // 배치 컨트롤러 참조

    [Header("UI References")]
    public Button startBattleButton;  // 전투 시작 버튼

    [Header("Battle References")]
    public TurnGuage_Manager turnGaugeManager;  // 턴 게이지 매니저 참조
    public Action_Controller actionController;  // 액션 컨트롤러 참조

    private bool isReadyToStartBattle = false;  // 버튼 클릭 여부 플래그
    private Unit currentTurnUnit;  // 현재 턴을 받은 유닛
    private bool shouldCheckBattleEnd = false;  // 유닛 사망 시 true로 설정되어 승패 체크 트리거
    public bool isTurnEnd = false;  // 유닛 턴 종료 여부 플래그

    /// <summary>
    /// 초기화: 버튼 이벤트 연결 및 초기 상태 설정
    /// </summary>
    private void Start()
    {
        // 버튼 이벤트 연결
        if (startBattleButton != null)
        {
            startBattleButton.onClick.AddListener(OnStartBattleButtonClick);
            startBattleButton.gameObject.SetActive(false);  // 초기에는 숨김
        }
        else
        {
            Debug.LogWarning("Start Battle Button이 할당되지 않았습니다!");
        }
    }

    /// <summary>
    /// 전투 단계를 순환하는 메인 상태 머신
    /// 각 단계별 코루틴을 실행하며 무한 루프로 동작
    /// </summary>
    private IEnumerator State()
    {
        while (true)
        {
            switch (currentPhase)
            {
                case BattlePhase.Deployment:
                    yield return StartCoroutine(HandleDeploymentPhase());
                    break;
                case BattlePhase.Battle:
                    yield return StartCoroutine(HandleBattlePhase());
                    break;
                case BattlePhase.Result:
                    // yield return StartCoroutine(HandleResultPhase());  // TODO: 결과 단계 구현 필요
                    yield break;  // 결과 단계 후 상태 머신 종료
            }
        }
    }

    /// <summary>
    /// 배치 단계 처리 코루틴
    /// 1. 배치 시작 → 2. 버튼 표시 → 3. 모든 유닛 배치 대기 → 4. 버튼 활성화 → 5. 버튼 클릭 대기 → 6. 배치 완료 처리 → 7. 다음 단계로 이동
    /// </summary>
    private IEnumerator HandleDeploymentPhase()
    {
        Debug.Log("Deployment Phase Started");

        // 배치 시작
        CameraManager.instance.canFocus = false;
        deploymentController.BeginDeployment();

        // 버튼 표시 (비활성화 상태로)
        if (startBattleButton != null)
        {
            startBattleButton.gameObject.SetActive(true);
            startBattleButton.interactable = false;
        }

        // 모든 유닛이 배치될 때까지 대기
        while (!deploymentController.IsDeploymentValid())
        {
            yield return null;
        }

        Debug.Log("배치 완료! 전투 시작 버튼을 눌러주세요.");

        // 버튼 활성화 (클릭 가능)
        if (startBattleButton != null)
        {
            startBattleButton.interactable = true;
        }

        // 플레이어가 전투 시작 버튼을 클릭할 때까지 대기
        yield return new WaitUntil(() => isReadyToStartBattle);

        Debug.Log("전투 시작! 배치 완료 처리 중...");

        // 버튼 숨김
        if (startBattleButton != null)
        {
            startBattleButton.gameObject.SetActive(false);
        }

        // 배치 완료 처리
        deploymentController.FinishDeployment();

        Debug.Log("Deployment Phase Ended");

        currentPhase = BattlePhase.Battle;  // 다음 단계로 전환 (Battle Phase)
        CameraManager.instance.canFocus = true;
    }

    /// <summary>
    /// 전투 시작 버튼 클릭 핸들러
    /// 플래그를 설정하여 배치 단계 코루틴이 다음으로 진행하도록 함
    /// </summary>
    private void OnStartBattleButtonClick()
    {
        Debug.Log("전투 시작 버튼 클릭됨!");
        isReadyToStartBattle = true;
    }

    /// <summary>
    /// 컷신 종료 콜백
    /// Scene_Start_CutScene에서 타임라인 종료 시 호출되어 배치 단계를 시작
    /// </summary>
    public void OnCutSceneEnd()
    {
        Debug.Log("CutScene End → Deployment Phase로 전환");
        currentPhase = BattlePhase.Deployment;
        isReadyToStartBattle = false;  // 플래그 초기화
        StartCoroutine(State());  // 상태 머신 시작
    }

    /// <summary>
    /// 전투 단계 처리 코루틴
    /// 1. 턴 시스템 초기화 → 2. 전투 루프 (게이지 증가 → 턴 유닛 행동 → 승패 체크) → 3. 결과 단계로 전환
    /// </summary>
    private IEnumerator HandleBattlePhase()
    {
        Debug.Log("Battle Phase Started");

        // 0. 전투 UI 초기화
        if (Battle_UI_Manager.instance != null)
        {
            Battle_UI_Manager.instance.OnBattlePhaseStart();
        }
        else
        {
            Debug.LogWarning("Battle_UI_Manager.instance is null!");
        }

        // 1. 모든 유닛 초기화 (Turn_Manager 참조 전달)
        foreach (var unit in Battle_Manager.instance.playerUnits)
        {
            if (unit != null)
                unit.Initialize(true, this);
        }
        foreach (var unit in Battle_Manager.instance.enemyUnits)
        {
            if (unit != null)
                unit.Initialize(false, this);
        }

        // 2. 턴 게이지 시스템 초기화
        if (turnGaugeManager != null)
        {
            turnGaugeManager.InitializeTurnSystem(
                Battle_Manager.instance.playerUnits,
                Battle_Manager.instance.enemyUnits
            );
        }
        else
        {
            Debug.LogError("TurnGaugeManager가 할당되지 않았습니다!");
            yield break;
        }

        // 3. 전투 메인 루프
        while (true)
        {
            // 3-1. 유닛이 죽었을 때만 승리/패배 조건 체크 (효율적)
            if (shouldCheckBattleEnd)
            {
                shouldCheckBattleEnd = false;  // 플래그 리셋

                if (CheckBattleEnd())
                {
                    Debug.Log("전투 종료 조건 충족!");

                    // 전투 UI 숨김
                    if (Battle_UI_Manager.instance != null)
                    {
                        Battle_UI_Manager.instance.OnBattleEnd();
                    }

                    currentPhase = BattlePhase.Result;
                    yield break;
                }
            }

            // 3-2. 모든 유닛의 턴 게이지 증가 (SPD 기반)
            turnGaugeManager.UpdateTurnGauges(Time.deltaTime);

            // 3-3. 턴을 받을 준비가 된 유닛이 있는지 확인
            if (turnGaugeManager.HasUnitReadyForTurn())
            {
                // 3-4. 다음 턴 유닛 가져오기 (게이지 100 소모)
                currentTurnUnit = turnGaugeManager.GetNextTurnUnit();

                if (currentTurnUnit != null)
                {
                    Debug.Log($"[Turn] {currentTurnUnit.unitData.unitName}의 턴!");

                    // 3-5. 유닛 행동 실행
                    yield return StartCoroutine(ExecuteUnitTurn(currentTurnUnit));

                    currentTurnUnit.turnGauge -= 100.0f;  // 행동 후 게이지 100 소모
                }
            }

            yield return null;  // 다음 프레임까지 대기
        }
    }

    /// <summary>
    /// 전투 종료 조건 체크
    /// 플레이어 또는 적이 전멸했는지 확인
    /// </summary>
    /// <returns>true: 전투 종료, false: 전투 계속</returns>
    private bool CheckBattleEnd()
    {
        var playerUnits = Battle_Manager.instance.playerUnits;
        var enemyUnits = Battle_Manager.instance.enemyUnits;

        // 플레이어 유닛이 모두 죽었는지
        bool allPlayersDead = playerUnits.TrueForAll(u => u == null || u.isDead);

        // 적 유닛이 모두 죽었는지
        bool allEnemiesDead = enemyUnits.TrueForAll(u => u == null || u.isDead);

        if (allPlayersDead)
        {
            Debug.Log("패배: 모든 플레이어 유닛이 전멸했습니다.");
            return true;
        }

        if (allEnemiesDead)
        {
            Debug.Log("승리: 모든 적 유닛이 전멸했습니다.");
            return true;
        }

        return false;
    }

    /// <summary>
    /// 유닛의 턴 행동 실행
    /// 플레이어 유닛: 입력 대기 (미구현)
    /// 적 유닛: AI 행동 (미구현)
    /// </summary>
    private IEnumerator ExecuteUnitTurn(Unit unit)
    {
        if (unit.isPlayerUnit)
        {
            Debug.Log($"[Player Turn] {unit.unitData.unitName} - 입력 대기 중 (TODO)");
            actionController.TurnStart(unit);
            isTurnEnd = false;

            yield return new WaitUntil(() => isTurnEnd);

            yield return new WaitForSeconds(1f);  // 임시 대기
        }
        else
        {
            Debug.Log($"[Enemy Turn] {unit.unitData.unitName} - AI 행동 중 (TODO)");
            // TODO: 적 AI 행동
            // - 공격 대상 선택
            // - 행동 실행
            yield return new WaitForSeconds(1f);  // 임시 대기
        }
    }

    /// <summary>
    /// 유닛 사망 시 호출되는 콜백
    /// Unit.OnDeath()에서 호출되어 승패 체크를 트리거
    /// </summary>
    public void OnUnitDeath(Unit deadUnit)
    {
        Debug.Log($"[Turn_Manager] {deadUnit.unitData.unitName} 사망 감지");
        Battle_Manager.instance.RemoveUnitFromBattle(deadUnit);
        shouldCheckBattleEnd = true;  // 다음 프레임에 승패 체크
    }
}
