using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 모든 슬롯의 기본 클래스
/// 드롭 가능 영역으로 동작하며, 아이템 표시 및 하이라이트 처리
/// </summary>
public abstract class Slot_Base : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    [SerializeField] protected Image slotBackground;
    [SerializeField] protected Image itemIcon;

    [Header("Visual Settings")]
    [SerializeField] protected Color normalColor = Color.white;
    [SerializeField] protected Color highlightColor = Color.yellow;
    [SerializeField] protected Color invalidColor = Color.red;

    public EquipmentData CurrentEquipment { get; protected set; }

    private DraggableItem draggableItem;

    public DraggableItem DraggableItem => draggableItem;

    /// <summary>
    /// 드롭 가능 여부 검사 (자식 클래스에서 구현)
    /// </summary>
    public abstract bool CanAcceptItem(EquipmentData equipment);

    /// <summary>
    /// 아이템 설정
    /// </summary>
    public virtual void SetItem(EquipmentData equipment, DraggableItem item = null)
    {
        CurrentEquipment = equipment;
        draggableItem = item;

        if (item != null)
        {
            item.SetSlot(this);
        }

        UpdateVisual();
    }

    /// <summary>
    /// 아이템 제거
    /// </summary>
    public virtual void ClearItem()
    {
        CurrentEquipment = null;
        draggableItem = null;
        UpdateVisual();
    }

    protected virtual void UpdateVisual()
    {
        if (itemIcon != null)
        {
            if (CurrentEquipment != null && CurrentEquipment.icon != null)
            {
                itemIcon.sprite = CurrentEquipment.icon;
                itemIcon.enabled = true;
            }
            else
            {
                itemIcon.enabled = false;
            }
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        DraggableItem draggedItem = eventData.pointerDrag?.GetComponent<DraggableItem>();
        if (draggedItem == null || draggedItem.Equipment == null) return;

        if (CanAcceptItem(draggedItem.Equipment))
        {
            Inventory_Manager.instance.HandleItemDrop(draggedItem.SourceSlot, this, draggedItem);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        DraggableItem draggedItem = eventData.pointerDrag.GetComponent<DraggableItem>();
        if (draggedItem != null && draggedItem.Equipment != null)
        {
            bool canAccept = CanAcceptItem(draggedItem.Equipment);
            if (slotBackground != null)
            {
                slotBackground.color = canAccept ? highlightColor : invalidColor;
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (slotBackground != null)
        {
            slotBackground.color = normalColor;
        }
    }
}
