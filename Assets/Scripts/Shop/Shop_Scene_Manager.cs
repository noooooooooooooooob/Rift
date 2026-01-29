using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// 상점 씬 관리
/// 장비 목록 표시 및 구매 처리
/// </summary>
public class Shop_Scene_Manager : MonoBehaviour
{
    [Header("Shop Items")]
    public List<EquipmentData> shopItems = new List<EquipmentData>();

    [Header("UI References")]
    public ShopItemUI[] shopItemUIs;
    public TextMeshProUGUI moneyText;
    public Button continueButton;

    [Header("Settings")]
    public string equipmentResourcePath = "Equipment";
    public int rerollCost = 50;

    private EquipmentData[] allEquipments;

    void Awake()
    {
        allEquipments = Resources.LoadAll<EquipmentData>(equipmentResourcePath);
        Debug.Log($"상점: 총 {allEquipments.Length}개 장비 발견");
        continueButton.onClick.AddListener(() =>
        {
            Stage_Manager.instance.CompleteCurrentNode();
        });
    }

    void Start()
    {
        // 랜덤으로 장비 선택
        SelectRandomItems();
        RefreshShop();
        UpdateMoneyDisplay();
    }

    /// <summary>
    /// 랜덤으로 상점 아이템 선택
    /// </summary>
    private void SelectRandomItems()
    {
        shopItems.Clear();

        if (allEquipments == null || allEquipments.Length == 0) return;
        if (shopItemUIs == null || shopItemUIs.Length == 0) return;

        // 사용 가능한 인덱스 리스트
        List<int> availableIndices = new List<int>();
        for (int i = 0; i < allEquipments.Length; i++)
        {
            availableIndices.Add(i);
        }

        // UI 슬롯 개수만큼 랜덤 선택
        int selectCount = Mathf.Min(shopItemUIs.Length, allEquipments.Length);
        for (int i = 0; i < selectCount; i++)
        {
            int randomIndex = Random.Range(0, availableIndices.Count);
            int equipmentIndex = availableIndices[randomIndex];

            shopItems.Add(allEquipments[equipmentIndex]);
            availableIndices.RemoveAt(randomIndex); // 중복 방지
        }

        Debug.Log($"상점: {shopItems.Count}개 아이템 랜덤 선택됨");
    }

    /// <summary>
    /// 상점 UI 갱신
    /// </summary>
    public void RefreshShop()
    {
        if (shopItemUIs == null) return;

        for (int i = 0; i < shopItemUIs.Length; i++)
        {
            if (shopItemUIs[i] == null) continue;

            if (i < shopItems.Count && shopItems[i] != null)
            {
                shopItemUIs[i].Setup(shopItems[i], this);
                shopItemUIs[i].gameObject.SetActive(true);
            }
            else
            {
                shopItemUIs[i].gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 아이템 구매 시도
    /// </summary>
    public bool TryBuyItem(EquipmentData item)
    {
        if (item == null) return false;
        if (Party_Manager.instance == null) return false;
        if (Inventory_Manager.instance == null) return false;

        // 돈 확인
        if (Party_Manager.instance.money < item.price)
        {
            Debug.Log($"돈이 부족합니다! 필요: {item.price}, 보유: {Party_Manager.instance.money}");
            return false;
        }

        // 인벤토리 공간 확인
        if (Inventory_Manager.instance.IsFull())
        {
            Debug.Log("인벤토리가 가득 찼습니다!");
            return false;
        }

        // 구매 처리
        Party_Manager.instance.money -= item.price;
        Inventory_Manager.instance.AddItem(item);

        // 상점에서 제거
        shopItems.Remove(item);
        RefreshShop();

        Debug.Log($"{item.equipmentName} 구매 완료! 남은 돈: {Party_Manager.instance.money}");

        UpdateMoneyDisplay();
        return true;
    }

    /// <summary>
    /// 상점 아이템 리롤 (버튼에 연결)
    /// </summary>
    public void Reroll()
    {
        if (Party_Manager.instance == null) return;

        // 비용 확인
        if (Party_Manager.instance.money < rerollCost)
        {
            Debug.Log($"리롤 비용이 부족합니다! 필요: {rerollCost}, 보유: {Party_Manager.instance.money}");
            return;
        }

        // 비용 차감
        Party_Manager.instance.money -= rerollCost;

        SelectRandomItems();
        RefreshShop();
        UpdateMoneyDisplay();
        Debug.Log($"상점 리롤! 남은 돈: {Party_Manager.instance.money}");
    }

    /// <summary>
    /// 돈 표시 갱신
    /// </summary>
    public void UpdateMoneyDisplay()
    {
        if (moneyText != null && Party_Manager.instance != null)
        {
            moneyText.text = $"{Party_Manager.instance.money} G";
        }
    }
}
