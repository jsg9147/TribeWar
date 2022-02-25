using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class EffectExplanation : MonoBehaviour
{
    [SerializeField] GameObject cardObject;
    [SerializeField] Image character;
    [SerializeField] TMP_Text nameTMP;
    [SerializeField] TMP_Text attackTMP;
    [SerializeField] TMP_Text defenseTMP;
    [SerializeField] TMP_Text explanationTMP;

    public Card card;
    public PRS originPRS;

    public void Setup(Card item)
    {
        if (cardObject.activeSelf == false)
            cardObject.SetActive(true); 

        this.card = item;
        character.sprite = this.card.sprite;
        nameTMP.text = this.card.name;
        if(item.cardType.card_category == CardCategory.Monster)
        {
            attackTMP.text = this.card.GetBaseStat("BP").ToString();
        }
        else
        {
            attackTMP.text = "";
        }
        explanationTMP.text = this.card.card_text.ToString();

        StartCoroutine(HideThisObject(0.5f, item));
    }

    IEnumerator HideThisObject(float time, Card item)
    {
        yield return new WaitForSeconds(time) ;
        if (cardObject.activeSelf)
            cardObject.SetActive(false);

        EnlargeCardManager.instance.Setup(item, true);
    }
}
