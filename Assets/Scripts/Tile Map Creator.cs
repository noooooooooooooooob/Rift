using UnityEngine;

/// <summary>
/// 타일맵 생성 도구
/// 에디터에서 10x5 그리드 타일맵을 자동 생성
/// TileCreator 커스텀 에디터와 함께 사용
/// </summary>
public class TileMapCreator : MonoBehaviour
{
    public GameObject tilePrefab;  // 타일 프리팹
    public Transform startPos;     // 생성 시작 위치
    public float spaceX, spaceZ;   // 타일 간격 (X, Z축)
    public Tile[,] tiles = new Tile[Battle_Manager.GridWidth, Battle_Manager.GridHeight];  // 생성된 타일 배열

    /// <summary>
    /// 런타임 초기화: 자식 오브젝트에서 타일들을 찾아서 배열에 저장
    /// </summary>
    private void Awake()
    {
        Debug.Log("[TileMapCreator] Awake - Finding tiles from children");

        // 자식 오브젝트들을 순회하며 타일 찾기
        foreach (Transform child in transform)
        {
            Tile tile = child.GetComponent<Tile>();
            if (tile != null)
            {
                Vector2Int pos = tile.GridPosition;
                if (pos.x >= 0 && pos.x < Battle_Manager.GridWidth &&
                    pos.y >= 0 && pos.y < Battle_Manager.GridHeight)
                {
                    tiles[pos.x, pos.y] = tile;
                    Debug.Log($"[TileMapCreator] Found tile at ({pos.x}, {pos.y}): {child.name}");
                }
            }
        }

        // NULL 타일 체크
        int nullCount = 0;
        for (int x = 0; x < Battle_Manager.GridWidth; x++)
        {
            for (int y = 0; y < Battle_Manager.GridHeight; y++)
            {
                if (tiles[x, y] == null)
                {
                    nullCount++;
                    Debug.LogWarning($"[TileMapCreator] tiles[{x}, {y}] is NULL!");
                }
            }
        }

        Debug.Log($"[TileMapCreator] Awake complete - {nullCount} NULL tiles out of {Battle_Manager.GridWidth * Battle_Manager.GridHeight}");
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
                Vector3 pos = new Vector3(startPos.position.x + x * spaceX, startPos.position.y, startPos.position.z + z * spaceZ);
                GameObject tileGO = Instantiate(tilePrefab, pos, tilePrefab.transform.rotation, transform);
                tileGO.name = $"Tile_{x}_{z}";
                Tile tile = tileGO.GetComponent<Tile>();

                tile.GridPosition = new Vector2Int(x, z);
                tiles[x, z] = tile;
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
