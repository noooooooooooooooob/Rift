using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// 인벤토리 시스템 통합 관리 (데이터 + UI)
/// Tab 키로 토글, 슬롯 생성/갱신 및 DraggableItem 인스턴스 관리
/// DontDestroyOnLoad로 씬 전환 시 유지
/// </summary>
public class Inventory_Manager : MonoBehaviour
{
    public static Inventory_Manager instance;

    public const int INVENTORY_SIZE = 9; // 3x3

    [Header("Inventory Data")]
    public EquipmentData[] items = new EquipmentData[INVENTORY_SIZE];

    [Header("References")]
    public Canvas canvas;
    public Transform inventorySlotContainer;
    public GameObject inventoryPanel;

    [Header("Prefabs")]
    public GameObject draggableItemPrefab;

    [Header("Unit Portraits")]
    public UnitPortrait[] unitPortraits = new UnitPortrait[3];

    [Header("Scene Restrictions")]
    [Tooltip("장비 교체가 허용되는 씬 이름 목록. 비어있으면 모든 씬에서 허용")]
    public string[] allowedScenes;

    [Header("State")]
    private Inventory_Slot[] inventorySlots;
    private bool isOpen;

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Start Scene")  // 씬 이름 확인
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 현재 씬에서 장비 교체가 가능한지 확인
    /// </summary>
    public static bool CanModifyEquipment
    {
        get
        {
            if (instance == null) return false;
            if (instance.allowedScenes == null || instance.allowedScenes.Length == 0)
                return true; // 제한 없음

            string currentScene = SceneManager.GetActiveScene().name;
            foreach (string sceneName in instance.allowedScenes)
            {
                if (sceneName == currentScene)
                    return true;
            }
            return false;
        }
    }

    void Awake()
    {
        // 싱글톤 + DontDestroyOnLoad
        if (instance == null)
        {
            instance = this;
            SceneManager.sceneLoaded += OnSceneLoaded;
            // Canvas를 DontDestroyOnLoad로 설정
            if (canvas != null)
                DontDestroyOnLoad(canvas.gameObject);
            else
                DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 중복 시 Canvas도 함께 파괴
            if (canvas != null)
                Destroy(canvas.gameObject);
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        InitializeInventorySlots();

        // 시작 시 닫힌 상태
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);
        isOpen = false;
    }

