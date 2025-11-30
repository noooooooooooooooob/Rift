using UnityEngine;

public class Battle_Manager : MonoBehaviour
{
    public static Battle_Manager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public BattleGrid battleGrid;
}
