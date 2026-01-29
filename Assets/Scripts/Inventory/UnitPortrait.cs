using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 유닛 초상화 + 장비 슬롯 + HP 바 UI
/// 각 유닛마다 장비 슬롯을 직접 포함
/// </summary>
public class UnitPortrait : MonoBehaviour
{
    [Header("References")]
    public Image portraitImage;

    [Header("HP Bar")]
    public Image hpBarFill;

    [Header("Equipment Slots")]
    public Equipment_Slot weaponSlot;
    public Equipment_Slot[] armorSlots = new Equipment_Slot[3];

    private int unitIndex;
    private Unit unit;
    private UnitEquipment unitEquipment;

    /// <summary>
    /// 초상화 초기화
    /// </summary>
    public void Setup(int index, Unit targetUnit)
    {
        unitIndex = index;

        if (targetUnit != null)
        {
            unit = targetUnit;
            unitEquipment = unit.GetComponent<UnitEquipment>();

            // 초상화 이미지 설정 (UnitData에서 가져오기)
            if (portraitImage != null && unit.unitData != null && unit.unitData.unitPortrait != null)
            {
                portraitImage.sprite = unit.unitData.unitPortrait;
                portraitImage.enabled = true;
            }

            // HP 바 갱신
            RefreshHP();

            // 장비 슬롯에 유닛 연결
            SetupEquipmentSlots();

            gameObject.SetActive(true);
        }
        else
        {
            unit = null;
            unitEquipment = null;
            ClearEquipmentSlots();
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// HP 바 갱신
    /// </summary>
    public void RefreshHP()
    {
        if (hpBarFill == null || unit == null) return;

        float hpRatio = unit.maxHP > 0 ? (float)unit.currentHP / unit.maxHP : 0f;
        hpBarFill.fillAmount = Mathf.Clamp01(hpRatio);
    }

    /// <summary>
    /// 장비 슬롯 설정
    /// </summary>
    private void SetupEquipmentSlots()
    {
        if (weaponSlot != null)
        {
            weaponSlot.slotType = EquipmentSlot.Weapon;
            weaponSlot.SetTargetEquipment(unitEquipment);
            Inventory_Manager.instance?.CreateDraggableForSlot(weaponSlot);
        }

        for (int i = 0; i < armorSlots.Length; i++)
        {
            if (armorSlots[i] != null)
            {
                armorSlots[i].slotType = EquipmentSlot.Armor;
                armorSlots[i].armorIndex = i;
                armorSlots[i].SetTargetEquipment(unitEquipment);
                Inventory_Manager.instance?.CreateDraggableForSlot(armorSlots[i]);
            }
        }
    }

    /// <summary>
    /// 장비 슬롯 초기화
    /// </summary>
    private void ClearEquipmentSlots()
    {
        if (weaponSlot != null)
        {
            weaponSlot.ClearSlot();
        }

        for (int i = 0; i < armorSlots.Length; i++)
        {
            if (armorSlots[i] != null)
            {
                armorSlots[i].ClearSlot();
            }
        }
    }

    /// <summary>
    /// 빈 슬롯으로 설정
    /// </summary>
    public void SetEmpty()
    {
        unit = null;
        unitEquipment = null;
        if (portraitImage != null)
        {
            portraitImage.sprite = null;
            portraitImage.enabled = false;
        }
        if (hpBarFill != null)
        {
            hpBarFill.fillAmount = 0f;
        }
        ClearEquipmentSlots();
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 장비 슬롯 DraggableItem 갱신
    /// </summary>
    public void RefreshDraggables()
    {
        if (weaponSlot != null)
        {
            Inventory_Manager.instance?.CreateDraggableForSlot(weaponSlot);
        }

        for (int i = 0; i < armorSlots.Length; i++)
        {
            if (armorSlots[i] != null)
            {
                Inventory_Manager.instance?.CreateDraggableForSlot(armorSlots[i]);
            }
        }
    }

    /// <summary>
    /// 이 초상화의 Unit
    /// </summary>
    public Unit Unit => unit;

    /// <summary>
    /// 이 초상화의 UnitEquipment
    /// </summary>
    public UnitEquipment UnitEquipment => unitEquipment;

    /// <summary>
    /// 유닛 인덱스
    /// </summary>
    public int UnitIndex => unitIndex;
}
