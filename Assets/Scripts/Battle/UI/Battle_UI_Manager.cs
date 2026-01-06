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
    public GameObject GameOver_Panel;
    public Turn_Gauge turnGaugeUI;
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
