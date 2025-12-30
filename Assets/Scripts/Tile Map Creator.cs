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
    public Tile[,] tiles = new Tile[BattleGrid.Width, BattleGrid.Height];  // 생성된 타일 배열

    /// <summary>
    /// 타일맵 생성 (에디터 전용)
    /// 10x5 그리드에 타일을 배치하고 Tile 컴포넌트 초기화
    /// </summary>
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
