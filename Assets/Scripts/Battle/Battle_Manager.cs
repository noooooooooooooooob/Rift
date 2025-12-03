using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class BattleGrid
{
    public const int Width = 10;
    public const int Height = 5;
    public Tile[,] Tiles = new Tile[Width, Height];
}

public class Battle_Manager : MonoBehaviour
{
    public static Battle_Manager instance;
    public TileMapCreator tileMapCreator;
    public BattleGrid battleGrid;
    public List<Unit> playerUnits;
    public List<Unit> enemyUnits;

    private void Awake()
    {
        instance = this;
        battleGrid = new BattleGrid();
        battleGrid.Tiles = new Tile[BattleGrid.Width, BattleGrid.Height];
        tileInit();
    }
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
