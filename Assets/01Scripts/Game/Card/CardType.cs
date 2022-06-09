using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;

public class CardType
{
    public static int currentId;
    public MoveType moveType;
    public CardCategory card_category;
    public AttackType attack_type;
    public Tribe tribe;
    public Sprite typeIcon;

    public void SetCardCategory(string categoryStr)
    {
        switch (categoryStr)
        {
            case "monster":
                this.card_category = CardCategory.Monster;
                break;
            case "magic":
                this.card_category = CardCategory.Magic;
                break;
            case "trap":
                this.card_category = CardCategory.Trap;
                break;
            default:
                break;
        }
    }

    public void SetRole(string roleStr)
    {
        switch (roleStr)
        {
            case "melee":
                this.attack_type = AttackType.melee;
                break;
            case "shooter":
                this.attack_type = AttackType.shooter;
                break;

            default:
                break;
        }
    }

    public void SetMove(string moveStr)
    {
        switch (moveStr)
        {
            case "rook":
                this.moveType = MoveType.Rook;
                break;
            case "bishop":
                this.moveType = MoveType.Bishop;
                break;
            case "queen":
                this.moveType = MoveType.Queen;
                break;
            default:
                break;
        }
    }

    public void SetTribe(string tribeStr)
    {
        switch (tribeStr)
        {
            case "dragon":
                this.tribe = Tribe.Dragon;
                break;
            case "warrior":
                this.tribe = Tribe.Warrior;
                break;
            case "magician":
                this.tribe = Tribe.Magician;
                break;
            default:
                this.tribe = Tribe.None;
                break;
        }
    }

    public void SetCardType(JSONNode cardData)
    {
        SetMove(cardData["move"]);
        SetTribe(cardData["tribe"]);
        SetCardCategory(cardData["category"]);
        SetRole(cardData["type"]);

        if (card_category == CardCategory.Magic)
            typeIcon = Resources.Load<Sprite>("Images/CardTypeIcon/magic") as Sprite;
        else
            typeIcon = Resources.Load<Sprite>("Images/CardTypeIcon/" + cardData["move"]) as Sprite;
    }
}
