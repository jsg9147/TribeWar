using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleJSON;
using System.Linq;

[System.Serializable]

/// <summary>
/// This class represents a cards
/// </summary>
public class Card
{
    public string id; // 이거를 이용해서 카드 위치를 찾는게 좋을듯
    public string name; // 카드 이름
    public CardType cardType = new CardType(); // 카드 타입, 몬스터, 마법, 함정이 있고 moveType 이 들어있음

    // public TypeTest typeTest = new TypeTest();

    public List<Stat> stats = new List<Stat>(); // 공격력과 수비력 스텟

    public int cost;
    public string card_text;
    public AttackType role;

    public Sprite sprite;

    public bool state; // state 클래스 만들고 상태를 지정.. 공격불가 , 파괴, 이동불가 등등

    public Ability ability = new Ability();

    #region Setup
    public Card(JSONNode cardData)
    {
        this.id = cardData["id"];
        this.name = cardData["name"];
        this.cost = cardData["cost"];
        this.card_text = cardData["text"];

        string effectStr = cardData["effect"];
        string effect_target_Str = cardData["effect_target"];

        if (effectStr == null)
            effectStr = "";
        if (effect_target_Str == null)
            effect_target_Str = "";
        Set_CardRole(cardData["type"]);
        cardType.SetCardType(cardData);

        Ability_Init(cardData["effect_time"]);

        ability.SetAbility(id, effectStr, effect_target_Str);

        if (cardType.card_category == CardCategory.Monster)
        {
            SetStat("bp", cardData["battle_power"]);
        }

        sprite = Resources.Load<Sprite>("Images/Card/" + id);
    }

    void Ability_Init(string effect_time)
    {
        switch (effect_time)
        {
            case "trigger":
                ability = new TriggerAbility();
                break;
            case "active":
                ability = new ActivateAbility();
                break;
            case "battle":
                ability = new BattleAbility();
                break;
            case "tribute":
            case "targetTribute":
                ability = new TributeAbility();
                break;
            default:
                ability = new Ability();
                break;
        }
    }


    public Card(Card copyCard)
    {
        this.id = copyCard.id;
        this.name = copyCard.name;
        this.cost = copyCard.cost;
        this.card_text = copyCard.card_text;

        if (copyCard.cardType != null)
        {
            cardType.card_category = copyCard.cardType.card_category;
            cardType.moveType = copyCard.cardType.moveType;
            cardType.tribe = copyCard.cardType.tribe;
        }

        ability = copyCard.ability;

        sprite = copyCard.sprite;

        if (cardType.card_category == CardCategory.Monster)
        {
            SetStat("bp", copyCard.GetBaseStat("bp"));
        }
    }

    void SetStat(string statName, int value)
    {
        var stat = new Stat();
        stat.statID = stats.Count;
        stat.name = statName;
        stat.baseValue = value;
        stat.originalValue = value;
        stat.minValue = 0;
        stat.maxValue = 100000;

        stats.Add(stat);
    }

    public string TribeStr()
    {
        string tribe_Str;
        switch (cardType.tribe)
        {
            case Tribe.Dragon:
                tribe_Str = "드래곤";
                break;
            case Tribe.Magician:
                tribe_Str = "마법사";
                break;
            case Tribe.Warrior:
                tribe_Str = "전사";
                break;
            default:
                tribe_Str = "일반";
                break;
        }
        return tribe_Str;
    }

    void Set_CardRole(string role_Str)
    {
        switch (role_Str)
        {
            case "melee":
                this.cardType.attack_type = AttackType.melee;
                break;
            case "shooter":
                this.cardType.attack_type = AttackType.shooter;
                break;
        }
    }

    public string Role_Str()
    {
        string role_Str;
        switch (cardType.attack_type)
        {
            case AttackType.melee:
                role_Str = "일반";
                break;
            case AttackType.shooter:
                role_Str = "원거리";
                break;

            default:
                role_Str = "일반";
                break;
        }
        return role_Str;
    }

    #endregion

    #region Get Stat Values
    public int GetBaseStat(string statName)
    {
        var stat = stats.Find(x => x.name == statName);
        return stat.baseValue;
    }

    public int GetBaseStat(int statID)
    {
        var stat = stats.Find(x => x.statID == statID);

        return stat.baseValue;
    }

    public int GetEffectiveValue(string statName)
    {
        var stat = stats.Find(x => x.name == statName);

        return stat.effectiveValue;
    }

    public int GeteffectiveValue(int statID)
    {
        var stat = stats.Find(x => x.statID == statID);

        return stat.effectiveValue;
    }

    public void Add_Modifier(Modifier modifier)
    {
        stats.Find(x => x.name == "bp").AddModifier(modifier);
    }

    #endregion

    public void onTurnEnd()
    {
        foreach (var stat in stats)
        {
            stat.OnEndTurn();
        }
    }

}

