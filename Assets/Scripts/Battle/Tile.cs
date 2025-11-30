using UnityEngine;

public class BattleGrid
{
    public const int Width = 10;
    public const int Height = 5;
    public Tile[,] Tiles;
}

public class Tile : MonoBehaviour
{
    public Vector2Int tileOffset; // Offset of the tile in the grid
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
}
