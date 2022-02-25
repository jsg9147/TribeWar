using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]

public class Tile : MonoBehaviour
{
    [SerializeField] SpriteRenderer tileImage;
    public Outpost outpost;
    public GameObject outpost_object;
    Color originColor;
    Color setColor;

    public Vector3 transformPos
    {
        get { return GetComponent<Transform>().position; }
        set { GetComponent<Transform>().position = transformPos; }
    }

    public Coordinate coordinate = new Coordinate();

    public bool canSelect;
    public bool isMyMonster;
    public CanSpawn canSpawn = CanSpawn.nothing;
    public TileState tileState = TileState.empty;

    bool can_TileColor_Change = true;

    // ���� ǥ�ñ��
    public void OutpostSetActive(int life, bool isMine)
    {
        outpost.Setup(life, isMine, coordinate);
        if (isMine)
        {
            tileState = TileState.playerOutpost;
            outpost_object.SetActive(true);
            Outpost_Life_Setup();
        }
        else
        {
            tileState = TileState.opponentOutpost;
        }
    }
    public void Outpost_Life_Setup() => outpost.LifeSetup();

    public void SetOriginColor(Color color) => originColor = color;
    public void ChangeTileColor(Color color) => tileImage.material.color = color;
    public void ResetColor() => tileImage.material.color = originColor;
    
    public void SetMonster(Entity spawnMonster)
    {
        if(spawnMonster.isMine)
        {
            tileState = TileState.onPlayerMonster;
        }
        else
        {
            tileState = TileState.onOpponentMonster;
        }
        isMyMonster = spawnMonster.isMine;
    }

    public void SetupSelectOutpost(bool player)
    {
        // �ڵ� ������ jsg
        if(player)
            tileState = TileState.playerOutpost;
        else
            tileState = TileState.opponentOutpost;
    }

    private void OnMouseEnter()
    {
        if (GameManager.instance?.clickBlock ?? true)
            return;

        if (can_TileColor_Change)
        {
            if (canSelect == false)
            {
                ChangeTileColor(Color.red);
            }
            else
            {
                ChangeTileColor(Color.blue);
            }
        }
        else
        {
            ChangeTileColor(Color.blue);
        }

        ChangeTileColor(Color.blue);

        if (EntityManager.instance != null)
        {
            EntityManager.instance.selectTile = this;
        }
    }

    private void OnMouseDown()
    {
        if (GameManager.instance?.clickBlock ?? true)
            return;

        MapManager.instance?.SetupOutpost(this);

    }

    private void OnMouseExit()
    {
        if (GameManager.instance?.clickBlock ?? true)
            return;

        if (can_TileColor_Change)
        {
            if (canSelect == false)
            {
                ChangeTileColor(Color.red);
            }
            else
            {
                ResetColor();
                if (EntityManager.instance != null)
                {
                    EntityManager.instance.selectTile = null;
                }
            }
        }
        else
        {
            ChangeTileColor(setColor);
        }
        if (EntityManager.instance != null)
        {
            EntityManager.instance.selectTile = null;
        }

        

    }

    // ���߿� ����ȭ�� �ƴ϶� �̵� ����ǥ�ø� ������Ʈ�� �Ұ�� ������Ʈ Ȱ��ȭ�� �ٲٸ� �ɵ�
    // ��ȯ�� ī�� Ŭ���� �̵����� ���� �����ִ� �Լ�
    public void CanMovePos_ChangeTheColor(bool changeColor, Color color)
    {
        if(changeColor)
        {
            ChangeTileColor(color);
            setColor = color;
            can_TileColor_Change = false;
        }
        else
        {
            ResetColor();
            can_TileColor_Change = true;
        }
    }
}
