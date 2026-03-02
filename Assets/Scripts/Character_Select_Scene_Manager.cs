using UnityEngine;
using UnityEngine.UI;

public class Character_Select_Scene_Manager : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    void Start()
    {
        continueButton.interactable = false;
    }
    public void CheckCanContinue()
    {
        int selectedCount = Party_Manager.instance.GetPartyCount();
        Debug.Log("선택된 유닛 수: " + selectedCount);
        if (selectedCount < 1)
        {
            continueButton.interactable = false;
        }
        else
        {
            continueButton.interactable = true;
        }
    }
}
