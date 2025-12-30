using UnityEngine;

/// <summary>
/// 전투 UI 관리자
/// 턴 게이지 UI 및 기타 전투 UI 요소들을 관리
/// </summary>
public class Battle_UI_Manager : MonoBehaviour
{
    public static Battle_UI_Manager instance;

    [Header("Panel References")]
    public GameObject battleUI_Panel;
    public Turn_Gauge turnGaugeUI;

    void Awake()
    {
        // 싱글톤 초기화
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogWarning("Battle_UI_Manager: Multiple instances detected!");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 초기에는 전투 UI 숨김
        if (battleUI_Panel != null)
        {
            battleUI_Panel.SetActive(false);
        }
    }

    /// <summary>
    /// 전투 UI 표시
    /// </summary>
    public void ShowBattleUI()
    {
        if (battleUI_Panel != null)
        {
            battleUI_Panel.SetActive(true);
        }

        if (turnGaugeUI != null)
        {
            turnGaugeUI.Initialize();
        }
    }

    /// <summary>
    /// 전투 UI 숨김
    /// </summary>
    public void HideBattleUI()
    {
        if (battleUI_Panel != null)
        {
            battleUI_Panel.SetActive(false);
        }
    }

    /// <summary>
    /// 전투 페이즈 시작 시 호출
    /// Turn_Manager에서 호출됨
    /// </summary>
    public void OnBattlePhaseStart()
    {
        Debug.Log("Battle_UI_Manager: OnBattlePhaseStart called");

        ShowBattleUI();

        if (turnGaugeUI != null)
        {
            turnGaugeUI.StartUpdating();
        }
        else
        {
            Debug.LogWarning("Battle_UI_Manager: turnGaugeUI is null!");
        }
    }

    /// <summary>
    /// 전투 종료 시 호출
    /// </summary>
    public void OnBattleEnd()
    {
        HideBattleUI();

        if (turnGaugeUI != null)
        {
            turnGaugeUI.enabled = false;
        }
    }
}
