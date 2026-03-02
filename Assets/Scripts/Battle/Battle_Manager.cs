using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 전투 시스템의 핵심 매니저 (싱글톤)
/// 전투 그리드와 플레이어/적 유닛 목록을 관리
/// </summary>
public class Battle_Manager : MonoBehaviour
{
    public const int GridWidth = 5;  // 그리드 너비 (5칸)
    public const int GridHeight = 5;  // 그리드 높이 (5칸)

    public static Battle_Manager instance;  // 싱글톤 인스턴스
    public BattleMap currentBattleMapData;  // 현재 전투 맵 데이터
    public bool isBossBattle = false;
    public BossMap currentBossMapData;  // 현재 보스 맵 데이터
    public TileMapCreator tileMapCreator;  // 타일맵 생성기 참조
    public Tile[,] playerTiles;
    public Tile[,] enemyTiles;
    public List<Unit> playerUnits;  // 플레이어 유닛 목록
    public List<Unit> enemyUnits;  // 적 유닛 목록
    public List<Active_Point> playerActivePoints;
    public List<Active_Point> enemyActivePoints;
    public int playerActivePoint = 2;
    public int enemyActivePoint = 2;
    public Button gameEndButton;

    /// <summary>
    /// 초기화: 싱글톤 설정 및 그리드 생성
    /// </summary>
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        playerTiles = new Tile[GridWidth, GridHeight];
        enemyTiles = new Tile[GridWidth, GridHeight];
        tileInit();
        UpdateActivePoints();
        if(isBossBattle)
        {
            BossMap[] bossMaps = Resources.LoadAll<BossMap>("Boss Maps");
            if (bossMaps.Length > 0)
                currentBossMapData = bossMaps[Random.Range(0, bossMaps.Length)];
        }
        else
        {
            BattleMap[] battleMaps = Resources.LoadAll<BattleMap>("Battle Maps");
            Debug.Log($"LoadAll 결과: {battleMaps.Length}개 로드됨");
            if (battleMaps.Length > 0)
                currentBattleMapData = battleMaps[Random.Range(0, battleMaps.Length)];
            else
                Debug.LogError("Battle Maps 폴더에서 BattleMap을 찾을 수 없엉");
        }
        DeployEnemyUnits();
    }

    /// <summary>
    /// TileMapCreator로부터 타일 참조를 복사하여 그리드 초기화
    /// </summary>
    private void tileInit()
    {
        if (tileMapCreator == null)
        {
            Debug.LogError("[Battle_Manager] tileMapCreator is NULL! Inspector에서 할당하세요.");
            return;
        }

        Debug.Log($"[Battle_Manager] tileInit start - copying from TileMapCreator");

        int nullCount = 0;
        for(int x = 0; x < GridWidth; x++)
        {
            for(int y = 0; y < GridHeight; y++)
            {
                playerTiles[x, y] = tileMapCreator.playerTiles[x, y];
                playerTiles[x, y].isPlayerTile = true;
                enemyTiles[x, y] = tileMapCreator.enemyTiles[x, y];
                if (playerTiles[x, y] == null)
                {
                    nullCount++;
                    Debug.LogWarning($"[Battle_Manager] playerTiles[{x},{y}] is NULL!");
                }
                if (enemyTiles[x, y] == null)
                {
                    nullCount++;
                    Debug.LogWarning($"[Battle_Manager] enemyTiles[{x},{y}] is NULL!");
                }
            }
        }

        Debug.Log($"[Battle_Manager] tileInit complete - {nullCount} NULL tiles out of {GridWidth * GridHeight}");
    }

    public void RemoveUnitFromBattle(Unit unit)
    {
        if (unit.isPlayerUnit)
        {
            if (playerUnits.Contains(unit))
            {
                playerUnits.Remove(unit);
                Debug.Log($"[Battle_Manager] Removed {unit.unitData.unitName} from playerUnits. Remaining: {playerUnits.Count}");
            }
            else
            {
                Debug.LogWarning($"[Battle_Manager] Attempted to remove {unit.unitData.unitName} from playerUnits, but it was not found.");
            }
        }
        else
        {
            if (enemyUnits.Contains(unit))
            {
                enemyUnits.Remove(unit);
                Debug.Log($"[Battle_Manager] Removed {unit.unitData.unitName} from enemyUnits. Remaining: {enemyUnits.Count}");
            }
            else
            {
                Debug.LogWarning($"[Battle_Manager] Attempted to remove {unit.unitData.unitName} from enemyUnits, but it was not found.");
            }
        }
    }

    public void AddPlayerActivePoint(int amount)
    {
        playerActivePoint += amount;
        if (playerActivePoint > playerActivePoints.Count)
            playerActivePoint = playerActivePoints.Count;
        UpdateActivePoints();
    }
    public void AddEnemyActivePoint(int amount)
    {
        enemyActivePoint += amount;
        if (enemyActivePoint > enemyActivePoints.Count)
            enemyActivePoint = enemyActivePoints.Count;
        UpdateActivePoints();
    }
    public bool UsePlayerActivePoint(int amount)
    {
        if(playerActivePoint >= amount)
        {
            playerActivePoint -= amount;
            UpdateActivePoints();
            return true;
        }
        return false;
    }
    public bool UseEnemyActivePoint(int amount)
    {
        if (enemyActivePoint >= amount)
        {
            enemyActivePoint -= amount;
            UpdateActivePoints();
            return true;
        }
        return false;
    }
    public void UpdateActivePoints()
    {
        for (int i = 0; i < playerActivePoints.Count; i++)
        {
            playerActivePoints[i].SetActivePoint(i < playerActivePoint);
        }
        for (int i = 0; i < enemyActivePoints.Count; i++)
        {
            enemyActivePoints[i].SetActivePoint(i < enemyActivePoint);
        }
    }
    void DeployEnemyUnits()
    {
        List<EnemyData> enemies = isBossBattle ? currentBossMapData.enemys : currentBattleMapData.enemies;

        if (enemies == null || enemies.Count == 0)
        {
            Debug.LogWarning("[Battle_Manager] 배치할 적 유닛이 없습니다!");
            return;
        }

        // 빈 타일 목록 (랜덤 배치용)
        List<Tile> emptyTiles = new List<Tile>();
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                if (enemyTiles[x, y] != null && enemyTiles[x, y].occupant == null)
                    emptyTiles.Add(enemyTiles[x, y]);
            }
        }

        foreach (var enemyData in enemies)
        {
            if (enemyData.enemyPrefab == null)
            {
                Debug.LogWarning("[Battle_Manager] enemyPrefab이 null입니다!");
                continue;
            }

            Tile targetTile = null;

            // 랜덤 배치
            if (enemyData.positions.x == -1 && enemyData.positions.y == -1)
            {
                if (emptyTiles.Count > 0)
                {
                    int randomIndex = Random.Range(0, emptyTiles.Count);
                    targetTile = emptyTiles[randomIndex];
                    emptyTiles.RemoveAt(randomIndex);
                }
                else
                {
                    Debug.LogWarning("[Battle_Manager] 빈 타일이 없습니다!");
                    continue;
                }
            }
            else
            {
                // 지정된 위치에 배치
                int x = enemyData.positions.x;
                int y = enemyData.positions.y;

                if (x >= 0 && x < GridWidth && y >= 0 && y < GridHeight)
                {
                    targetTile = enemyTiles[x, y];
                    emptyTiles.Remove(targetTile);
                }
                else
                {
                    Debug.LogWarning($"[Battle_Manager] 유효하지 않은 위치: ({x}, {y})");
                    continue;
                }
            }

            if (targetTile != null && targetTile.occupant == null)
            {
                // 유닛 생성 및 배치
                GameObject enemyObj = Instantiate(enemyData.enemyPrefab, targetTile.transform.position + enemyData.enemyPrefab.GetComponent<Unit>().unitOffset, Quaternion.identity);
                Unit enemyUnit = enemyObj.GetComponent<Unit>();

                if (enemyUnit != null)
                {
                    // 유닛 오프셋 적용
                    enemyObj.transform.position = targetTile.WorldPosition + enemyUnit.unitOffset;
                    targetTile.occupant = enemyUnit.gameObject;
                    enemyUnit.currentTile = targetTile;
                    enemyUnits.Add(enemyUnit);
                    Debug.Log($"[Battle_Manager] {enemyUnit.unitData.unitName} 배치 완료: ({targetTile.GridPosition})");
                }
                else
                {
                    Debug.LogError("[Battle_Manager] 프리팹에 Unit 컴포넌트가 없습니다!");
                    Destroy(enemyObj);
                }
            }
        }
    }

    public void OnBattleEnd()
    {
        // 플레이어 유닛이 모두 죽었는지
        bool allPlayersDead = playerUnits.TrueForAll(u => u == null || u.isDead);
        if (allPlayersDead)
        {
            // 전투 UI 숨김
            if (Battle_UI_Manager.instance != null)
            {
                Battle_UI_Manager.instance.OnBattleEnd(allPlayersDead);
            }
            Party_Manager.instance.OnBattleEnd();
            if (Battle_UI_Manager.instance != null)
            {
                Battle_UI_Manager.instance.ReturnUIToUnits();
            }
        }
        else
        {
            Party_Manager.instance.GainMoney(isBossBattle ? currentBossMapData.awardGold : currentBattleMapData.awardGold);
            // 전투 UI 숨김
            if (Battle_UI_Manager.instance != null)
            {
                Battle_UI_Manager.instance.OnBattleEnd(allPlayersDead);
            }
            Stage_Manager.instance.CompleteCurrentNode();
            Party_Manager.instance.OnBattleEnd();
            if (Battle_UI_Manager.instance != null)
            {
                Battle_UI_Manager.instance.ReturnUIToUnits();
            }
        }
        
    }
}
