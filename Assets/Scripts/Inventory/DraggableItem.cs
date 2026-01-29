using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 드래그 앤 드롭 처리
/// 슬롯의 iconImage를 사용하여 드래그 표시
/// </summary>
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    /// <summary>
    /// 현재 드래그 중인 아이템 (static으로 전역 접근)
    /// </summary>
    public static DraggableItem currentlyDragging;

    [Header("Drag Settings")]
    [Range(0f, 1f)]
    public float dragAlpha = 0.5f; // 드래그 중 원본 불투명도

    private EquipmentData item;
    private SlotBase sourceSlot;
    private Canvas canvas;

    // 드래그 이미지 (마우스 따라다니는 복사본)
    private GameObject dragImageObject;
    private RectTransform dragImageRect;

    // 원본 아이콘 참조
    private Image slotIconImage;
    private Color originalIconColor;

    private bool dropSucceeded;

    /// <summary>
    /// 이 드래그 아이템의 장비 데이터
    /// </summary>
    public EquipmentData Item => item;

    /// <summary>
    /// 드래그가 시작된 슬롯
    /// </summary>
    public SlotBase SourceSlot => sourceSlot;

    /// <summary>
    /// 아이템 및 출발 슬롯 설정
    /// </summary>
    public void Setup(EquipmentData equipmentData, SlotBase slot, Canvas parentCanvas)
    {
        item = equipmentData;
        sourceSlot = slot;
        canvas = parentCanvas;

        // 슬롯의 iconImage 참조
        if (slot != null)
        {
            slotIconImage = slot.iconImage;
        }
    }

    /// <summary>
    /// 드래그 시작
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null) return;

        // 장비 슬롯에서 드래그 시작 시 씬 제한 체크
        if (sourceSlot is Equipment_Slot && !Inventory_Manager.CanModifyEquipment)
        {
            eventData.pointerDrag = null;
            return;
        }

        currentlyDragging = this;
        dropSucceeded = false;

        // 원본 아이콘 반투명하게
        if (slotIconImage != null)
        {
            originalIconColor = slotIconImage.color;
            Color c = slotIconImage.color;
            c.a = dragAlpha;
            slotIconImage.color = c;
        }

        // 드래그 이미지 생성
        CreateDragImage(eventData.position);
    }

    /// <summary>
    /// 드래그 중
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (item == null || dragImageRect == null) return;

        // 드래그 이미지가 마우스 따라다님
        dragImageRect.position = eventData.position;
    }

    /// <summary>
    /// 드래그 종료
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        currentlyDragging = null;

        // 드래그 이미지 제거
        if (dragImageObject != null)
        {
            Destroy(dragImageObject);
        }

        // 드롭 실패 시 원본 아이콘 복원
        if (!dropSucceeded && slotIconImage != null)
        {
            slotIconImage.color = originalIconColor;
        }
    }

    /// <summary>
    /// 드롭 성공 시 호출 (슬롯에서 호출)
    /// </summary>
    public void OnDropSuccess()
    {
        dropSucceeded = true;

        // 드래그 이미지 제거
        if (dragImageObject != null)
        {
            Destroy(dragImageObject);
        }

        // 드래그 아이템 오브젝트 제거 (슬롯에서 새로 생성됨)
        Destroy(gameObject);
    }

    /// <summary>
    /// 마우스를 따라다니는 드래그 이미지 생성
    /// </summary>
    private void CreateDragImage(Vector2 position)
    {
        if (canvas == null || slotIconImage == null || slotIconImage.sprite == null) return;

        // 새 게임오브젝트 생성
        dragImageObject = new GameObject("DragImage");
        dragImageObject.transform.SetParent(canvas.transform);
        dragImageObject.transform.SetAsLastSibling();

        // RectTransform 설정
        dragImageRect = dragImageObject.AddComponent<RectTransform>();
        dragImageRect.sizeDelta = slotIconImage.rectTransform.sizeDelta; // 원본 크기 사용
        dragImageRect.position = position;

        // Image 설정
        Image img = dragImageObject.AddComponent<Image>();
        img.sprite = slotIconImage.sprite;
        img.raycastTarget = false;

        // CanvasGroup
        CanvasGroup cg = dragImageObject.AddComponent<CanvasGroup>();
        cg.blocksRaycasts = false;
        cg.alpha = 0.9f;
    }
}
