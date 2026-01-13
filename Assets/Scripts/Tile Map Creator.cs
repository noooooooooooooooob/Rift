using UnityEngine;

/// <summary>
/// 타일맵 생성 도구
/// 에디터에서 10x5 그리드 타일맵을 자동 생성
/// TileCreator 커스텀 에디터와 함께 사용
/// </summary>
public class TileMapCreator : MonoBehaviour
{
    public GameObject tilePrefab;  // 타일 프리팹
    public Transform playerTilesStartPos;     // 생성 시작 위치
    public Transform enemyTilesStartPos;      // 적 타일 생성 시작 위치
    public float spaceX, spaceZ;   // 타일 간격 (X, Z축)
    public Tile[,] playerTiles = new Tile[Battle_Manager.GridWidth, Battle_Manager.GridHeight];  // 생성된 타일 배열
    public Tile[,] enemyTiles = new Tile[Battle_Manager.GridWidth, Battle_Manager.GridHeight];  // 내부 타일 배열

    /// <summary>
    /// 런타임 초기화: 자식 오브젝트에서 타일들을 찾아서 배열에 저장
    /// </summary>
    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            Tile tile = child.GetComponent<Tile>();
            if (tile != null)
            {
                Vector2Int gridPos = tile.GridPosition;
                if (child.name.StartsWith("Player_Tile_"))
                {
                    playerTiles[gridPos.x, gridPos.y] = tile;
                }
                else if (child.name.StartsWith("Enemy_Tile_"))
                {
                    enemyTiles[gridPos.x, gridPos.y] = tile;
                }
            }
        }
    }

    /// <summary>
    /// 타일맵 생성 (에디터 전용)
    /// 10x5 그리드에 타일을 배치하고 Tile 컴포넌트 초기화
    /// </summary>
    public void GenerateMap()
    {
        ClearMap();  // 기존 타일 제거

        for (int z = 0; z < Battle_Manager.GridHeight; z++)
        {
            for (int x = 0; x < Battle_Manager.GridWidth; x++)
            {
                Vector3 pos = new Vector3(playerTilesStartPos.position.x + x * spaceX, playerTilesStartPos.position.y, playerTilesStartPos.position.z + z * spaceZ);
                GameObject tileGO = Instantiate(tilePrefab, pos, tilePrefab.transform.rotation, transform);
                tileGO.name = $"Player_Tile_{x}_{z}";
                Tile tile = tileGO.GetComponent<Tile>();

                tile.GridPosition = new Vector2Int(x, z);
                playerTiles[x, z] = tile;

                // 적 타일 생성
                Vector3 enemyPos = new Vector3(enemyTilesStartPos.position.x + x * spaceX, enemyTilesStartPos.position.y, enemyTilesStartPos.position.z + z * spaceZ);
                GameObject enemyTileGO = Instantiate(tilePrefab, enemyPos, tilePrefab.transform.rotation, transform);
                enemyTileGO.name = $"Enemy_Tile_{x}_{z}";
                Tile enemyTile = enemyTileGO.GetComponent<Tile>();

                enemyTile.GridPosition = new Vector2Int(x, z);
                enemyTiles[x, z] = enemyTile;
            }
        }
    }

    /// <summary>
    /// 타일맵 초기화 (에디터 전용)
    /// 모든 자식 오브젝트(타일)를 즉시 삭제
    /// </summary>
    public void ClearMap()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

}
