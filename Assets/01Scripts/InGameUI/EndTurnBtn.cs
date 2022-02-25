using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndTurnBtn : MonoBehaviour
{
    [SerializeField] Sprite actirve;
    [SerializeField] Sprite inActive;
    [SerializeField] TMP_Text btnText;
    // Start is called before the first frame update
    void Start()
    {
        Setup(false);
        TurnManager.OnTurnStarted += Setup;
    }

    private void OnDestroy()
    {
        TurnManager.OnTurnStarted -= Setup;
    }

    public void Setup(bool isActive)
    {
        GetComponent<Image>().sprite = isActive ? actirve : inActive;
        GetComponent<Button>().interactable = isActive;
        btnText.color = isActive ? Color.black : Color.yellow;
    }
}
