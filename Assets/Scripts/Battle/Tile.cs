using UnityEngine;

public class BattleGrid
{
    public const int Width = 10;
    public const int Height = 5;
    public Tile[,] Tiles;
}

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
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void SetHighlight(bool highlight)
    {
        if (highlight)
        {
            spriteRenderer.color = Color.red;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }
}
