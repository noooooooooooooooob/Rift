using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

/// <summary>
/// 전투 그리드를 정의하는 클래스
/// 10x5 크기의 타일 기반 전투 필드를 표현
/// </summary>
public class BattleGrid
{
    public const int Width = 10;  // 그리드 너비 (10칸)
    public const int Height = 5;  // 그리드 높이 (5칸)
    public Tile[,] Tiles = new Tile[Width, Height];  // 타일 배열
}

/// <summary>
/// 전투 시스템의 핵심 매니저 (싱글톤)
/// 전투 그리드와 플레이어/적 유닛 목록을 관리
/// </summary>
public class Battle_Manager : MonoBehaviour
{
    public static Battle_Manager instance;  // 싱글톤 인스턴스
    public TileMapCreator tileMapCreator;  // 타일맵 생성기 참조
    public BattleGrid battleGrid;  // 전투 그리드 데이터
    public List<Unit> playerUnits;  // 플레이어 유닛 목록
    public List<Unit> enemyUnits;  // 적 유닛 목록

    /// <summary>
    /// 초기화: 싱글톤 설정 및 그리드 생성
    /// </summary>
    private void Awake()
    {
        instance = this;
        battleGrid = new BattleGrid();
        battleGrid.Tiles = new Tile[BattleGrid.Width, BattleGrid.Height];

        // 유닛 리스트 초기화 (중요!)
        playerUnits = new List<Unit>();
        enemyUnits = new List<Unit>();

        tileInit();
    }

    /// <summary>
    /// TileMapCreator로부터 타일 참조를 복사하여 그리드 초기화
    /// </summary>
    private void tileInit()
    {
        for(int x = 0; x < BattleGrid.Width; x++)
        {
            for(int y = 0; y < BattleGrid.Height; y++)
            {
                battleGrid.Tiles[x, y] = tileMapCreator.tiles[x, y];
            }
        }
    }
}
