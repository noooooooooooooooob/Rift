using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI 버튼 클릭 시 유닛을 Party_Manager에 추가/제거하는 컴포넌트
/// 클릭 시 선택/해제 토글, 최대 3명까지 선택 가능
/// </summary>
public class UnitRecruitButton : MonoBehaviour
{
    public Button button;
    [Header("Unit Configuration")]
    [Tooltip("추가할 유닛의 프리팹 (예: Serian.prefab)")]
    public GameObject unitPrefab;
    private GameObject spawnedUnit;  // 파티에 추가된 유닛 인스턴스

    [Header("Visual Feedback (Optional)")]
    [Tooltip("선택 시 표시할 이미지 (체크마크 등)")]
    public Sprite selectedIndicator;
    public Sprite portraitImage;
    private const int MAX_PARTY_SIZE = 3;
    private bool isSelected = false;
    void Start()
    {
        button.onClick.AddListener(OnButtonClicked);
        UpdateVisualState();
    }
    /// <summary>
    /// 버튼 클릭 시 호출: 선택/해제 토글
    /// </summary>
    void OnButtonClicked()
    {
        if (Party_Manager.instance == null)
        {
            Debug.LogError("Party_Manager.instance가 null입니다!");
            return;
        }

        if (unitPrefab == null)
        {
            Debug.LogError($"{gameObject.name}: unitPrefab이 설정되지 않았습니다!");
            return;
        }

        if (isSelected)
        {
            RemoveFromParty();
        }
        else
        {
            AddToParty();
        }
    }

    /// <summary>
    /// 파티에 유닛 추가
    /// </summary>
    void AddToParty()
    {
        // 파티 인원 제한 확인
        if (Party_Manager.instance.GetPartyCount() >= MAX_PARTY_SIZE)
        {
            Debug.LogWarning($"파티 인원이 최대({MAX_PARTY_SIZE}명)에 도달했습니다!");
            return;
        }

        // 유닛 생성
        spawnedUnit = Instantiate(unitPrefab);
        spawnedUnit.name = unitPrefab.name;
        spawnedUnit.SetActive(false);  // 씬에 보이지 않도록 비활성화

        // Party_Manager에 추가
        Party_Manager.instance.AddUnitToParty(spawnedUnit);

        // 상태 업데이트
        isSelected = true;
        UpdateVisualState();
    }

    /// <summary>
    /// 파티에서 유닛 제거
    /// </summary>
    void RemoveFromParty()
    {
        if (spawnedUnit != null)
        {
            Party_Manager.instance.RemoveUnitFromParty(spawnedUnit);
            spawnedUnit = null;
        }

        // 상태 업데이트
        isSelected = false;
        UpdateVisualState();
    }

    /// <summary>
    /// 선택 상태에 따라 UI 업데이트
    /// </summary>
    void UpdateVisualState()
    {
        // 선택 표시 아이콘
        if (isSelected)
        {
            button.image.sprite = selectedIndicator;
        }
        else
        {
            button.image.sprite = portraitImage;
        }
    }

    /// <summary>
    /// 외부에서 상태 초기화 (파티 초기화 시 호출)
    /// </summary>
    public void ResetButton()
    {
        if (isSelected && spawnedUnit != null)
        {
            // Party_Manager에서 제거하지 않고 로컬 상태만 초기화
            // (Party_Manager.ClearParty()가 이미 유닛을 파괴함)
            spawnedUnit = null;
        }

        isSelected = false;
        UpdateVisualState();
    }

    /// <summary>
    /// 현재 선택 상태 반환
    /// </summary>
    public bool IsSelected()
    {
        return isSelected;
    }
}
