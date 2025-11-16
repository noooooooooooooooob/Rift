using System;
using UnityEngine;

public class SpriteBillboard : MonoBehaviour
{
    [SerializeField] private bool freezeRotationX = true;
    [SerializeField] private bool freezeRotationY = false;
    [SerializeField] private bool freezeRotationZ = true;
    Transform cameraTransform;
    void Start()
    {
        cameraTransform = Camera.main.transform;
    }
    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(
            freezeRotationX ? 0 : cameraTransform.rotation.eulerAngles.x,
            freezeRotationY ? 0 : cameraTransform.rotation.eulerAngles.y,
            freezeRotationZ ? 0 : cameraTransform.rotation.eulerAngles.z
        );
    }
}
