using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPossibleMove
{
    int CurrentX, CurrentY;
    int mapSize;
    List<Coordinate> canMovePointList;
    public List<Coordinate> Rook_Distance(Entity entity, bool attack = false)
    {
        int moveSpace;
        mapSize = MapManager.instance.mapSize;
        Tile[,] mapData = MapManager.instance.mapData;
        
        canMovePointList = new List<Coordinate>();

        CurrentX = entity.coordinate.x;
        CurrentY = entity.coordinate.y;

        moveSpace = entity.card.cost + 1;

        if(!attack && entity.card.role == CardRole.shooter)
        {
            moveSpace = 1;
        }

        int i = CurrentX + 1;

        // Right        

        while (true)
        {
            if (i >= mapSize || i - CurrentX > moveSpace)
                break;

            Tile rightDir = mapData[i, CurrentY];

            if (rightDir.tileState == TileState.empty)
            {
                canMovePointList.Add(new Coordinate(i, CurrentY));
            }
            else
            {
                if (rightDir.tileState == TileState.onOpponentMonster || rightDir.tileState == TileState.opponentOutpost)
                {
                    canMovePointList.Add(new Coordinate(i, CurrentY));
                }

                break;
            }
            i++;
        }

        // Left        
        i = CurrentX - 1;
        while (true)
        {
            if (i < 0 || CurrentX - i > moveSpace)
                break;

            Tile leftDir = mapData[i, CurrentY];

            if (leftDir.tileState == TileState.empty)
            {
                canMovePointList.Add(new Coordinate(i, CurrentY));
            }
            else
            {
                if (leftDir.tileState == TileState.onOpponentMonster || leftDir.tileState == TileState.opponentOutpost)
                {
                    canMovePointList.Add(new Coordinate(i, CurrentY));
                }

                break;
            }
            i--;
        }

        // Up        
        i = CurrentY + 1;
        while (true)
        {
            if (i >= mapSize || i - CurrentY > moveSpace)
                break;

            Tile upDir = mapData[CurrentX, i];

            if (upDir.tileState == TileState.empty)
            {
                canMovePointList.Add(new Coordinate(CurrentX, i));
            }
            else
            {
                if (upDir.tileState == TileState.onOpponentMonster || upDir.tileState == TileState.opponentOutpost)
                {
                    canMovePointList.Add(new Coordinate(CurrentX, i));
                }

                break;
            }
            i++;
        }

        // Down
        i = CurrentY - 1;
        while (true)
        {
            if (i < 0 || CurrentY - i > moveSpace)
                break;

            Tile downDir = mapData[CurrentX, i];

            if (downDir.tileState == TileState.empty)
            {
                canMovePointList.Add(new Coordinate(CurrentX, i));
            }
            else
            {
                if (downDir.tileState == TileState.onOpponentMonster || downDir.tileState == TileState.opponentOutpost)
                {
                    canMovePointList.Add(new Coordinate(CurrentX, i));
                }

                break;
            }
            i--;
        }
        return canMovePointList;
    }

    public List<Coordinate> Bishop_Distance(Entity entity, bool attack = false)
    {
        mapSize = MapManager.instance.mapSize;
        Tile[,] mapData = MapManager.instance.mapData;

        canMovePointList = new List<Coordinate>();

        CurrentX = entity.coordinate.x;
        CurrentY = entity.coordinate.y;

        int i = CurrentX + 1;
        int j = CurrentY + 1;

        int moveSpace = (entity.card.cardType.moveType == MoveType.Queen) ? entity.card.cost : entity.card.cost + 1;

        if (!attack && entity.card.role == CardRole.shooter)
        {
            moveSpace = 1;
        }
        // Right Up
        while (true)
        {
            if (i >= mapSize || j >= mapSize)
                break;

            if (i - CurrentX > moveSpace || j - CurrentY > moveSpace)
                break;

            Tile rightUp = mapData[i, j];

            if (rightUp.tileState == TileState.empty)
            {
                canMovePointList.Add(new Coordinate(i, j));
            }
            else
            {
                if (rightUp.tileState == TileState.onOpponentMonster || rightUp.tileState == TileState.opponentOutpost)
                {
                    canMovePointList.Add(new Coordinate(i, j));
                }

                break;
            }
            i++;
            j++;
        }

        // Left Up

        i = CurrentX - 1;
        j = CurrentY + 1;
        
        while (true)
        {
            if (i < 0 || j >= mapSize)
                break;

            if (CurrentX - i > moveSpace || j - CurrentY > moveSpace)
                break;

            Tile leftUp = mapData[i, j];

            if (leftUp.tileState == TileState.empty)
            {
                canMovePointList.Add(new Coordinate(i, j));
            }
            else
            {
                if (leftUp.tileState == TileState.onOpponentMonster || leftUp.tileState == TileState.opponentOutpost)
                {
                    canMovePointList.Add(new Coordinate(i, j));
                }

                break;
            }
            i--;
            j++;
        }

        // Right Down

        i = CurrentX + 1;
        j = CurrentY - 1;

        while (true)
        {
            if (i >= mapSize || j < 0)
                break;

            if (i - CurrentX > moveSpace || CurrentY - j > moveSpace)
                break;

            Tile rightDown = mapData[i, j];

            if (rightDown.tileState == TileState.empty)
            {
                canMovePointList.Add(new Coordinate(i, j));
            }
            else
            {
                if (rightDown.tileState == TileState.onOpponentMonster || rightDown.tileState == TileState.opponentOutpost)
                {
                    canMovePointList.Add(new Coordinate(i, j));
                }

                break;
            }
            i++;
            j--;
        }

        // Left Down

        i = CurrentX - 1;
        j = CurrentY - 1;

        while (true)
        {
            if (i < 0 || j < 0)
                break;

            if (CurrentX - i > moveSpace || CurrentY - j > moveSpace)
                break;

            Tile leftDown = mapData[i, j];

            if (leftDown.tileState == TileState.empty)
            {
                canMovePointList.Add(new Coordinate(i, j));
            }
            else
            {
                if (leftDown.tileState == TileState.onOpponentMonster || leftDown.tileState == TileState.opponentOutpost)
                {
                    canMovePointList.Add(new Coordinate(i, j));
                }

                break;
            }
            i--;
            j--;
        }

        return canMovePointList;
    }

}
