using UnityEngine;

public class Action_Menu_UI : MonoBehaviour
{
    public GameObject ActionMenuPanel;
    public GameObject SkillMenuPanel;
    public GameObject ReturnMenuPanel;
    public void ShowActionMenu()
    {
        ActionMenuPanel.SetActive(true);
    }
    public void HideActionMenu()
    {
        ActionMenuPanel.SetActive(false);
    }
    public void ShowSkillMenu()
    {
        SkillMenuPanel.SetActive(true);
    }
    public void HideSkillMenu()
    {
        SkillMenuPanel.SetActive(false);
    }
    public void ShowReturnMenu()
    {
        SkillMenuPanel.SetActive(false);
        ActionMenuPanel.SetActive(false);
        ReturnMenuPanel.SetActive(true);
    }
    public void HideReturnMenu()
    {
        ReturnMenuPanel.SetActive(false);
    }
}
