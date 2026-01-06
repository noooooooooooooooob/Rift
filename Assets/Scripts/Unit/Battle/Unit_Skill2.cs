using UnityEngine;

public class Unit_Skill2 : MonoBehaviour
{
    public Unit unit;

    private void Awake()
    {
        if (unit == null)
            unit = GetComponent<Unit>();
    }
}
