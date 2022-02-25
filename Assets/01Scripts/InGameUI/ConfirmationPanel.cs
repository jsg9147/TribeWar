using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;

public class ConfirmationPanel : MonoBehaviour
{
    [SerializeField] TMP_Text confirmationTMP;

    public Button confirmBtn;
    public Button cancelBtn;

    public void Show(string msg)
    {
        confirmationTMP.text = msg;
        //Dotween ease 검색 하면 어떻게 나오는지 여러 종류가 있음
        Sequence sequence = DOTween.Sequence()
            .Append(transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InOutQuad));
    }

    // 유니티에서 직접 실행 가능하도록 하는 방법
    [ContextMenu("ScaleOne")]
    void ScaleOne() => transform.localScale = Vector3.one;

    [ContextMenu("ScaleZero")]
    public void ScaleZero() => transform.localScale = Vector3.zero;

    public void ConfirmButton()
    {
        ScaleZero();
        EntityManager.instance.ConfirmSelect(true);
    }

    public void CancelButton()
    {
        ScaleZero();
        EntityManager.instance.ConfirmSelect(false);
    }

    private void Start()
    {
        ScaleZero();
        confirmBtn.onClick.AddListener(() =>
        {
            ConfirmButton();
        });

        cancelBtn.onClick.AddListener(() =>
        {
            CancelButton();
        });
    }
}
