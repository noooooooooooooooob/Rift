using UnityEngine;

public class TileMapCreator : MonoBehaviour
{
    public GameObject tilePrefab;
    public Transform startPos;
    public float spaceX, spaceZ;
    public void GenerateMap()
    {
        ClearMap();  // 기존 타일 제거

        for (int z = 0; z < BattleGrid.Height; z++)
        {
            for (int x = 0; x < BattleGrid.Width; x++)
            {
                Vector3 pos = new Vector3(startPos.position.x + x * spaceX, startPos.position.y, startPos.position.z + z * spaceZ);
                GameObject tileGO = Instantiate(tilePrefab, pos, tilePrefab.transform.rotation, transform);
                tileGO.name = $"Tile_{x}_{z}";
                Tile tile = tileGO.GetComponent<Tile>();

                tile.GridPosition = new Vector2Int(x, z);
                Battle_Manager.instance.battleGrid.Tiles[x, z] = tile;
            }
        }
    }

    public void ClearMap()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

}
