using System.Collections.Generic;
using TMPro;
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
    public GameObject gameClear_Panel;
    public TMP_Text gameClear_MoneyText;
    public GameObject gameOver_Panel;
    public Turn_Gauge turnGaugeUI;
    public RectTransform[] playerUI_Anchors; // 플레이어 유닛 UI 앵커들
    public RectTransform[] enemyUI_Anchors;  // 적 유닛 UI
    public Unit_Description unitDescriptionUI;
    private bool isClickInteractable = false;
    public bool IsClickInteractable
    {
        get
        {
            return isClickInteractable;
        }
        set
        {
            isClickInteractable = value;
            if(isClickInteractable == false)
            {
                HideUnitDescription();
            }
        }
    }
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
        if (gameOver_Panel != null)
        {
            gameOver_Panel.SetActive(false);
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
        SetupEnemyUI();
    }

    public void SetupEnemyUI()
    {
        var enemyUnits = Battle_Manager.instance.enemyUnits;
        for (int i = 0; i < enemyUnits.Count && i < enemyUI_Anchors.Length; i++)
        {
            Unit unit = enemyUnits[i];
            unit.hpBar.UpdateBar(unit.currentHP, unit.maxHP);
            unit.upBar.UpdateBar(unit.currentUP, unit.stats.UP);

            // 원래 부모 저장 후 이동
            unit.originalUIParent = unit.uiAnchor.parent;
            unit.uiAnchor.SetParent(enemyUI_Anchors[i], false);
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
    public void ShowUnitDescription(Unit unit)
    {
        if(!isClickInteractable)
            return;
        unitDescriptionUI.ShowDescription(unit);
    }
    public void HideUnitDescription()
    {
        unitDescriptionUI.HideDescription();
    }

    public void OnBattlePhaseStart()
    {
        // 전투 UI 패널 활성화
        if (battleUI_Panel != null)
        {
            battleUI_Panel.SetActive(true);
        }
    }
    public void OnBattleEnd(bool allPlayersDead)
    {
        // 전투 UI 패널 비활성화
        if (battleUI_Panel != null)
        {
            battleUI_Panel.SetActive(false);
        }
        if(!allPlayersDead)
        {
            if (gameClear_Panel != null)
            {
                gameClear_MoneyText.text = Battle_Manager.instance.isBossBattle ?   Battle_Manager.instance.currentBossMapData.awardGold.ToString() : Battle_Manager.instance.currentBattleMapData.awardGold.ToString();
                gameClear_Panel.SetActive(true);
            }
        }
        else
        {
            if (gameOver_Panel != null)
            {
                gameOver_Panel.SetActive(true);
            }
        }
    }
}
