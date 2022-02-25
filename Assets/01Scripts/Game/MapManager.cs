using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MapManager : MonoBehaviour
{
    public static MapManager instance { get; private set; }

    void Awake() => instance = this;

    public int Life = 100;

    public PRS originPRS;

    public GameObject TilePrefab;
    public GameObject ParentTile;
    public int mapSize; // 지금 7로 에디터에 입력해놨음
    public float tilePrefabSizeX = 2.5f;
    public float tilePrefabSizeY = 2.5f;

    public int moveMap_x, moveMap_y;

    public Tile[,] mapData;

    
    
    List<List<Tile>> map = new List<List<Tile>>();

    public int outpostMaxCount;
    int outpostCount;
    bool selectOutpostComplete;

    public List<Outpost> player_OutpostList = new List<Outpost>();
    public List<Outpost> opponent_OutpostList = new List<Outpost>();
    public int livePlayerOutpost, liveOpponentOutpost;

    bool isServer, isClient;


    private void Start()
    {
        Setup();
        generateMap();
        ChangeColorOutOfTile(Color.red);
        isServer = NetworkRpcFunc.instance.isServer;
        isClient = NetworkRpcFunc.instance.isClient;
    }

    private void Setup()
    {
        mapData = new Tile[mapSize, mapSize];
        selectOutpostComplete = false;
        outpostCount = 0;
    }

    void generateMap()
    {
        Debug.Log("맵 생성을 시작합니다. ");
        map = new List<List<Tile>>();
        for (int i = 0; i < mapSize; i++)
        {
            List<Tile> row = new List<Tile>();
            for (int j = 0; j < mapSize; j++)
            {
                Tile tile = ((GameObject)Instantiate(TilePrefab, new Vector3((i - Mathf.Floor(mapSize / 2)) * tilePrefabSizeX + moveMap_x , (j - Mathf.Floor(mapSize / 2)) * tilePrefabSizeY + moveMap_y, -30), Quaternion.Euler(new Vector3()))).GetComponent<Tile>();
                tile.name = "(" + i.ToString() + ", " + j.ToString() + ")";
                tile.transform.parent = ParentTile.transform;

                tile.coordinate = new Coordinate(i, j);

                MapColorChange(i, j, tile);
                mapData[i, j] = tile;
                tile.canSelect = true;
                row.Add(tile);
            }
            map.Add(row);
        }
        GameManager.instance.localGamePlayerScript.ChangeLoadingStatus();
    }

    void MapColorChange(int x, int y, Tile tile)
    {
        Color color;
        int calX, calY;
        calX = x % 2;
        calY = y % 2;

        ColorUtility.TryParseHtmlString("#EFDE33", out color);

        if (calX == calY)
        {
            tile.SetOriginColor(color);
            tile.ResetColor();
        }
        else
        {
            tile.SetOriginColor(Color.white);
            tile.ResetColor();
        }

    }

    void ChangeColorOutOfTile(Color color)
    {
        for(int i = 0; i < mapSize; i++)
        {
            for(int j = mapSize; j > (mapSize / 2); j--)
            {
                mapData[i, j - 1].ChangeTileColor(color);

                if(color == Color.red)
                    mapData[i, j - 1].canSelect = false;

                if(color == Color.white)
                    mapData[i, j - 1].canSelect = true;

            }
        }
    }
    public Tile coordinateTile(Coordinate coordinate)
    {
        return mapData[coordinate.x, coordinate.y];
    }
    public void SetupOutpost(Tile tile)
    {
        if (selectOutpostComplete)
            return;

        if (tile.coordinate.y >= mapSize / 2)
            return;

        if (tile.tileState != TileState.empty)
            return;

        Debug.Log("거점선택 좌표 : " + tile.coordinate.vector3Pos);

        outpostCount++;

        tile.SetupSelectOutpost(true);
        GameManager.instance.localGamePlayerScript.CmdSetOutpostPos(tile.coordinate.vector3Pos, isServer);

        if (outpostCount < outpostMaxCount)
        {
            selectOutpostComplete = false;
        }
        else
        {
            selectOutpostComplete = true;
            foreach(var tileSet in mapData)
            {
                tileSet.canSelect = true;
                tileSet.ResetColor();
            }
        }

    }

    public void SetOutpost(Coordinate coordinate, bool server)
    {
        bool isMine = server == isServer;
        Outpost outpost;
        if (isMine == false)
        {
            coordinate.SetReverse(mapSize);
        }

        outpost = coordinateTile(coordinate).outpost;
        coordinateTile(coordinate).OutpostSetActive(Life, isMine);

        if (isMine)
            player_OutpostList.Add(outpost);
        else
            opponent_OutpostList.Add(outpost);

        SetCanSpawnPoint(outpost, isMine);

        if ((player_OutpostList.Count + opponent_OutpostList.Count) == outpostMaxCount * 2)
        {
            foreach (var opponent_Outpost in opponent_OutpostList)
            {
                coordinateTile(opponent_Outpost.coordinate).outpost_object.SetActive(true);
                coordinateTile(opponent_Outpost.coordinate).OutpostSetActive(Life, false);
                coordinateTile(opponent_Outpost.coordinate).Outpost_Life_Setup();
            }
            livePlayerOutpost = player_OutpostList.Count;
            liveOpponentOutpost = opponent_OutpostList.Count;

            if(isServer)
                GameManager.instance.StartGame();
        }
    }

    void SetCanSpawnPoint(Outpost outpost, bool isMine)
    {
        int minX = outpost.coordinate.x - 1;
        int minY = outpost.coordinate.y - 1;
        int maxX = outpost.coordinate.x + 1;
        int maxY = outpost.coordinate.y + 1;

        if (minX <= 0)
            minX = 0;

        if (minY <= 0)
            minY = 0;

        if (maxX >= mapSize)
            maxX = mapSize - 1;

        if (maxY >= mapSize)
            maxY = mapSize - 1;

        for (int i = minX; i <= maxX; i++)
        {
            for (int j = minY; j <= maxY; j++)
            {
                if (isMine)
                    mapData[i, j].canSpawn = CanSpawn.playerCanSpawn;
                else
                    mapData[i, j].canSpawn = CanSpawn.opponentCanSpawn;
            }
        }
    }

    public void OutpostDestroy(Outpost outpost, bool isMine)
    {
        if (isMine)
            livePlayerOutpost--;
        else
            liveOpponentOutpost--;

        int minX = outpost.coordinate.x - 1;
        int minY = outpost.coordinate.y - 1;
        int maxX = outpost.coordinate.x + 1;
        int maxY = outpost.coordinate.y + 1;

        if (minX <= 0)
            minX = 0;

        if (minY <= 0)
            minY = 0;

        if (maxX >= mapSize)
            maxX = mapSize - 1;

        if (maxY >= mapSize)
            maxY = mapSize - 1;

        for (int i = minX; i <= maxX; i++)
        {
            for (int j = minY; j <= maxY; j++)
            {
                mapData[i, j].canSpawn = CanSpawn.nothing;
            }
        }

        if (outpost.isMine)
            player_OutpostList.Remove(outpost);
        else
            opponent_OutpostList.Remove(outpost);

        foreach(Outpost playerOutpost in player_OutpostList)
        {
            SetCanSpawnPoint(playerOutpost, true);
        }

        foreach (Outpost opponentOutpost in opponent_OutpostList)
        {
            SetCanSpawnPoint(opponentOutpost, false);
        }
    }
}
