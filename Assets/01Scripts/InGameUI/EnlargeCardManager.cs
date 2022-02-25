using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class EnlargeCardManager : MonoBehaviour , IPointerDownHandler
{
    [SerializeField] GameObject enlargeCard;
    [SerializeField] EnlargeCardUI cardUIScript;

    public static EnlargeCardManager instance { get; private set; }
    void Awake() => instance = this;


    public void Setup(Card card, bool isFront)
    {
        if (isFront)
        {
            if (enlargeCard.activeSelf == false)
                enlargeCard.SetActive(true);

            cardUIScript.SetCardData(card);
        }
        else
        {
            enlargeCard.SetActive(false);
        }
    }


    public void OnPointerDown(PointerEventData eventData)
    {
        if (enlargeCard.activeSelf)
            enlargeCard.SetActive(false);
    }
}
