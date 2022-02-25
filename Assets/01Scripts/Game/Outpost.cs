using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Outpost : MonoBehaviour
{
    // Tile 과 Outpost 스크립트 정리가 필요 jsg

    [SerializeField] SpriteRenderer outpost_Image;
    [SerializeField] Sprite MyOutpost;
    [SerializeField] Sprite otherPlayerOutpost;
    [SerializeField] TMP_Text LifeTMP;
    [SerializeField] GameObject OutpostObj;

    public int life;
    public bool isMine;
    public bool isDie;

    // 공격시 entity 좌표를 여기서 가져다 쓰는데 z축이 틀어지는 문제 때문에 0 으로 해줌
    public Vector3 transformPos
    {
        get { return new Vector3(GetComponent<Transform>().position.x, GetComponent<Transform>().position.y, 0); }
        set { GetComponent<Transform>().position = transformPos; }
    }

    public int idx;
    public Coordinate coordinate;

    public void Setup(int _life, bool _isMine, Coordinate _coordinate)
    {
        this.life = _life;
        this.isMine = _isMine;

        if (_isMine)
        {
            outpost_Image.sprite = MyOutpost;
        }
        else
        {
            outpost_Image.sprite = otherPlayerOutpost;
        }

        this.coordinate = _coordinate;
    }

    public void LifeSetup()
    {
        LifeTMP.text = life.ToString();
    }

    public bool Damaged(int damage)
    {

        life -= damage;
        LifeTMP.text = life.ToString();

        if(life <= 0)
        {
            isDie = true;
            OutpostObj.SetActive(false);
            return true;
        }
        return false;
    }
}
