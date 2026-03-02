using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Rest_Scene_Manager : MonoBehaviour
{
    public Button continueButton;
    public List<Transform> unitPositions;
    float oneHealPercent = 0.7f;
    float allHealPercent = 0.3f;
    List<Unit> partyUnits = new List<Unit>();
    private bool isSelectingUnit = false;

    void Start()
    {
        continueButton.onClick.AddListener(() =>
        {
            Stage_Manager.instance.CompleteCurrentNode();
        });
        partyUnits = Party_Manager.instance.GetPartyUnits();
        for (int i = 0; i < partyUnits.Count; i++)
        {
            Unit unit = partyUnits[i];
            unit.transform.position = unitPositions[i].position;
            unit.gameObject.SetActive(true);
        }
    }

    public void RestOne()
    {
        isSelectingUnit = true;

        // 유닛들에 콜백 등록
        foreach (Unit unit in partyUnits)
        {
            var clickable = unit.GetComponent<UnitClickable>();
            clickable.onClickCallback = OnUnitSelected;
        }
    }

    private void OnUnitSelected(Unit unit)
    {
        if (!isSelectingUnit) return;

        unit.Heal((int)(unit.maxHP * oneHealPercent));
        isSelectingUnit = false;

        // 콜백 해제
        foreach (Unit u in partyUnits)
        {
            u.GetComponent<UnitClickable>().onClickCallback = null;
        }
    }

    public void RestAll()
    {
        Party_Manager.instance.HealPartyPercent(allHealPercent);
    }
}
