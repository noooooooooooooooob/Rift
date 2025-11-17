using System.Collections;
using UnityEngine;

enum TurnPhase
{
    Generate,
    Ready,
    Act,
    End
}

public class TurnManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(TurnCycle());
    }
    IEnumerator TurnCycle()
    {
        while (true)
        {
            yield return null;
        }
    }
}
