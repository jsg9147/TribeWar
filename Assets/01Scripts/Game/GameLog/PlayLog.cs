using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayLog : MonoBehaviour
{
    public Image cardImage;
    public TMP_Text nameTMP;
    public TMP_Text coordinateTMP;
    public TMP_Text effectTMP;



    public void Monster_Log(Entity fieldCard, string infoStr = "��ȯ!")
    {
        nameTMP.text = fieldCard.card.name;
        cardImage.sprite = fieldCard.card.sprite;
        effectTMP.text = infoStr;
        coordinateTMP.text = "(" + fieldCard.coordinate.x + ", " + fieldCard.coordinate.y + ")";
    }

    public void Monster_Move_Log(Entity fieldCard, Coordinate before_Coord)
    {
        nameTMP.text = fieldCard.card.name;
        cardImage.sprite = fieldCard.card.sprite;
        coordinateTMP.text = "(" + before_Coord.x + ", " + before_Coord.y + ")";
        effectTMP.text = "�̵� (" + fieldCard.coordinate.x + ", " + fieldCard.coordinate.y + ")";
    }

    public void Magic_Log(Card card, string infoStr = "ȿ�� �ߵ�!")
    {
        nameTMP.text = card.name;
        cardImage.sprite = card.sprite;
        effectTMP.text = infoStr;
        coordinateTMP.text = "";
    }

    public void Effected_Log(Entity fieldCard, string infoStr = "ȿ�� ����")
    {
        nameTMP.text = fieldCard.card.name;
        cardImage.sprite = fieldCard.card.sprite;
        effectTMP.text = infoStr;
        coordinateTMP.text = "(" + fieldCard.coordinate.x + ", " + fieldCard.coordinate.y + ")";
    }

    public void Monster_Battle_Log(Entity card, int beforeBP, int afterBP, string effectStr = "����")
    {
        nameTMP.text = card.card.name;
        cardImage.sprite = card.card.sprite;
        coordinateTMP.text = "(" + card.coordinate.x + ", " + card.coordinate.y + ")";
        if (afterBP == 0)
            effectTMP.text = effectStr + " " + beforeBP + "�� �ı�!";
        else
            effectTMP.text = effectStr + " " + beforeBP + "��" + afterBP;
    }

    public void Outpost_Attack_Log(Entity fieldCard)
    {
        nameTMP.text = fieldCard.card.name;
        cardImage.sprite = fieldCard.card.sprite;
        coordinateTMP.text = "(" + fieldCard.coordinate.x + ", " + fieldCard.coordinate.y + ")";
        effectTMP.text = "���� ����! (" + fieldCard.card.GetEffectiveValue("BP") + ")";
    }

    public void Outpost_Damaged_Log(Outpost outpost, Entity fieldCard)
    {
        // ���� �̹��� ���� ���Ѿ���
        nameTMP.text = fieldCard.card.name;
        cardImage.sprite = fieldCard.card.sprite;
        coordinateTMP.text = "(" + outpost.coordinate.x + ", " + outpost.coordinate.y + ")";
        effectTMP.text = "�ǰ�! (" + (outpost.life + fieldCard.GetEffectiveValue("BP")) + "��" + outpost.life + ")";
    }
}
