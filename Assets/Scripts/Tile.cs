using UnityEngine;

public class Tile : MonoBehaviour
{
    public Vector2Int tileOffset; // Offset of the tile in the grid
    Vector2Int gridPosition;
    public Vector2Int GridPosition
    {
        get { return gridPosition; }
        set { gridPosition = value; }
    }

}
