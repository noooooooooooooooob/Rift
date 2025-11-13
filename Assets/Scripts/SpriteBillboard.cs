using System;
using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    [SerializeField] private bool freezeRotationXZ = true;
    void Update()
    {
        transform.rotation = freezeRotationXZ ? Quaternion.Euler(0f,Camera.main.transform.rotation.eulerAngles.y,0f) : Camera.main.transform.rotation;
    }
}
