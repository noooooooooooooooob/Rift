using UnityEngine;
using Unity.Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    public CinemachineCamera boardCam;           // 기본 보드 카메라
    CinemachineCamera currentCharCam = null;

    [Header("Priority")]
    public int boardPriority = 10;
    public int focusPriority = 20;

    void Awake()
    {
        instance = this;
        SetBoardView();
    }

    public void SetBoardView()
    {
        boardCam.Priority = boardPriority;
        if (currentCharCam != null)
            currentCharCam.Priority = boardPriority - 1; // 낮게

        currentCharCam = null;
    }

    public void FocusOnCharacter(CinemachineCamera charCam)
    {
        // 보드 카메라는 우선순위 낮추고
        boardCam.Priority = boardPriority;

        // 캐릭터 카메라는 높게
        charCam.Priority = focusPriority;

        currentCharCam = charCam;
    }

    public bool IsBoardView => currentCharCam == null;
}
