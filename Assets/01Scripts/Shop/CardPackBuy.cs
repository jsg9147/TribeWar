using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardPackBuy : MonoBehaviour
{
    public ShopScreen shopScreen;

    CardPack cardPack;

    public CardUI cardPrefab;

    public Image background;
    public Image pack_Image;
    
    public TMP_Text nameText;

    public int card_Count;

    public float nomal_Importance;
    public float rare_Importance;
    public float unique_Importance;

    float all_Importance;

    public void Pack_Setup(CardPack pack, string pack_name, ShopScreen _shop)
    {
        nameText.text = pack_name;
        pack_Image.sprite = Resources.Load<Sprite>("Images/Pack/" + pack.GetPackCode());

        all_Importance = nomal_Importance + rare_Importance + unique_Importance;

        this.cardPack = pack;
        this.shopScreen = _shop;
    }

    public List<int> Random_One_Pack()
    {
        List<int> cards = new List<int>(); 

        for(int i = 0; i < card_Count; i++)
        {
            float random_importance = Random.Range(0f, all_Importance);
            if (random_importance < nomal_Importance)
            {
                cards.Add(cardPack.GetRandomNomal());
            }
            else if(random_importance > nomal_Importance && random_importance < nomal_Importance + rare_Importance)
            {
                cards.Add(cardPack.GetRandomRare());
            }
            else if(random_importance < nomal_Importance + rare_Importance + unique_Importance && random_importance > nomal_Importance + rare_Importance)
            {
                cards.Add(cardPack.GetRandomUnique());
            }
        }

        return cards;
    }

    // 지금 카드 1팩 구매 버튼에 할당중
    public void DrawCastPack()
    {
        shopScreen.DrawScreen_On();
        List<int> pack = Random_One_Pack();
        foreach(int card_number in pack)
        {
            CardUI cardUI = Instantiate(cardPrefab, shopScreen.card_Layout_group);

            Card card = GetCard(card_number);
            cardUI.Setup(card, true, Belong.Draw);

            StartCoroutine(WebMain.instance.web.Set_Buy_Card(card.card_code));
        }
    }

    Card GetCard(int card_number)
    {
        string card_code;
        string card_numStr = string.Format("{0:D3}", card_number);

        card_code = cardPack.GetPackCode() + "-" + card_numStr;
        return CardDatabase.instance.CardData(card_code);
    }

    public void Buy_Ten_Pack()
    {

    }
}
