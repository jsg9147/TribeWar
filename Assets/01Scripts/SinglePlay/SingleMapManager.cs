using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SingleMapManager : MonoBehaviour
{
    public static SingleMapManager instance
    {
        get; private set;
    }

    void Awake() => instance = this;

    public int Life = 100;

    public PRS originPRS;

    public GameObject TilePrefab;
    public GameObject ParentTile;
    public int mapSize;
    public float tilePrefabSizeX = 2.5f;
    public float tilePrefabSizeY = 2.5f;

    public int moveMap_x, moveMap_y;

    public SingleTile[,] mapData;

    List<List<SingleTile>> map = new List<List<SingleTile>>();

    public int outpostMaxCount;
    int outpostCount;
    public bool select_Outpost_Complete;

    public List<Outpost> player_OutpostList = new List<Outpost>();
    public List<Outpost> opponent_OutpostList = new List<Outpost>();
    public int livePlayerOutpost, liveOpponentOutpost;

    public List<Coordinate> canSummonPos;
    public List<Coordinate> canAI_SummonPos;

    //private void Start()
    //{
    //    Init();
    //}

    //private void Init()
    //{
    //    mapData = new SingleTile[mapSize, mapSize];

    //    canSummonPos = new List<Coordinate>();
    //    canAI_SummonPos = new List<Coordinate>();

    //    select_Outpost_Complete = false;
    //    outpostCount = 0;

    //    generateMap();
    //    ChangeColorOutOfTile(Color.red);
    //    Set_Tutorial_Outpost();
    //    //Set_AI_Outpost();
    //}

    //void generateMap()
    //{
    //    Debug.Log("Map is Generated. ");
    //    map = new List<List<SingleTile>>();
    //    for (int i = 0; i < mapSize; i++)
    //    {
    //        List<SingleTile> row = new List<SingleTile>();
    //        for (int j = 0; j < mapSize; j++)
    //        {
    //            SingleTile tile =
    //                ((GameObject)Instantiate
    //                (TilePrefab,
    //                new Vector3((i - Mathf.Floor(mapSize / 2)) * tilePrefabSizeX + moveMap_x,
    //                (j - Mathf.Floor(mapSize / 2)) * tilePrefabSizeY + moveMap_y, -30), Quaternion.Euler(new Vector3()))).GetComponent<SingleTile>();
    //            tile.name = "(" + i.ToString() + ", " + j.ToString() + ")";
    //            tile.transform.parent = ParentTile.transform;

    //            tile.coordinate = new Coordinate(i, j);

    //            MapColorChange(i, j, tile);
    //            mapData[i, j] = tile;
    //            tile.canSelect = true;
    //            row.Add(tile);
    //        }
    //        map.Add(row);
    //    }
    //}

    //void MapColorChange(int x, int y, SingleTile tile)
    //{
    //    Color color;
    //    int calX, calY;
    //    calX = x % 2;
    //    calY = y % 2;

    //    ColorUtility.TryParseHtmlString("#EFDE33", out color);

    //    if (calX == calY)
    //    {
    //        tile.SetOriginColor(color);
    //        tile.ResetColor();
    //    }
    //    else
    //    {
    //        tile.SetOriginColor(Color.white);
    //        tile.ResetColor();
    //    }

    //}

    //void ChangeColorOutOfTile(Color color)
    //{
    //    for (int i = 0; i < mapSize; i++)
    //    {
    //        for (int j = mapSize; j > (mapSize / 2); j--)
    //        {
    //            mapData[i, j - 1].ChangeTileColor(color);

    //            if (color == Color.red)
    //                mapData[i, j - 1].canSelect = false;

    //            if (color == Color.white)
    //                mapData[i, j - 1].canSelect = true;

    //        }
    //    }
    //}

    //public void Tile_ClickBlock(bool isBlock)
    //{
    //    foreach (var tile in mapData)
    //    {
    //        tile.clickBlock = isBlock;
    //    }
    //}

    //public void Tile_Color_Reset()
    //{
    //    foreach (var tile in mapData)
    //    {
    //        tile.ResetColor();
    //    }
    //}
    //public SingleTile coordinateTile(Coordinate coordinate)
    //{
    //    return mapData[coordinate.x, coordinate.y];
    //}

    //public void SetupOutpost(SingleTile tile)
    //{
    //    if (select_Outpost_Complete)
    //        return;

    //    if (tile.coordinate.y >= mapSize / 2)
    //        return;

    //    if (tile.tileState != TileState.empty)
    //        return;

    //    outpostCount++;

    //    tile.SetupSelectOutpost(true);
    //    SetOutpost(tile.coordinate, true);

    //    if (outpostCount < outpostMaxCount)
    //    {
    //        select_Outpost_Complete = false;
    //    }
    //    else
    //    {
    //        select_Outpost_Complete = true;
    //        foreach (var tileSet in mapData)
    //        {
    //            tileSet.canSelect = true;
    //            tileSet.ResetColor();
    //        }
    //    }
    //}

    //void Set_Tutorial_Outpost()
    //{
    //    Coordinate AI_Outpost1 = new Coordinate(4, 6);
    //    Coordinate AI_Outpost2 = new Coordinate(5, 6);
    //    SetOutpost(AI_Outpost1, false);
    //    SetOutpost(AI_Outpost2, false);
    //}

    //void Set_AI_Outpost()
    //{
    //    Coordinate randCoord;
    //    for (int i = 0; i < outpostMaxCount; i++)
    //    {
    //        int x = Random.Range(0, mapSize);
    //        int y = Random.Range(mapSize / 2, mapSize);

    //        randCoord = new Coordinate(x, y);

    //        SetOutpost(randCoord, false);
    //    }

    //}

    //public void SetOutpost(Coordinate coordinate, bool isMine)
    //{
    //    Outpost outpost;

    //    outpost = coordinateTile(coordinate).outpost;
    //    coordinateTile(coordinate).OutpostSetActive(Life, isMine);

    //    if (isMine)
    //        player_OutpostList.Add(outpost);
    //    else
    //        opponent_OutpostList.Add(outpost);

    //    SetCanSpawnPoint(outpost, isMine);

    //    if (player_OutpostList.Count == outpostMaxCount)
    //        Can_Summon_Tile_Display(false);

    //    if ((player_OutpostList.Count + opponent_OutpostList.Count) == outpostMaxCount * 2)
    //    {
    //        foreach (var opponent_Outpost in opponent_OutpostList)
    //        {
    //            coordinateTile(opponent_Outpost.coordinate).outpost_object.SetActive(true);
    //            coordinateTile(opponent_Outpost.coordinate).OutpostSetActive(Life, false);
    //            coordinateTile(opponent_Outpost.coordinate).Outpost_Life_Setup();
    //        }
    //        livePlayerOutpost = player_OutpostList.Count;
    //        liveOpponentOutpost = opponent_OutpostList.Count;

    //        SingleManager.instance.StartGame();
    //    }
    //}

    //void SetCanSpawnPoint(Outpost outpost, bool isMine)
    //{
    //    int minX = outpost.coordinate.x - 1;
    //    int minY = outpost.coordinate.y - 1;
    //    int maxX = outpost.coordinate.x + 1;
    //    int maxY = outpost.coordinate.y + 1;

    //    if (minX <= 0)
    //        minX = 0;

    //    if (minY <= 0)
    //        minY = 0;

    //    if (maxX >= mapSize)
    //        maxX = mapSize - 1;

    //    if (maxY >= mapSize)
    //        maxY = mapSize - 1;

    //    for (int i = minX; i <= maxX; i++)
    //    {
    //        for (int j = minY; j <= maxY; j++)
    //        {
    //            if (isMine)
    //            {
    //                mapData[i, j].canSpawn = CanSpawn.playerCanSpawn;
    //                canSummonPos.Add(new Coordinate(i, j));
    //            }
    //            else
    //            {
    //                mapData[i, j].canSpawn = CanSpawn.opponentCanSpawn;
    //                canAI_SummonPos.Add(new Coordinate(i, j));
    //            }
    //        }
    //    }
    //}

    //public void Can_Summon_Tile_Display(bool isDisplay)
    //{
    //    foreach (var coord in canSummonPos)
    //    {
    //        if (isDisplay)
    //        {
    //            mapData[coord.x, coord.y].ColorChange_Rock(true, Color.green);

    //        }
    //        else
    //        {
    //            mapData[coord.x, coord.y].ColorChange_Rock(false, Color.white);

    //        }
    //        mapData[coord.x, coord.y].clickBlock = true;
    //    }
    //}

    //public void OutpostDestroy(Outpost outpost, bool isMine)
    //{
    //    if (isMine)
    //        livePlayerOutpost--;
    //    else
    //        liveOpponentOutpost--;

    //    int minX = outpost.coordinate.x - 1;
    //    int minY = outpost.coordinate.y - 1;
    //    int maxX = outpost.coordinate.x + 1;
    //    int maxY = outpost.coordinate.y + 1;

    //    if (minX <= 0)
    //        minX = 0;

    //    if (minY <= 0)
    //        minY = 0;

    //    if (maxX >= mapSize)
    //        maxX = mapSize - 1;

    //    if (maxY >= mapSize)
    //        maxY = mapSize - 1;

    //    for (int i = minX; i <= maxX; i++)
    //    {
    //        for (int j = minY; j <= maxY; j++)
    //        {
    //            mapData[i, j].canSpawn = CanSpawn.nothing;
    //        }
    //    }

    //    if (outpost.isMine)
    //        player_OutpostList.Remove(outpost);
    //    else
    //        opponent_OutpostList.Remove(outpost);

    //    foreach (Outpost playerOutpost in player_OutpostList)
    //    {
    //        SetCanSpawnPoint(playerOutpost, true);
    //    }

    //    foreach (Outpost opponentOutpost in opponent_OutpostList)
    //    {
    //        SetCanSpawnPoint(opponentOutpost, false);
    //    }
    //}
}
