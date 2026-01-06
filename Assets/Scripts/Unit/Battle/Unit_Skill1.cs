using UnityEngine;

public class Unit_Skill1 : MonoBehaviour
{
    public Unit unit;

    private void Awake()
    {
        if (unit == null)
            unit = GetComponent<Unit>();
    }
}
