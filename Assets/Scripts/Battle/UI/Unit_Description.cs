using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SkillAreaImageEntry
{
    public Vector2Int position;
    public Image image;
}

public class Unit_Description : MonoBehaviour
{
    [SerializeField] private GameObject descriptionPanel;
    public Unit unit;
    public TextMeshProUGUI unitNameText;
    public TextMeshProUGUI unitHPText;
    public TextMeshProUGUI unitUPText;
    public TextMeshProUGUI unitAttackText;
    public TextMeshProUGUI unitDefenseText;
    public TextMeshProUGUI unitSpeedText;
    public TextMeshProUGUI unitAGIText;
    public TextMeshProUGUI unitLSText;
    public TextMeshProUGUI unitRSTText;
    public TextMeshProUGUI unitEVText;
    public TextMeshProUGUI unitCAText;
    public TextMeshProUGUI unitDRText;
    public TextMeshProUGUI unitAPText;
    public TextMeshProUGUI skillNameText;
    public TextMeshProUGUI skillDescriptionText;
    public TextMeshProUGUI skillCostText;
    public Button showpassiveButton;
    public Button showAttackButton;
    public Button showSkill1Button;
    public Button showSkill2Button;

    [Header("Button Colors")]
    public Color selectedColor = new Color(0.7f, 0.9f, 1f);

    private Button currentSelectedButton;
    private Dictionary<Button, Color> originalColors = new Dictionary<Button, Color>();

    [Header("Skill Area")]
    [SerializeField] private GameObject skillAreaContainer;
    [SerializeField] private List<SkillAreaImageEntry> skillAreaImageList;
    public Color selfOriginColor = Color.yellow;
    public Color attackColor = Color.red;
    public Color healColor = Color.green;
    public Color otherColor = Color.softBlue;

    public void Awake()
    {
        descriptionPanel.SetActive(false);

        // 각 버튼의 원래 색상 저장
        originalColors[showpassiveButton] = showpassiveButton.image.color;
        originalColors[showAttackButton] = showAttackButton.image.color;
        originalColors[showSkill1Button] = showSkill1Button.image.color;
        originalColors[showSkill2Button] = showSkill2Button.image.color;

        showpassiveButton.onClick.AddListener(() => ShowPassiveDescription());
        showAttackButton.onClick.AddListener(() => ShowAttackDescription());
        showSkill1Button.onClick.AddListener(() => ShowSkill1Description());
        showSkill2Button.onClick.AddListener(() => ShowSkill2Description());
    }
    private void SelectButton(Button button)
    {
        // 이전 선택 버튼 색상 복원
        if (currentSelectedButton != null && originalColors.ContainsKey(currentSelectedButton))
        {
            currentSelectedButton.image.color = originalColors[currentSelectedButton];
        }

        // 새 버튼 선택 및 색상 변경
        currentSelectedButton = button;
        if (currentSelectedButton != null)
        {
            currentSelectedButton.image.color = selectedColor;
        }
    }

    private void ShowPassiveDescription()
    {
        SelectButton(showpassiveButton);
        HideSkillArea();
        // TODO: UnitData에 패시브 필드 추가 후 연결
        skillNameText.text = "패시브";
        skillDescriptionText.text = "-";
    }
    private void ShowAttackDescription()
    {
        if (unit == null || unit.attackAction == null || unit.attackAction.attackSkillData == null) return;

        ShowSkillArea(unit.attackAction.attackSkillData);
        SelectButton(showAttackButton);
        skillNameText.text = unit.attackAction.attackSkillData.skillName;
        skillDescriptionText.text = unit.attackAction.attackSkillData.description;
        skillCostText.text = unit.attackAction.attackSkillData.cost.ToString();
    }
    private void ShowSkill1Description()
    {
        if (unit == null || unit.skill1Action == null || unit.skill1Action.skill1Data == null) return;

        ShowSkillArea(unit.skill1Action.skill1Data);
        SelectButton(showSkill1Button);
        skillNameText.text = unit.skill1Action.skill1Data.skillName;
        skillDescriptionText.text = unit.skill1Action.skill1Data.description;
        skillCostText.text = unit.skill1Action.skill1Data.cost.ToString();
    }
    private void ShowSkill2Description()
    {
        if (unit == null || unit.skill2Action == null || unit.skill2Action.skill2Data == null) return;

        ShowSkillArea(unit.skill2Action.skill2Data);
        SelectButton(showSkill2Button);
        
        skillNameText.text = unit.skill2Action.skill2Data.skillName;
        skillDescriptionText.text = unit.skill2Action.skill2Data.description;
        skillCostText.text = unit.skill2Action.skill2Data.cost.ToString();
    }
    public void ShowDescription(Unit unit)
    {
        if (unit == null || unit.unitData == null) return;

        descriptionPanel.SetActive(true);
        this.unit = unit;
        SetDescription();
        ShowPassiveDescription(); // 기본으로 Passive 선택
    }

    public void HideDescription()
    {
        descriptionPanel.SetActive(false);
        SelectButton(null); // 선택 상태 초기화
    }

    private void SetDescription()
    {
        // 기본 정보
        unitNameText.text = unit.unitData.unitName;
        unitHPText.text = $"{unit.maxHP}";
        unitUPText.text = $"{unit.stats.UP}";

        // 스탯
        unitAttackText.text = unit.stats.ATK.ToString();
        unitDefenseText.text = unit.stats.DEF.ToString();
        unitSpeedText.text = unit.stats.SPD.ToString();
        unitAGIText.text = unit.stats.AGI.ToString();

        // 퍼센트 스탯
        unitLSText.text = $"{unit.stats.LS * 100:F0}%";
        unitRSTText.text = $"{unit.stats.RST * 100:F0}%";
        unitEVText.text = $"{unit.stats.EV * 100:F0}%";
        unitCAText.text = $"{unit.stats.CA * 100:F0}%";
        unitDRText.text = $"{unit.stats.DR * 100:F0}%";
        unitAPText.text = unit.stats.AP.ToString();
    }
    void HideSkillArea()
    {
        skillAreaContainer.SetActive(false);
    }
    void ShowSkillArea(SkillData skillData)
    {
        skillAreaContainer.SetActive(true);

        foreach (var entry in skillAreaImageList)
        {
            if (entry.image != null)
            {
                if(entry.position == Vector2Int.zero)
                    entry.image.color = selfOriginColor;
                else
                    entry.image.color = Color.white;
            }
        }

        Color color;
        switch (skillData.targetType)
        {
            case targetType.Enemy:
                color = attackColor;
                break;
            case targetType.Ally:
                color = healColor;
                break;
            case targetType.Self:
                skillAreaImageList.Find(e => e.position == Vector2Int.zero).image.color = selfOriginColor;
                color = otherColor;
                break;
            default:
                color = otherColor;
                break;
        }

        if (skillData == null) return;

        // 스킬 범위에 해당하는 이미지 활성화
        foreach (Vector2Int pos in skillData.area)
        {
            var entry = skillAreaImageList.Find(e => e.position == pos);
            if (entry != null && entry.image != null)
            {
                entry.image.color = color;
            }
        }
    }
}