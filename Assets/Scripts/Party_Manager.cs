using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

/// <summary>
/// 파티 유닛 관리자 (싱글톤, DontDestroyOnLoad)
/// 파티 선택 씬에서 전투 씬으로 유닛 데이터를 전달하는 역할
/// 씬 전환 시에도 파티 유닛들을 유지하여 연속적인 게임 진행 지원
/// </summary>
public class Party_Manager : MonoBehaviour
{
    public static Party_Manager instance;  // 싱글톤 인스턴스

    [Header("Party Management")]
    public List<GameObject> partyUnits = new List<GameObject>();  // 선택된 파티 유닛 GameObject들 (DontDestroyOnLoad로 보존)

    /// <summary>
    /// 초기화: 싱글톤 설정 및 DontDestroyOnLoad 적용
    /// </summary>
    void Awake()
    {
        // 싱글톤 설정
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Stage Select Scene")  // 씬 이름 확인
        {
            ResetPartyUnitsScale();
        }
    }

    /// <summary>
    /// 파티 선택 씬에서 호출: 유닛을 파티에 추가
    /// GameObject에 DontDestroyOnLoad를 적용하여 씬 전환 시에도 유지
    /// </summary>
    public void AddUnitToParty(GameObject unitGO)
    {
        if (unitGO == null)
        {
            Debug.LogError("AddUnitToParty: unitGO is null!");
            return;
        }

        Unit unit = unitGO.GetComponent<Unit>();
        if (unit == null)
        {
            Debug.LogError("AddUnitToParty: GameObject에 Unit 컴포넌트가 없습니다!");
            return;
        }

        if (!partyUnits.Contains(unitGO))
        {
            partyUnits.Add(unitGO);
            DontDestroyOnLoad(unitGO);  // GameObject를 씬 전환 시 유지

            // SpriteBillboard 비활성화 (파티 상태에서는 카메라 추적 불필요)
            SpriteBillboard billboard = unitGO.GetComponent<SpriteBillboard>();
            if (billboard != null)
            {
                billboard.enabled = false;
            }

            Debug.Log($"{unit.unitData.unitName} 파티에 추가됨 (총 {partyUnits.Count}명)");
        }
        else
        {
            Debug.LogWarning($"{unit.unitData.unitName}은 이미 파티에 있습니다.");
        }
    }

    /// <summary>
    /// 파티에서 유닛 제거
    /// GameObject도 함께 파괴됨
    /// </summary>
    public void RemoveUnitFromParty(GameObject unitGO)
    {
        if (partyUnits.Contains(unitGO))
        {
            Unit unit = unitGO.GetComponent<Unit>();
            string unitName = unit != null ? unit.unitData.unitName : unitGO.name;

            partyUnits.Remove(unitGO);
            Destroy(unitGO);  // 제거 시 파괴
            Debug.Log($"{unitName} 파티에서 제거됨 (남은 인원: {partyUnits.Count}명)");
        }
    }

    /// <summary>
    /// 전투 씬에서 호출: 파티 유닛 리스트 가져오기
    /// GameObject에서 Unit 컴포넌트를 추출하여 반환
    /// </summary>
    public List<Unit> GetPartyUnits()
    {
        List<Unit> units = new List<Unit>();
        foreach (GameObject go in partyUnits)
        {
            if (go != null)
            {
                Unit unit = go.GetComponent<Unit>();
                if (unit != null)
                    units.Add(unit);
            }
        }
        return units;
    }

    /// <summary>
    /// 전투 종료 후 호출: 유닛 상태 업데이트
    /// 사망한 유닛을 파티에서 제거 (HP 등은 Unit 객체에 이미 저장됨)
    /// </summary>
    public void UpdatePartyAfterBattle()
    {
        // SpriteBillboard 비활성화 (전투씬 이후에는 불필요)
        foreach (GameObject go in partyUnits)
        {
            if (go != null)
            {
                SpriteBillboard billboard = go.GetComponent<SpriteBillboard>();
                if (billboard != null)
                {
                    billboard.enabled = false;
                }
            }
        }

        HidePartyUnits();
        // 사망한 유닛 제거
        int deadCount = 0;
        for (int i = partyUnits.Count - 1; i >= 0; i--)
        {
            GameObject go = partyUnits[i];
            if (go == null)
            {
                partyUnits.RemoveAt(i);
                deadCount++;
                continue;
            }

            Unit unit = go.GetComponent<Unit>();
            if (unit != null && unit.isDead)
            {
                Destroy(go);
                partyUnits.RemoveAt(i);
                deadCount++;
            }
        }

        if (deadCount > 0)
            Debug.Log($"전투 종료: {deadCount}명 사망, 생존 인원: {partyUnits.Count}명");
    }

    /// <summary>
    /// 파티 초기화 (게임 시작/게임오버 시)
    /// 모든 파티 유닛을 제거하고 리스트 초기화
    /// </summary>
    public void ClearParty()
    {
        foreach (GameObject go in partyUnits)
        {
            if (go != null)
                Destroy(go);
        }
        partyUnits.Clear();
        Debug.Log("파티 초기화 완료");
    }

    /// <summary>
    /// 파티가 비어있는지 확인
    /// </summary>
    public bool IsPartyEmpty()
    {
        return partyUnits.Count == 0;
    }

    /// <summary>
    /// 파티 인원 수 확인
    /// </summary>
    public int GetPartyCount()
    {
        return partyUnits.Count;
    }

    public void OnBattleEnd()
    {
        UpdatePartyAfterBattle();
    }

    /// <summary>
    /// 파티 유닛들 비활성화 (맵 씬에서 사용)
    /// </summary>
    public void HidePartyUnits()
    {
        foreach (GameObject go in partyUnits)
        {
            if (go != null)
                go.SetActive(false);
        }
    }

    /// <summary>
    /// 파티 유닛들 스케일 원상복구 (캐릭터 셀렉트 → 스테이지 셀렉트 전환 시)
    /// </summary>
    public void ResetPartyUnitsScale()
    {
        foreach (GameObject go in partyUnits)
        {
            if (go != null)
            {
                go.transform.localScale = Vector3.one;
                go.SetActive(false);
            }
        }
    }

    public void HealParty(float percentage)
    {
        foreach (GameObject go in partyUnits)
        {
            if (go != null)
            {
                Unit unit = go.GetComponent<Unit>();
                if (unit != null && !unit.isDead)
                {
                    int healAmount = Mathf.CeilToInt(unit.maxHP * percentage);
                    unit.Heal(healAmount);
                }
            }
        }
    }
}
