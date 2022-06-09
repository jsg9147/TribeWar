using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[System.Serializable]

public class SingleTile : MonoBehaviour
{
    [SerializeField] SpriteRenderer tileImage;
    public Outpost outpost;
    public GameObject outpost_object;
    public bool canSelect;
    public bool isMyMonster;
    public Coordinate coordinate = new Coordinate();


    public CanSpawn canSpawn = CanSpawn.nothing;
    public TileState tileState = TileState.empty;
    public bool clickBlock;

    bool can_TileColor_Change = true;

    Color originColor;
    Color setColor;

    public Vector3 transformPos
    {
        get
        {
            return GetComponent<Transform>().position;
        }
        set
        {
            GetComponent<Transform>().position = transformPos;
        }
    }


    void Start()
    {
        Init();
    }

    void Init()
    {
        clickBlock = false;
    }

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
            tileState = TileState.enermyOutpost;
        }
    }

    public void Outpost_Life_Setup() => outpost.LifeSetup();

    public void SetOriginColor(Color color) => originColor = color;
    public void ChangeTileColor(Color color) => tileImage.material.color = color;
    public void ResetColor() => tileImage.material.color = originColor;

    public void SetMonster(SingleEntity spawnMonster)
    {
        if (spawnMonster.isMine)
        {
            tileState = TileState.onPlayerMonster;
        }
        else
        {
            tileState = TileState.onEnermyEntity;
        }
        isMyMonster = spawnMonster.isMine;
    }

    public void SetupSelectOutpost(bool player)
    {
        if (player)
            tileState = TileState.playerOutpost;
        else
            tileState = TileState.enermyOutpost;
    }

    private void OnMouseEnter()
    {
        if (SingleManager.instance?.clickBlock ?? true)
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

        if (clickBlock)
            return;

        if (SingleEntityManager.instance != null)
        {
            SingleEntityManager.instance.selectTile = this;
        }
    }

    private void OnMouseDown()
    {
        if (SingleManager.instance?.clickBlock ?? true)
            return;
        if (clickBlock)
            return;

        SingleMapManager.instance?.SetupOutpost(this);
    }

    private void OnMouseExit()
    {
        if (SingleManager.instance?.clickBlock ?? true)
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

                if (SingleEntityManager.instance != null)
                {
                    SingleEntityManager.instance.selectTile = null;
                }
            }
        }
        else
        {
            ChangeTileColor(setColor);
        }
        if (SingleEntityManager.instance != null)
        {
            SingleEntityManager.instance.selectTile = null;
        }



    }

    public void ColorChange_Rock(bool isRock, Color color)
    {
        if (isRock)
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
