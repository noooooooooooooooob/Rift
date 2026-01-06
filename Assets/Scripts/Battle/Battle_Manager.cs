using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

/// <summary>
/// 전투 시스템의 핵심 매니저 (싱글톤)
/// 전투 그리드와 플레이어/적 유닛 목록을 관리
/// </summary>
public class Battle_Manager : MonoBehaviour
{
    public const int GridWidth = 10;  // 그리드 너비 (10칸)
    public const int GridHeight = 5;  // 그리드 높이 (5칸)

    public static Battle_Manager instance;  // 싱글톤 인스턴스
    public TileMapCreator tileMapCreator;  // 타일맵 생성기 참조
    public Tile[,] tiles;  // 전투 그리드 타일 배열
    public List<Unit> playerUnits;  // 플레이어 유닛 목록
    public List<Unit> enemyUnits;  // 적 유닛 목록

    /// <summary>
    /// 초기화: 싱글톤 설정 및 그리드 생성
    /// </summary>
    private void Awake()
    {
        instance = this;
        tiles = new Tile[GridWidth, GridHeight];

        // // 유닛 리스트 초기화 (중요!)
        // playerUnits = new List<Unit>();
        // enemyUnits = new List<Unit>();

        tileInit();
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
                tiles[x, y] = tileMapCreator.tiles[x, y];
                if (tiles[x, y] == null)
                {
                    nullCount++;
                    Debug.LogWarning($"[Battle_Manager] tiles[{x}, {y}] is NULL after copying from TileMapCreator");
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
    public void OnBattleEnd()
    {
        // 전투 UI 숨김
        if (Battle_UI_Manager.instance != null)
        {
            Battle_UI_Manager.instance.OnBattleEnd();
        }
        Stage_Manager.instance.CompleteCurrentNode();
    }
}
