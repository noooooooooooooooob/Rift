using UnityEngine;
using UnityEngine.UI;

public class Rest_Scene_Manager : MonoBehaviour
{
    public Button continueButton;
    float oneHealPercent = 0.7f;
    float allHealPercent = 0.2f;
    void Awake()
    {
        continueButton.onClick.AddListener(() =>
        {
            Stage_Manager.instance.CompleteCurrentNode();
        });
    }
    public void RestOne()
    {
        
    } 
    public void RestAll()
    {
        Party_Manager.instance.HealPartyPercent(allHealPercent);
    }
}
