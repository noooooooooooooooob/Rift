using UnityEngine;
using Unity.Cinemachine;

/// <summary>
/// Cinemachine 카메라 전환 관리자 (싱글톤)
/// 보드 뷰(전체 전장)와 캐릭터 포커스 뷰 간의 전환을 우선순위로 제어
/// 높은 우선순위를 가진 카메라가 활성화됨
/// </summary>
public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;  // 싱글톤 인스턴스

    public CinemachineCamera boardCam;           // 기본 보드 카메라 (전체 전장 뷰)
    CinemachineCamera currentCharCam = null;     // 현재 포커스 중인 캐릭터 카메라 (null이면 보드 뷰)

    [Header("Priority")]
    public int boardPriority = 10;    // 보드 카메라 우선순위
    public int focusPriority = 20;    // 캐릭터 포커스 카메라 우선순위 (더 높음)
    public bool canFocus = true;      // 카메라 포커스 가능 여부

    /// <summary>
    /// 초기화: 싱글톤 설정 및 보드 뷰로 시작
    /// </summary>
    void Awake()
    {
        instance = this;
        SetBoardView();
    }

    /// <summary>
    /// 보드 뷰로 전환
    /// 보드 카메라 우선순위를 높이고 캐릭터 카메라 우선순위를 낮춤
    /// </summary>
    public void SetBoardView()
    {
        boardCam.Priority = boardPriority;
        if (currentCharCam != null)
            currentCharCam.Priority = boardPriority - 1; // 낮게

        currentCharCam = null;
    }

    /// <summary>
    /// 특정 캐릭터에 포커스
    /// 캐릭터 카메라 우선순위를 높여서 해당 유닛을 클로즈업
    /// </summary>
    /// <param name="charCam">포커스할 캐릭터의 Cinemachine 카메라</param>
    public void FocusOnCharacter(CinemachineCamera charCam)
    {
        if (!canFocus)
            return;
        // 보드 카메라는 우선순위 낮추고
        boardCam.Priority = boardPriority;

        // 캐릭터 카메라는 높게
        charCam.Priority = focusPriority;

        currentCharCam = charCam;
    }

    /// <summary>
    /// 현재 보드 뷰인지 확인
    /// </summary>
    public bool IsBoardView => currentCharCam == null;
}
