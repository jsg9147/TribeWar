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
        //Dotween ease �˻� �ϸ� ��� �������� ���� ������ ����
        Sequence sequence = DOTween.Sequence()
            .Append(transform.DOScale(Vector3.one, 0.3f).SetEase(Ease.InOutQuad));
    }

    // ����Ƽ���� ���� ���� �����ϵ��� �ϴ� ���
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
