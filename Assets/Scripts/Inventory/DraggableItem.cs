using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// 드래그 가능한 아이템 컴포넌트
/// 슬롯의 자식으로 존재하며, 드래그 시 Canvas 최상위로 이동
/// </summary>
public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("References")]
    [SerializeField] private Image itemImage;

    [Header("Animation Settings")]
    [SerializeField] private float dragAlpha = 0.6f;
    [SerializeField] private float returnDuration = 0.2f;

    public EquipmentData Equipment { get; private set; }
    public Slot_Base SourceSlot { get; private set; }

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private Transform originalParent;
    private Vector3 originalPosition;
    private int originalSiblingIndex;
    private bool isDragging;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void Initialize(Slot_Base slot, EquipmentData equipment, Canvas rootCanvas)
    {
        SourceSlot = slot;
        Equipment = equipment;
        canvas = rootCanvas;

        if (itemImage != null && equipment != null && equipment.icon != null)
        {
            itemImage.sprite = equipment.icon;
            itemImage.enabled = true;
        }
    }

    public void SetSlot(Slot_Base slot)
    {
        SourceSlot = slot;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Equipment == null) return;

        isDragging = true;

        // 원래 위치 저장
        originalParent = transform.parent;
        originalPosition = rectTransform.anchoredPosition;
        originalSiblingIndex = transform.GetSiblingIndex();

        // Canvas 최상위로 이동 (다른 UI 위에 그려지도록)
        transform.SetParent(canvas.transform);
        transform.SetAsLastSibling();

        // 투명도 조절 및 레이캐스트 비활성화
        canvasGroup.alpha = dragAlpha;
        canvasGroup.blocksRaycasts = false;

        // 스케일 애니메이션
        rectTransform.DOScale(1.1f, 0.1f).SetEase(Ease.OutBack);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Equipment == null || !isDragging) return;

        // 마우스 위치 따라 이동
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isDragging) return;

        isDragging = false;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // 드롭이 처리되지 않은 경우 원래 위치로 복귀
        if (transform.parent == canvas.transform)
        {
            ReturnToOriginalPosition();
        }

        // 스케일 복귀
        rectTransform.DOScale(1f, 0.1f).SetEase(Ease.OutBack);
    }

    public void ReturnToOriginalPosition()
    {
        transform.SetParent(originalParent);
        transform.SetSiblingIndex(originalSiblingIndex);

        rectTransform.DOAnchorPos(originalPosition, returnDuration)
            .SetEase(Ease.OutBack);
    }

    public void MoveToSlot(Slot_Base newSlot)
    {
        transform.SetParent(newSlot.transform);
        SourceSlot = newSlot;

        rectTransform.DOAnchorPos(Vector2.zero, returnDuration)
            .SetEase(Ease.OutBack);
    }
}
