using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

/// <summary>
/// 메인 카메라 컨트롤러
/// 우클릭 드래그로 스플라인 돌리 이동 및 피치(상하) 회전
/// 마우스 휠로 FOV 기반 줌 인/아웃
/// 부드러운 이동을 위한 Damping 적용
/// </summary>
public class MainCameraController : MonoBehaviour
{
    [Header("Cinemachine")]
    public CinemachineSplineDolly dolly;           // 스플라인 경로를 따라 이동하는 Dolly
    public Transform cameraTransform;              // 실제 CinemachineCamera의 transform
    public CinemachineCamera vcam;                 // FOV 수정용 Virtual Camera

    [Header("Settings")]
    public float horizontalSensitivity = 0.003f;   // 좌우 드래그 감도 (Dolly Position)
    public float verticalSensitivity   = 0.1f;     // 상하 드래그 감도 (Pitch)
    public float minPitch = -15f;                  // 최소 피치 각도 (내려다보기 제한)
    public float maxPitch = 30f;                   // 최대 피치 각도 (올려다보기 제한)

    [Header("Zoom")]
    public float zoomSensitivity = 5f;             // 줌 속도
    public float minFOV = 30f;                     // 최소 FOV (줌인)
    public float maxFOV = 70f;                     // 최대 FOV (줌아웃)
    float targetFOV;                               // 목표 FOV
    float currentFOV;                              // 현재 FOV

    [Header("Damping")]
    public float positionDamping = 8f;             // 위치 이동 Damping
    public float rotationDamping = 8f;             // 회전 Damping
    public float zoomDamping = 8f;                 // 줌 Damping

    bool isDragging;                               // 현재 드래그 중인지
    Vector2 lastPointerPos;                        // 이전 프레임의 마우스 위치

    float currentPitch;                            // 현재 피치 각도
    float targetPitch;                             // 목표 피치 각도

    float currentPos;                              // 현재 Dolly Position (0~1)
    float targetPos;                               // 목표 Dolly Position (0~1)

    /// <summary>
    /// 초기화: 현재 카메라 상태를 기준으로 초기값 설정
    /// </summary>
    void Start()
    {
        // ---------- Pitch 초기화 ----------
        currentPitch = cameraTransform.localEulerAngles.x;
        if (currentPitch > 180) currentPitch -= 360;  // -180~180 범위로 정규화
        targetPitch = currentPitch;

        // ---------- Dolly pos ----------
        currentPos = dolly.CameraPosition;
        targetPos = currentPos;

        // ---------- FOV ----------
        currentFOV = vcam.Lens.FieldOfView;
        targetFOV = currentFOV;
    }

    /// <summary>
    /// 매 프레임 입력 처리 및 카메라 업데이트
    /// </summary>
    void Update()
    {
        if (Mouse.current == null) return;

        HandleDrag();       // 우클릭 드래그 처리
        HandleZoomWheel();  // 마우스 휠 줌 처리
        ApplyDamping();     // Damping 적용하여 부드럽게 이동
    }


    // ==========================
    // 1) 드래그 처리
    // ==========================
    /// <summary>
    /// 우클릭 드래그 처리
    /// 좌우 드래그: Dolly Position 이동 (스플라인 경로를 따라 이동)
    /// 상하 드래그: Pitch 회전 (카메라 각도 조절)
    /// </summary>
    void HandleDrag()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            isDragging = true;
            lastPointerPos = Mouse.current.position.ReadValue();
        }
        else if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }

        if (!isDragging) return;

        Vector2 curr = Mouse.current.position.ReadValue();
        Vector2 delta = curr - lastPointerPos;
        lastPointerPos = curr;

        ApplyDrag(delta);
    }


    // ==========================
    // 2) 마우스 휠 줌
    // ==========================
    /// <summary>
    /// 마우스 휠로 FOV 조절 (줌 인/아웃)
    /// 스크롤 업: FOV 감소 = 줌인
    /// 스크롤 다운: FOV 증가 = 줌아웃
    /// </summary>
    void HandleZoomWheel()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;  // +120 / -120 단위

        if (Mathf.Abs(scroll) > 0.01f)
        {
            // FOV 감소 = 줌인
            targetFOV -= scroll * zoomSensitivity * Time.deltaTime;
            targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);
        }
    }


    // ==========================
    // 3) Damping 일괄 적용
    // ==========================
    /// <summary>
    /// 모든 카메라 파라미터에 Damping 적용
    /// Lerp를 사용하여 부드러운 이동/회전/줌 효과 구현
    /// </summary>
    void ApplyDamping()
    {
        // Dolly Position
        currentPos = Mathf.Lerp(currentPos, targetPos, Time.deltaTime * positionDamping);
        dolly.CameraPosition = currentPos;

        // Pitch
        currentPitch = Mathf.Lerp(currentPitch, targetPitch, Time.deltaTime * rotationDamping);
        Vector3 euler = cameraTransform.localEulerAngles;
        euler.x = currentPitch;
        cameraTransform.localEulerAngles = euler;

        // FOV Zoom
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * zoomDamping);
        vcam.Lens.FieldOfView = currentFOV;
    }


    // ==========================
    // 4) 드래그 값 적용
    // ==========================
    /// <summary>
    /// 드래그 델타값을 카메라 파라미터에 적용
    /// X 델타 → Dolly Position (스플라인 경로 이동)
    /// Y 델타 → Pitch (카메라 상하 각도)
    /// </summary>
    void ApplyDrag(Vector2 delta)
    {
        // 1. Spline Dolly pos 좌우 이동
        targetPos += -delta.x * horizontalSensitivity;
        targetPos = Mathf.Clamp01(targetPos);  // 0~1 범위 제한

        // 2. Pitch
        targetPitch -= -delta.y * verticalSensitivity;
        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);  // 각도 제한
    }
}
