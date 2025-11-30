using UnityEngine;
using UnityEngine.InputSystem;

public class ClickController : MonoBehaviour
{
    [SerializeField] private Camera cam; // 안 넣으면 Camera.main 사용

    void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    void Update()
    {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 screenPos = Mouse.current.position.ReadValue();
            Ray ray = cam.ScreenPointToRay(screenPos);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
            {
                // 2) 클릭 가능한 오브젝트인지 확인
                var clickable = hit.collider.GetComponent<IClickable>();

                // Debug.Log($"Raycast hit: {hit.collider.name}");

                if (clickable != null)
                {
                    clickable.OnClick();
                }
                else
                {
                    // 콜라이더는 맞았는데 IClickable 없음 → 빈 공간 취급
                    CameraManager.instance.SetBoardView();
                    // CharacterInfoPanel.I.Hide();
                }
            }
            else
            {
                // 아무 것도 안 맞음 → 완전 빈 공간 클릭
                CameraManager.instance.SetBoardView();
                // CharacterInfoPanel.I.Hide();
            }
        }
    }
}
