using UnityEngine;

public class Unit_Guard : MonoBehaviour
{
    public Unit unit;

    private void Awake()
    {
        if (unit == null)
            unit = GetComponent<Unit>();
    }
}
