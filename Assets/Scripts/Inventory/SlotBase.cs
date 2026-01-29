using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 슬롯 추상 기본 클래스
/// 인벤토리 슬롯과 장비 슬롯의 공통 기능 정의
/// </summary>
public abstract class SlotBase : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    public Image iconImage;
    public Image highlightImage;

    [Header("Colors")]
    public Color normalColor = Color.white;
    public Color highlightColor = new Color(1f, 1f, 0.5f, 1f);
    public Color invalidColor = new Color(1f, 0.5f, 0.5f, 1f);

    protected EquipmentData currentItem;

    /// <summary>
    /// 현재 슬롯의 아이템
    /// </summary>
    public EquipmentData CurrentItem => currentItem;

    /// <summary>
    /// 슬롯이 비어있는지 확인
    /// </summary>
    public bool IsEmpty => currentItem == null;

    protected virtual void Start()
    {
        UpdateVisual();

        // 하이라이트 기본 비활성화
        if (highlightImage != null)
        {
            highlightImage.enabled = false;
        }
    }

    /// <summary>
    /// 아이템 드롭 처리
    /// </summary>
    public abstract void OnDrop(PointerEventData eventData);

    /// <summary>
    /// 아이템 수용 가능 여부 확인
    /// </summary>
    public abstract bool CanAcceptItem(EquipmentData item);

    /// <summary>
    /// 슬롯에 아이템 설정
    /// </summary>
    public virtual void SetItem(EquipmentData item)
    {
        currentItem = item;
        UpdateVisual();
    }

    /// <summary>
    /// 슬롯에서 아이템 제거
    /// </summary>
    public virtual EquipmentData ClearItem()
    {
        EquipmentData removed = currentItem;
        currentItem = null;
        UpdateVisual();
        return removed;
    }

    /// <summary>
    /// UI 시각 업데이트
    /// </summary>
    protected virtual void UpdateVisual()
    {
        if (iconImage != null)
        {
            if (currentItem != null && currentItem.icon != null)
            {
                iconImage.sprite = currentItem.icon;
                iconImage.enabled = true;
            }
            else
            {
                iconImage.sprite = null;
                iconImage.enabled = false;
            }
        }
    }

    /// <summary>
    /// 마우스 진입 시 하이라이트 및 툴팁 표시
    /// </summary>
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (highlightImage != null)
        {
            // 드래그 중인 아이템이 있으면 수용 가능 여부에 따라 색상 변경
            DraggableItem dragging = DraggableItem.currentlyDragging;
            if (dragging != null)
            {
                if (CanAcceptItem(dragging.Item))
                    highlightImage.color = highlightColor;
                else
                    highlightImage.color = invalidColor;
            }
            else
            {
                highlightImage.color = highlightColor;
            }
            highlightImage.enabled = true;
        }

        // 드래그 중이 아니고 아이템이 있으면 툴팁 표시
        if (DraggableItem.currentlyDragging == null && currentItem != null)
        {
            if (ItemTooltip.instance != null)
            {
                ItemTooltip.instance.Show(currentItem);
            }
        }
    }

    /// <summary>
    /// 마우스 이탈 시 하이라이트 해제 및 툴팁 숨기기
    /// </summary>
    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (highlightImage != null)
        {
            highlightImage.color = normalColor;
            highlightImage.enabled = false;
        }

        // 툴팁 숨기기
        if (ItemTooltip.instance != null)
        {
            ItemTooltip.instance.Hide();
        }
    }
}
