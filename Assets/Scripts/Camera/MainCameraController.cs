using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.InputSystem;

public class MainCameraController : MonoBehaviour
{
    [Header("Cinemachine")]
    public CinemachineSplineDolly dolly;
    public Transform cameraTransform; // 실제 CinemachineCamera의 transform
    public CinemachineCamera vcam; // FOV 수정용

    [Header("Settings")]
    public float horizontalSensitivity = 0.003f;
    public float verticalSensitivity   = 0.1f;
    public float minPitch = -15f;
    public float maxPitch = 30f;

    [Header("Zoom")]
    public float zoomSensitivity = 5f;
    public float minFOV = 30f;
    public float maxFOV = 70f;
    float targetFOV;
    float currentFOV;

    [Header("Damping")]
    public float positionDamping = 8f;
    public float rotationDamping = 8f;
    public float zoomDamping = 8f;

    bool isDragging;
    Vector2 lastPointerPos;

    float currentPitch;
    float targetPitch;

    float currentPos;
    float targetPos;

    void Start()
    {
        // ---------- Pitch 초기화 ----------
        currentPitch = cameraTransform.localEulerAngles.x;
        if (currentPitch > 180) currentPitch -= 360;
        targetPitch = currentPitch;

        // ---------- Dolly pos ----------
        currentPos = dolly.CameraPosition;
        targetPos = currentPos;

        // ---------- FOV ----------
        currentFOV = vcam.Lens.FieldOfView;
        targetFOV = currentFOV;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        HandleDrag();
        HandleZoomWheel();
        ApplyDamping();
    }


    // ==========================
    // 1) 드래그 처리
    // ==========================
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
    void ApplyDrag(Vector2 delta)
    {
        // 1. Spline Dolly pos 좌우 이동
        targetPos += -delta.x * horizontalSensitivity;
        targetPos = Mathf.Clamp01(targetPos);

        // 2. Pitch
        targetPitch -= -delta.y * verticalSensitivity;
        targetPitch = Mathf.Clamp(targetPitch, minPitch, maxPitch);
    }
}
