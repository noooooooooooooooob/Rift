using UnityEngine;
using Unity.Cinemachine;

public class UnitClickable : MonoBehaviour, IClickable
{
    public CinemachineCamera focusCamera;  // 이 캐릭터 전용 줌인용 카메라
    // public CharacterInfoData infoData;     // 이름/직업/HP 등 정보

    public void OnClick()
    {
        Debug.Log($"Clicked on unit: {gameObject.name}");
        // 카메라 전환
        CameraManager.instance.FocusOnCharacter(focusCamera);

        // UI 열기
        // CharacterInfoPanel.instance.Show(infoData);
    }
}
