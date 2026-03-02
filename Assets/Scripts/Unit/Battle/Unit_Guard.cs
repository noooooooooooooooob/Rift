using UnityEngine;

public class Unit_Guard : MonoBehaviour
{
    public Unit unit;

    private void Awake()
    {
        if (unit == null)
            unit = GetComponent<Unit>();
    }
    public void ExecuteGuard()
    {
        Debug.Log($"[Unit_Guard] {unit.unitData.unitName} is guarding.");
        
        unit.Guard((int)((float)unit.maxHP * 0.1f));
    }
}
