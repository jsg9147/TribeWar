using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Rarity
{
    nomal,
    rare,
    unique
}

public class CardPack
{
    string packCode;

    List<int> nomal;
    List<int> rare;
    List<int> unique;

    public void CardPack_Data_Setup(string _code)
    {
        this.packCode = _code;

        nomal = new List<int>();
        rare = new List<int>();
        unique = new List<int>();
    }

    public string GetPackCode()
    {
        return packCode;
    }

    public void AddCard(int card_Num, string rarity)
    {
        switch(rarity)
        {
            case "nomal":
                nomal.Add(card_Num);
                break;
            case "rare":
                rare.Add(card_Num);
                break;
            case "unique":
                unique.Add(card_Num);
                break;
        }
    }

    public List<int> GetNomalList()
    {
        return nomal;
    }
    public List<int> GetRareList()
    {
        return rare;
    }

    public List<int> GetUniqueList()
    {
        return unique;
    }

    public int GetRandomNomal()
    {
        int random = Random.Range(0, nomal.Count);
        return nomal[random];
    }
    public int GetRandomRare()
    {
        int random = Random.Range(0, rare.Count);
        return rare[random];
    }
    public int GetRandomUnique()
    {
        int random = Random.Range(0, unique.Count);
        return unique[random];
    }
}
