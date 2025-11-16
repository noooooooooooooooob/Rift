using UnityEngine;

public class UnitStat
{
    public int HP;
    public int UP;
    public int ATK;
    public int DEF;
    public int SPD;
    public int AGI;
    [Range(0f, 1f)] public float LS;        // LS
    [Range(0f, 1f)] public float RST; // RST
    [Range(0f, 1f)] public float EV;          // EV
    [Range(0f, 1f)] public float CA;    // CA
    [Range(0f, 0.99f)] public float DR; // DR
    public float AP;                   // AP
}