using UnityEngine;

public class Unit_Ultimate : MonoBehaviour
{
    public Unit unit;

    private void Awake()
    {
        if (unit == null)
            unit = GetComponent<Unit>();
    }
}
