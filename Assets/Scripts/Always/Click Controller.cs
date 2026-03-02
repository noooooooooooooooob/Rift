using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

/// <summary>
/// 전역 클릭 입력 처리 컨트롤러
/// Unity의 새로운 Input System을 사용하여 좌클릭을 감지하고 레이캐스트 수행
/// IClickable 인터페이스를 구현한 오브젝트에 클릭 이벤트 전달
/// 빈 공간 클릭 시 카메라를 보드 뷰로 전환
/// </summary>
public class ClickController : MonoBehaviour
{
    [SerializeField] private Camera cam; // 레이캐스트에 사용할 카메라 (미지정 시 Camera.main 사용)

    /// <summary>
    /// 초기화: 카메라 참조 설정
    /// </summary>
    void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    /// <summary>
    /// 매 프레임 좌클릭 입력 검사
    /// 클릭 시 레이캐스트로 대상 확인하고 IClickable 인터페이스 호출
    /// </summary>
    void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // UI 클릭 체크 추가 (레이캐스트 방지)
            if (EventSystem.current.IsPointerOverGameObject())
            {
                Debug.Log("UI clicked - skipping raycast");
                return;  // UI 클릭 시 3D 레이캐스트 실행 안함
            }

            Vector2 screenPos = Mouse.current.position.ReadValue();
            Ray ray = cam.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                // 클릭 가능한 오브젝트인지 확인
                var clickable = hit.collider.GetComponent<IClickable>();

                if (clickable != null)
                {
                    clickable.OnClick();  // 클릭 이벤트 전달
                }
                else
                {
                    if(Battle_UI_Manager.instance != null)
                        Battle_UI_Manager.instance.HideUnitDescription();
                }
            }
            
        }
    }
}
