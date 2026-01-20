using System;
using UnityEngine;

/// <summary>
/// 스프라이트 빌보드 컴포넌트
/// 2D 스프라이트가 항상 카메라를 향하도록 회전 처리
/// 축별로 회전 고정 여부를 설정 가능 (기본: Y축만 회전, X/Z축은 고정)
/// [ExecuteAlways]로 에디터 모드에서도 동작
/// </summary>
[ExecuteAlways]
public class SpriteBillboard : MonoBehaviour
{
    [SerializeField] private bool freezeRotationX = true;   // X축 회전 고정 (상하 회전 방지)
    [SerializeField] private bool freezeRotationY = false;  // Y축 회전 허용 (카메라 방향으로 회전)
    [SerializeField] private bool freezeRotationZ = true;   // Z축 회전 고정 (기울기 방지)
    Transform cameraTransform;  // 메인 카메라 Transform 캐싱

    /// <summary>
    /// 초기화: 메인 카메라 Transform 가져오기
    /// </summary>
    void Start()
    {
        if (cameraTransform == null)
        {
            findCamera();
        }
    }

    /// <summary>
    /// LateUpdate에서 회전 처리 (다른 Transform 업데이트 이후 실행)
    /// 카메라의 회전값을 복사하되, 고정된 축은 0도로 설정
    /// </summary>
    void LateUpdate()
    {
        if (cameraTransform == null) 
        {
            findCamera();
        }

        transform.rotation = Quaternion.Euler(
            freezeRotationX ? 0 : cameraTransform.rotation.eulerAngles.x,
            freezeRotationY ? 0 : cameraTransform.rotation.eulerAngles.y,
            freezeRotationZ ? 0 : cameraTransform.rotation.eulerAngles.z
        );
    }

    void findCamera()
    {
        cameraTransform = Camera.main.transform;
    }
}
