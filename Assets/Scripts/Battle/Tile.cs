using UnityEngine;

public class Tile : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Vector2Int gridPosition;
    public Vector2Int GridPosition
    {
        get { return gridPosition; }
        set { gridPosition = value; }
    }
    public Unit occupant;
    public bool IsOccupied
    {
        get { return occupant != null; }
    }
    public Vector3 WorldPosition
    {
        get { return transform.position; }
    }
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetHighlight(bool highlight)
    {
        if (highlight)
        {
            spriteRenderer.color = Color.yellow;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }
}
