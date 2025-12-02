using UnityEngine;

[CreateAssetMenu(fileName = "UnitData", menuName = "Scriptable Objects/UnitData")]
public class UnitData : ScriptableObject
{
    public string unitName;
    public string unitClass;
    public string unitDescription;
    public Sprite unitPortrait;
    public Sprite unitDraggingSprite;
    public UnitStat baseStat;
}
