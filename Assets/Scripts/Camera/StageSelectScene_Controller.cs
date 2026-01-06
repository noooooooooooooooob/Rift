using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 스테이지 선택 씬 카메라 컨트롤러
/// 좌클릭 드래그로 카메라 이동
/// </summary>
public class StageSelectScene_Controller : MonoBehaviour
{
    [Header("Camera")]
    public Transform cameraTransform;              // 카메라 Transform

    [Header("Drag Settings")]
    public float dragSensitivity = 0.01f;          // 드래그 감도
    public float minPosition = -10f;               // 최소 X 위치
    public float maxPosition = 10f;                // 최대 X 위치

    [Header("Scroll Settings")]
    public float scrollSensitivity = 0.5f;         // 휠 스크롤 감도

    [Header("Damping")]
    public float damping = 8f;                     // 이동 Damping

    bool isDragging;                               // 현재 드래그 중인지
    Vector2 lastPointerPos;                        // 이전 프레임의 마우스 위치

    Vector3 targetPosition;                        // 목표 위치
    Vector3 currentPosition;                       // 현재 위치

    void Start()
    {
        // 카메라가 할당되지 않았으면 메인 카메라 사용
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        currentPosition = cameraTransform.position;
        targetPosition = currentPosition;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        HandleDrag();
        HandleScroll();
        ApplyDamping();
    }

    /// <summary>
    /// 좌클릭 드래그 처리
    /// 드래그 방향의 반대로 카메라 이동
    /// </summary>
    void HandleDrag()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            isDragging = true;
            lastPointerPos = Mouse.current.position.ReadValue();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }

        if (!isDragging) return;

        Vector2 curr = Mouse.current.position.ReadValue();
        Vector2 delta = curr - lastPointerPos;
        lastPointerPos = curr;

        ApplyDrag(delta);
    }

    /// <summary>
    /// 드래그 델타값을 카메라 위치에 적용
    /// </summary>
    void ApplyDrag(Vector2 delta)
    {
        // 드래그 방향의 반대로 이동 (맵을 끌어당기는 느낌)
        targetPosition.x -= delta.x * dragSensitivity;

        // 위치 제한
        targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition, maxPosition);
    }

    /// <summary>
    /// 마우스 휠 스크롤 처리
    /// 휠 아래로: 오른쪽 이동 (다음 스테이지)
    /// 휠 위로: 왼쪽 이동 (이전 스테이지)
    /// </summary>
    void HandleScroll()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;  // +120 / -120 단위

        if (Mathf.Abs(scroll) > 0.01f)
        {
            // 휠 아래로 (-) = 오른쪽 이동 (+X)
            // 휠 위로 (+) = 왼쪽 이동 (-X)
            targetPosition.x -= scroll * scrollSensitivity;

            // 위치 제한
            targetPosition.x = Mathf.Clamp(targetPosition.x, minPosition, maxPosition);
        }
    }

    /// <summary>
    /// Damping 적용하여 부드러운 이동
    /// </summary>
    void ApplyDamping()
    {
        currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * damping);
        cameraTransform.position = currentPosition;
    }
}