    void Update()
    {
        // Tab 키로 인벤토리 토글
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            Toggle();
        }
    }

    #region Inventory Data Management

    /// <summary>
    /// 인벤토리에 아이템 추가 (빈 슬롯 자동 탐색)
    /// </summary>
    /// <returns>추가된 슬롯 인덱스, 실패시 -1</returns>
    public int AddItem(EquipmentData item)
    {
        if (item == null) return -1;

        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null)
            {
                items[i] = item;
                return i;
            }
        }

        Debug.LogWarning("Inventory is full!");
        return -1;
    }

    /// <summary>
    /// 특정 슬롯에 아이템 설정
    /// </summary>
    public bool SetItem(int index, EquipmentData item)
    {
        if (index < 0 || index >= items.Length) return false;
        items[index] = item;
        return true;
    }

    /// <summary>
    /// 특정 슬롯의 아이템 가져오기
    /// </summary>
    public EquipmentData GetItem(int index)
    {
        if (index < 0 || index >= items.Length) return null;
        return items[index];
    }

    /// <summary>
    /// 특정 슬롯의 아이템 제거
    /// </summary>
    public EquipmentData RemoveItem(int index)
    {
        if (index < 0 || index >= items.Length) return null;

        EquipmentData removed = items[index];
        items[index] = null;
        return removed;
    }

    /// <summary>
    /// 특정 슬롯이 비어있는지 확인
    /// </summary>
    public bool IsSlotEmpty(int index)
    {
        if (index < 0 || index >= items.Length) return false;
        return items[index] == null;
    }

    /// <summary>
    /// 빈 슬롯 개수 반환
    /// </summary>
    public int GetEmptySlotCount()
    {
        int count = 0;
        for (int i = 0; i < items.Length; i++)
        {
            if (items[i] == null) count++;
        }
        return count;
    }

    /// <summary>
    /// 인벤토리가 가득 찼는지 확인
    /// </summary>
    public bool IsFull()
    {
        return GetEmptySlotCount() == 0;
    }

    /// <summary>
    /// 인벤토리 초기화
    /// </summary>
    public void ClearInventory()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i] = null;
        }
    }

    #endregion

    #region UI Slot Management

    /// <summary>
    /// 인벤토리 슬롯 초기화 (9칸)
    /// </summary>
    private void InitializeInventorySlots()
    {
        inventorySlots = inventorySlotContainer.GetComponentsInChildren<Inventory_Slot>();

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            inventorySlots[i].slotIndex = i;
        }
    }

    /// <summary>
    /// 인벤토리 UI 갱신
    /// </summary>
    public void RefreshInventory()
    {
        if (inventorySlots == null) return;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i] != null)
            {
                inventorySlots[i].RefreshFromManager();
                CreateDraggableForSlot(inventorySlots[i]);
            }
        }
    }

    #endregion

    #region Unit Portrait Management

    /// <summary>
    /// 유닛 초상화 갱신 (Party_Manager에서 유닛 가져오기)
    /// </summary>
    public void RefreshUnitPortraits()
    {
        if (Party_Manager.instance == null) return;

        var partyUnits = Party_Manager.instance.GetPartyUnits();

        for (int i = 0; i < unitPortraits.Length; i++)
        {
            if (unitPortraits[i] == null) continue;

            if (i < partyUnits.Count && partyUnits[i] != null)
            {
                unitPortraits[i].Setup(i, partyUnits[i]);
            }
            else
            {
                unitPortraits[i].SetEmpty();
            }
        }
    }

    #endregion

    /// <summary>
    /// 슬롯에 드래그 가능한 아이템 생성
    /// </summary>
    public void CreateDraggableForSlot(SlotBase slot)
    {
        if (slot == null) return;

        // 기존 드래그 아이템 제거
        ClearDraggableInSlot(slot);

        // 아이템이 없으면 생성하지 않음
        if (slot.CurrentItem == null) return;

        if (draggableItemPrefab == null)
        {
            Debug.LogWarning("DraggableItem prefab is not assigned!");
            return;
        }

        // 새 드래그 아이템 생성
        GameObject draggableGO = Instantiate(draggableItemPrefab, slot.transform);
        DraggableItem draggable = draggableGO.GetComponent<DraggableItem>();

        if (draggable != null)
        {
            draggable.Setup(slot.CurrentItem, slot, canvas);

            // DraggableItem 프리팹의 Image를 투명하게 (슬롯의 iconImage가 아이콘 표시)
            // Image 컴포넌트는 드래그 이벤트 수신을 위해 활성화 상태 유지 필요
            Image img = draggableGO.GetComponent<Image>();
            if (img != null)
            {
                img.color = new Color(1f, 1f, 1f, 0f);
            }

            // RectTransform 설정 (슬롯 중앙에 배치)
            RectTransform rt = draggableGO.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = Vector2.zero;
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
            }
        }
    }

    /// <summary>
    /// 슬롯의 드래그 아이템 제거
    /// </summary>
    private void ClearDraggableInSlot(SlotBase slot)
    {
        if (slot == null) return;

        DraggableItem[] draggables = slot.GetComponentsInChildren<DraggableItem>();
        foreach (var d in draggables)
        {
            Destroy(d.gameObject);
        }
    }

    /// <summary>
    /// 모든 슬롯의 드래그 아이템 갱신
    /// </summary>
    public void RefreshAllDraggables()
    {
        // 인벤토리 슬롯
        if (inventorySlots != null)
        {
            foreach (var slot in inventorySlots)
            {
                if (slot != null)
                {
                    CreateDraggableForSlot(slot);
                }
            }
        }

        // 유닛 초상화의 장비 슬롯
        foreach (var portrait in unitPortraits)
        {
            if (portrait != null)
            {
                portrait.RefreshDraggables();
            }
        }
    }

    /// <summary>
    /// 인벤토리 UI 열기
    /// </summary>
    public void Open()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(true);

        isOpen = true;
        RefreshInventory();
        RefreshUnitPortraits();
    }

    /// <summary>
    /// 인벤토리 UI 닫기
    /// </summary>
    public void Close()
    {
        if (inventoryPanel != null)
            inventoryPanel.SetActive(false);

        isOpen = false;
    }

    /// <summary>
    /// 인벤토리 UI 토글
    /// </summary>
    public void Toggle()
    {
        // 토글 시 툴팁 숨기기
        if (ItemTooltip.instance != null)
            ItemTooltip.instance.Hide();

        if (isOpen)
            Close();
        else
            Open();
    }

    /// <summary>
    /// 인벤토리가 열려있는지 확인
    /// </summary>
    public bool IsOpen => isOpen;
}
