using UnityEngine;

public class Rest_Scene_Manager : MonoBehaviour
{
    float oneHealPercent = 0.7f;
    float allHealPercent = 0.2f;
    public void RestOne()
    {
        
    } 
    public void RestAll()
    {
        Party_Manager.instance.HealParty(allHealPercent);
    }
}
