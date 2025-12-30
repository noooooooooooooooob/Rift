using UnityEngine;
using Unity.Cinemachine;

/// <summary>
/// 유닛을 클릭 가능하게 만드는 컴포넌트
/// IClickable 인터페이스를 구현하여 ClickController로부터 클릭 이벤트를 받음
/// 클릭 시 해당 유닛에 포커스하도록 카메라 전환
/// </summary>
public class UnitClickable : MonoBehaviour, IClickable
{
    public CinemachineCamera focusCamera;  // 이 캐릭터 전용 줌인용 카메라
    // public CharacterInfoData infoData;     // 이름/직업/HP 등 정보 (TODO: 구현 예정)

    /// <summary>
    /// 유닛 클릭 시 호출
    /// 카메라를 해당 유닛에 포커스하고 UI 패널 표시 (TODO)
    /// </summary>
    public void OnClick()
    {
        Debug.Log($"Clicked on unit: {gameObject.name}");
        // 카메라 전환
        CameraManager.instance.FocusOnCharacter(focusCamera);

        // UI 열기 (TODO: 캐릭터 정보 패널 구현 예정)
        // CharacterInfoPanel.instance.Show(infoData);
    }
}
