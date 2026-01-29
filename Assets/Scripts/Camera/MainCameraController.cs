using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

/// <summary>
/// 메인 카메라 컨트롤러
/// 우클릭 드래그로 카메라 X축 이동
/// 마우스 휠로 Y+Z 이동 기반 줌 인/아웃
/// 부드러운 이동을 위한 Damping 적용
/// </summary>
public class MainCameraController : MonoBehaviour
{
    [Header("Cinemachine")]
    public Transform cameraTransform;              // 실제 CinemachineCamera의 transform
    public CinemachineCamera vcam;                 // Virtual Camera (FOV는 고정)
    public Transform lookAtTarget;                 // 카메라가 바라볼 타겟 (전장 중심)

    [Header("Movement Settings")]
    public float horizontalSensitivity = 0.01f;    // 좌우 드래그 감도
    public float verticalSensitivity = 0.01f;      // 상하 드래그 감도 (Y 오프셋)

    [Header("Position Limits")]
    public float minX = -5f;                       // X축 최소 위치
    public float maxX = 5f;                        // X축 최대 위치
    public float minYOffset = -3f;                 // Y 오프셋 최소값
    public float maxYOffset = 3f;                  // Y 오프셋 최대값
    public float minY = 1f;                        // 최종 Y 최소값 (바닥 뚫기 방지)

    [Header("Zoom (Y+Z Movement)")]
    public float zoomSensitivity = 0.001f;         // 줌 속도
    [Tooltip("줌인 시 카메라 Y 위치 (낮음)")]
    public float zoomInY = 3f;
    [Tooltip("줌아웃 시 카메라 Y 위치 (높음)")]
    public float zoomOutY = 10f;
    [Tooltip("줌인 시 카메라 Z 위치 (가까움)")]
    public float zoomInZ = -5f;
    [Tooltip("줌아웃 시 카메라 Z 위치 (멀음)")]
    public float zoomOutZ = -15f;

    float currentZoom;                             // 현재 줌 레벨 (0=줌아웃, 1=줌인)
    float targetZoom;                              // 목표 줌 레벨

    [Header("Damping")]
    public float positionDamping = 8f;             // 위치 이동 Damping
    public float rotationDamping = 8f;             // 회전 Damping
    public float zoomDamping = 8f;                 // 줌 Damping

    bool isDragging;                               // 현재 드래그 중인지
    Vector2 lastPointerPos;                        // 이전 프레임의 마우스 위치

    float currentXPos;                             // 현재 X 위치
    float targetXPos;                              // 목표 X 위치

    float currentYOffset;                          // 현재 Y 오프셋 (드래그)
    float targetYOffset;                           // 목표 Y 오프셋 (드래그)

    /// <summary>
    /// 초기화: 현재 카메라 상태를 기준으로 초기값 설정
    /// </summary>
    void Start()
    {
        // ---------- Position ----------
        currentXPos = cameraTransform.position.x;
        targetXPos = currentXPos;

        // ---------- Zoom ----------
        // 현재 Y 위치를 기준으로 초기 줌 레벨 계산
        float currentY = cameraTransform.position.y;
        currentZoom = Mathf.InverseLerp(zoomOutY, zoomInY, currentY);
        targetZoom = currentZoom;

        // ---------- Y Offset ----------
        currentYOffset = 0f;
        targetYOffset = 0f;
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
    /// 좌우 드래그: 카메라 X 위치 이동
    /// 상하 드래그: 카메라 Y 오프셋 이동 (줌 Y에 추가)
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
    /// 마우스 휠로 Y+Z 위치 이동 (줌 인/아웃)
    /// 스크롤 업: 줌인 (카메라가 낮아지면서 가까워짐)
    /// 스크롤 다운: 줌아웃 (카메라가 높아지면서 멀어짐)
    /// </summary>
    void HandleZoomWheel()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;  // +120 / -120 단위

        if (Mathf.Abs(scroll) > 0.01f)
        {
            // 스크롤 업 = 줌인 (targetZoom 증가)
            targetZoom += scroll * zoomSensitivity;
            targetZoom = Mathf.Clamp01(targetZoom);
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
        // X Position (드래그)
        currentXPos = Mathf.Lerp(currentXPos, targetXPos, Time.deltaTime * positionDamping);

        // Y Offset (드래그)
        currentYOffset = Mathf.Lerp(currentYOffset, targetYOffset, Time.deltaTime * positionDamping);

        // Zoom (Y, Z Position)
        currentZoom = Mathf.Lerp(currentZoom, targetZoom, Time.deltaTime * zoomDamping);
        float zoomY = Mathf.Lerp(zoomOutY, zoomInY, currentZoom);
        float targetZ = Mathf.Lerp(zoomOutZ, zoomInZ, currentZoom);

        Vector3 pos = cameraTransform.position;
        pos.x = currentXPos;
        pos.y = Mathf.Max(zoomY + currentYOffset, minY);  // 줌 Y + 드래그 오프셋, 최소값 제한
        pos.z = targetZ;
        cameraTransform.position = pos;

        // LookAt (X축 rotation만 변경, Y/Z축 고정)
        if (lookAtTarget != null)
        {
            Vector3 direction = lookAtTarget.position - cameraTransform.position;
            float targetPitch = Mathf.Atan2(-direction.y, new Vector2(direction.x, direction.z).magnitude) * Mathf.Rad2Deg;

            Vector3 euler = cameraTransform.eulerAngles;
            euler.x = Mathf.LerpAngle(euler.x, targetPitch, Time.deltaTime * rotationDamping);
            // Y, Z축은 고정
            cameraTransform.eulerAngles = euler;
        }
    }


    // ==========================
    // 4) 드래그 값 적용
    // ==========================
    /// <summary>
    /// 드래그 델타값을 카메라 파라미터에 적용
    /// X 델타 → 카메라 X 위치 이동
    /// Y 델타 → 카메라 Y 오프셋 이동
    /// </summary>
    void ApplyDrag(Vector2 delta)
    {
        // X 위치 이동
        targetXPos += -delta.x * horizontalSensitivity;
        targetXPos = Mathf.Clamp(targetXPos, minX, maxX);

        // Y 오프셋 이동
        targetYOffset += -delta.y * verticalSensitivity;
        targetYOffset = Mathf.Clamp(targetYOffset, minYOffset, maxYOffset);
    }
}
