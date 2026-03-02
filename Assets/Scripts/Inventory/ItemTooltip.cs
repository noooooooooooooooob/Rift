using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// 아이템 툴팁 UI
/// 마우스 위치에 따라 아이템 정보 표시
/// </summary>
public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip instance;

    [Header("UI References")]
    public GameObject tooltipPanel;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI descriptionText;

    [Header("Settings")]
    public Vector2 offset = new Vector2(10f, -10f);

    private RectTransform panelRect;
    private Canvas canvas;

    void Awake()
    {
        // 이미 instance가 있고 파괴되지 않았으면 중복 방지
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        if (tooltipPanel != null)
        {
            panelRect = tooltipPanel.GetComponent<RectTransform>();
            tooltipPanel.SetActive(false);
        }

        canvas = GetComponentInParent<Canvas>();
    }

    void Update()
    {
        if (tooltipPanel != null && tooltipPanel.activeSelf)
        {
            UpdatePosition();
        }
    }

    /// <summary>
    /// 툴팁 표시
    /// </summary>
    public void Show(EquipmentData item)
    {
        if (item == null || tooltipPanel == null) return;

        // 이름 설정
        if (nameText != null)
        {
            nameText.text = item.equipmentName;
        }

        // 설명 설정
        if (descriptionText != null)
        {
            descriptionText.text = item.description;
        }

        tooltipPanel.SetActive(true);
        UpdatePosition();
    }

    /// <summary>
    /// 툴팁 숨기기
    /// </summary>
    public void Hide()
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 마우스 위치에 따라 툴팁 위치 업데이트
    /// </summary>
    private void UpdatePosition()
    {
        if (panelRect == null || canvas == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        // 화면 경계 체크하여 툴팁이 화면 밖으로 나가지 않도록
        float pivotX = 0f;
        float pivotY = 1f;

        // 오른쪽 경계 체크
        if (mousePos.x + panelRect.rect.width > Screen.width)
        {
            pivotX = 1f;
        }

        // 하단 경계 체크
        if (mousePos.y - panelRect.rect.height < 0)
        {
            pivotY = 0f;
        }

        panelRect.pivot = new Vector2(pivotX, pivotY);
        panelRect.position = mousePos + offset;
    }
}
