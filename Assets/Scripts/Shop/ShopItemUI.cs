using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// 상점 아이템 UI
/// 아이템 정보 표시 및 구매 버튼 처리
/// </summary>
public class ShopItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI priceText;
    public Button buyButton;

    private EquipmentData item;
    private Shop_Scene_Manager shopManager;

    /// <summary>
    /// 아이템 정보 설정
    /// </summary>
    public void Setup(EquipmentData equipmentData, Shop_Scene_Manager manager)
    {
        item = equipmentData;
        shopManager = manager;

        if (item == null) return;

        // UI 업데이트
        if (iconImage != null && item.icon != null)
        {
            iconImage.sprite = item.icon;
            iconImage.enabled = true;
        }

        if (nameText != null)
        {
            nameText.text = item.equipmentName;
        }

        if (priceText != null)
        {
            priceText.text = $"{item.price}";
        }

        // 구매 버튼 이벤트 연결
        if (buyButton != null)
        {
            buyButton.onClick.RemoveAllListeners();
            buyButton.onClick.AddListener(OnBuyClicked);
        }
    }

    /// <summary>
    /// 구매 버튼 클릭
    /// </summary>
    private void OnBuyClicked()
    {
        if (shopManager != null && item != null)
        {
            shopManager.TryBuyItem(item);
        }
    }

    /// <summary>
    /// 마우스 진입 시 툴팁 표시
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null && ItemTooltip.instance != null)
        {
            ItemTooltip.instance.Show(item);
        }
    }

    /// <summary>
    /// 마우스 이탈 시 툴팁 숨기기
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        if (ItemTooltip.instance != null)
        {
            ItemTooltip.instance.Hide();
        }
    }
}
