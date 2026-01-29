using UnityEngine;

/// <summary>
/// 유닛별 장비 상태 관리
/// 무기 1개 + 방어구 3개 (범용 슬롯)
/// </summary>
public class UnitEquipment : MonoBehaviour
{
    [Header("Equipment")]
    public EquipmentData weapon;
    public EquipmentData[] armors = new EquipmentData[3];

    private Unit unit;

    void Awake()
    {
        unit = GetComponent<Unit>();

        // armors 배열이 null이거나 크기가 다르면 초기화
        if (armors == null || armors.Length != 3)
        {
            armors = new EquipmentData[3];
        }
    }

    /// <summary>
    /// 장비 장착
    /// </summary>
    /// <param name="item">장착할 장비</param>
    /// <param name="armorIndex">방어구 슬롯 인덱스 (0-2), 무기는 무시됨</param>
    /// <returns>장착 성공 여부</returns>
    public bool Equip(EquipmentData item, int armorIndex = 0)
    {
        if (item == null) return false;

        if (item.slot == EquipmentSlot.Weapon)
        {
            if (weapon != null) return false; // 이미 무기가 있으면 실패
            weapon = item;
            item.ApplyEffects(unit);
            return true;
        }
        else // Armor
        {
            if (armorIndex < 0 || armorIndex >= armors.Length) return false;
            if (armors[armorIndex] != null) return false; // 이미 방어구가 있으면 실패
            armors[armorIndex] = item;
            item.ApplyEffects(unit);
            return true;
        }
    }

    /// <summary>
    /// 장비 해제
    /// </summary>
    /// <param name="type">슬롯 타입</param>
    /// <param name="armorIndex">방어구 슬롯 인덱스 (0-2)</param>
    /// <returns>해제된 장비 (없으면 null)</returns>
    public EquipmentData Unequip(EquipmentSlot type, int armorIndex = 0)
    {
        EquipmentData removed = null;

        if (type == EquipmentSlot.Weapon)
        {
            removed = weapon;
            if (removed != null)
            {
                removed.RemoveEffects(unit);
                weapon = null;
            }
        }
        else // Armor
        {
            if (armorIndex < 0 || armorIndex >= armors.Length) return null;
            removed = armors[armorIndex];
            if (removed != null)
            {
                removed.RemoveEffects(unit);
                armors[armorIndex] = null;
            }
        }

        return removed;
    }

    /// <summary>
    /// 장비 스왑 (기존 장비를 반환하고 새 장비 장착)
    /// </summary>
    /// <param name="newItem">새로 장착할 장비</param>
    /// <param name="type">슬롯 타입</param>
    /// <param name="armorIndex">방어구 슬롯 인덱스 (0-2)</param>
    /// <returns>기존에 장착되어 있던 장비 (없으면 null)</returns>
    public EquipmentData Swap(EquipmentData newItem, EquipmentSlot type, int armorIndex = 0)
    {
        if (newItem == null) return null;
        if (newItem.slot != type) return null; // 타입 불일치

        EquipmentData oldItem = Unequip(type, armorIndex);

        if (type == EquipmentSlot.Weapon)
        {
            weapon = newItem;
            newItem.ApplyEffects(unit);
        }
        else
        {
            if (armorIndex < 0 || armorIndex >= armors.Length) return oldItem;
            armors[armorIndex] = newItem;
            newItem.ApplyEffects(unit);
        }

        return oldItem;
    }

    /// <summary>
    /// 특정 슬롯의 장비 가져오기
    /// </summary>
    public EquipmentData GetEquipment(EquipmentSlot type, int armorIndex = 0)
    {
        if (type == EquipmentSlot.Weapon)
            return weapon;

        if (armorIndex < 0 || armorIndex >= armors.Length)
            return null;

        return armors[armorIndex];
    }

    /// <summary>
    /// 특정 슬롯이 비어있는지 확인
    /// </summary>
    public bool IsSlotEmpty(EquipmentSlot type, int armorIndex = 0)
    {
        return GetEquipment(type, armorIndex) == null;
    }
}
