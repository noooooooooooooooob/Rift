using UnityEngine;

[ExecuteAlways]
public class WorldPositionDisplay : MonoBehaviour
{
    [Header("World Position (Read Only)")]
    [SerializeField] private Vector3 worldPosition;

    private void Update()
    {
        worldPosition = transform.position;
    }
}
