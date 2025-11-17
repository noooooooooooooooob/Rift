using UnityEngine;

public class TileMapCreator : MonoBehaviour
{
    public GameObject tilePrefab;
    public Transform startPos;
    public int width;
    public int height;
    public float spaceX, spaceZ;
    public void GenerateMap()
    {
        ClearMap();  // 기존 타일 제거

        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3 pos = new Vector3(startPos.position.x + x * spaceX, startPos.position.y, startPos.position.z + z * spaceZ);
                GameObject tile = Instantiate(tilePrefab, pos, tilePrefab.transform.rotation, transform);
                tile.name = $"Tile_{x}_{z}";
                tile.GetComponent<Tile>().GridPosition = new Vector2Int(x, z);
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
