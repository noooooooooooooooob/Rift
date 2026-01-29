using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 전투 UI 관리자
/// 턴 게이지 UI 및 기타 전투 UI 요소들을 관리
/// </summary>
public class Battle_UI_Manager : MonoBehaviour
{
    public static Battle_UI_Manager instance;

    public List<Transform> unit_UI_Positions; // 유닛 UI 위치 리스트

    [Header("Panel References")]
    public GameObject battleUI_Panel;
    public GameObject GameOver_Panel;
    public Turn_Gauge turnGaugeUI;
    public RectTransform[] playerUI_Anchors; // 플레이어 유닛 UI 앵커들
    public RectTransform[] enemyUI_Anchors;  // 적 유닛 UI
    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            return;
        }
    }
    void Start()
    {
        if (GameOver_Panel != null)
        {
            GameOver_Panel.SetActive(false);
        }
    }
    public void OnDeploymentPhaseStart()
    {
        var partyUnits = Party_Manager.instance.GetPartyUnits();
        for (int i = 0; i < partyUnits.Count && i < playerUI_Anchors.Length; i++)
        {
            Unit unit = partyUnits[i];
            unit.hpBar.UpdateBar(unit.currentHP, unit.maxHP);
            unit.upBar.UpdateBar(unit.currentUP, unit.stats.UP);

            // 원래 부모 저장 후 이동
            unit.originalUIParent = unit.uiAnchor.parent;
            unit.uiAnchor.SetParent(playerUI_Anchors[i], false);
            unit.uiAnchor.anchoredPosition = Vector2.zero;
            unit.uiAnchor.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// UI를 원래 부모로 되돌리기
    /// </summary>
    public void ReturnUIToUnits()
    {
        foreach (Unit unit in Party_Manager.instance.GetPartyUnits())
        {
            if (unit.originalUIParent != null)
            {
                unit.uiAnchor.SetParent(unit.originalUIParent, false);
                unit.uiAnchor.anchoredPosition = Vector2.zero;
                unit.uiAnchor.gameObject.SetActive(false);
            }
        }
    }

    public void OnBattlePhaseStart()
    {
        // 전투 UI 패널 활성화
        if (battleUI_Panel != null)
        {
            battleUI_Panel.SetActive(true);
        }
    }
    public void OnBattleEnd()
    {
        // 전투 UI 패널 비활성화
        if (battleUI_Panel != null)
        {
            battleUI_Panel.SetActive(false);
        }
        if (GameOver_Panel != null)
        {
            GameOver_Panel.SetActive(true);
        }
    }
}
