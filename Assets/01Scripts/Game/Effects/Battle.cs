using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Battle 
{
    CardMove cardMove = new CardMove();
    EffectsSolver effects = new EffectsSolver();
    
    // 일반 배틀
    public void Attack(Entity attacker, Entity defender)
    {
        int attackerBP, defenderBP;
        attackerBP = attacker.GetEffectiveValue("BP");
        defenderBP = defender.GetEffectiveValue("BP");

        if (defender.card.role == CardRole.shooter)
            attackerBP = attackerBP * 2;

        attacker.GetComponent<Order>().SetMostFrontOrder(true);

        Vector3 attackPosition = defender.transformPos;
        ResolverEffect(attacker, defender);

        Sequence sequence = DOTween.Sequence()
        .Append(attacker.transform.DOMove(attackPosition, 0.4f)).SetEase(Ease.InSine)
        .AppendCallback(() =>
        {
            if(attacker.card.role != CardRole.shooter)
                attacker.Damaged(defenderBP);

            defender.Damaged(attackerBP);
            PlayLogControl.instance.Log_Sorter(LogCategory.Attack, attacker, attackerBP, attacker.GetEffectiveValue("BP"));
            PlayLogControl.instance.Log_Sorter(LogCategory.Defend, defender, defenderBP, defender.GetEffectiveValue("BP"));
        }) .OnComplete(() => AttackCallback(attacker, defender)); //죽음 처리
    }

    // 거점 공격
    public void Attack(Entity attacker, Outpost outpost, bool server)
    {
        attacker.GetComponent<Order>().SetMostFrontOrder(true);

        Vector3 attackPosition = outpost.transformPos;

        Sequence sequence = DOTween.Sequence()
        .Append(attacker.transform.DOMove(attackPosition, 0.4f)).SetEase(Ease.InSine)
        .AppendCallback(() =>
        {
            outpost.Damaged(attacker.GetEffectiveValue("BP"));
            PlayLogControl.instance.Log_Sorter(LogCategory.Outpost_Attack, attacker, outpost);
        }).OnComplete(() => OutpostAttackCallback(attacker, outpost)); //죽음 처리
    }

    // 파괴 했다면 이동 시켜야함 
    void AttackCallback(Entity attacker, Entity defender)
    {
        attacker.GetComponent<Order>().SetMostFrontOrder(false);

        if (defender.isDie)
        {
            defender.bottomTile.tileState = TileState.empty;

            if(attacker.isDie == false)
            {
                Sequence sequence = DOTween.Sequence()
                .Append(defender.transform.DOShakePosition(0.5f))
                .Append(defender.transform.DOScale(Vector3.one, 0.3f)).SetEase(Ease.OutCirc)
                .Append(attacker.transform.DOMove(defender.transformPos, 0.1f).SetEase(Ease.InSine))
                .AppendCallback(() =>
                {
                    if (attacker.card.role != CardRole.shooter)
                        cardMove.Move(attacker, defender.bottomTile);
                    else
                        attacker.MoveTransform(attacker.bottomTile.transformPos, true, 0.5f);
                });
            }
            else
            {
                attacker.bottomTile.tileState = TileState.empty;
            }
        }
        else
        {
            Sequence sequence = DOTween.Sequence()
                .Append(attacker.transform.DOShakePosition(0.5f))
                .AppendCallback(() =>
                {
                    if (attacker.card.role != CardRole.shooter)
                        cardMove.AfterAttackMove(attacker, defender.coordinate);
                    else
                        attacker.MoveTransform(attacker.bottomTile.transformPos, true, 0.5f);
                });
        }

        attacker.attackable = false;
        EntityManager.instance.UpdateEntityState();
    }

    void OutpostAttackCallback(Entity attacker, Outpost outpost)
    {
        Tile outpostTile = MapManager.instance.mapData[outpost.coordinate.x, outpost.coordinate.y];

        attacker.GetComponent<Order>().SetMostFrontOrder(false);

        if (outpost.isDie)
        {
            outpostTile.tileState = TileState.empty;

            Sequence sequence = DOTween.Sequence()
                .Append(outpost.transform.DOShakePosition(0.5f))
                .Append(attacker.transform.DOMove(outpost.transformPos, 0.1f).SetEase(Ease.InSine))
                .AppendCallback(() =>
                {
                    if(attacker.card.role != CardRole.shooter)
                        cardMove.Move(attacker, outpostTile);

                    // 추후 변경해야할 동작방식, 거점 추가할때 리스트에 넣었다가 제거하는 방식 jsg
                    if (attacker.isMine)
                        MapManager.instance.OutpostDestroy(outpost, attacker.isMine);
                    else
                        MapManager.instance.OutpostDestroy(outpost, attacker.isMine);

                    outpostTile.outpost_object.SetActive(false);
                    CheckGameResult();
                });
        }
        else
        {
            Sequence sequence = DOTween.Sequence()
                .Append(outpost.transform.DOShakePosition(0.5f))
                .AppendCallback(() =>
                {
                    if (attacker.card.role != CardRole.shooter)
                       cardMove.AfterAttackMove(attacker, outpostTile.coordinate);
                    else
                        attacker.MoveTransform(attacker.bottomTile.transformPos, true, 0.5f);
                });
        }
        attacker.attackable = false;
        EntityManager.instance.UpdateEntityState();
    }

    void ResolverEffect(Entity attacker, Entity defender)
    {
        if (attacker.card.ability.effect_Class == EffectClass.Battle)
        {
            if(attacker.card.ability.target.GetTarget() == EffectTarget.TargetCard)
            {
                attacker.card.ability.effect.Resolve(defender);
            }
            else if(attacker.card.ability.target.GetTarget() == EffectTarget.ThisCard)
            {
                attacker.card.ability.effect.Resolve(attacker);
            }    
        }

        if (defender.card.ability.effect_Class == EffectClass.Battle)
        {
            if (defender.card.ability.target.GetTarget() == EffectTarget.TargetCard)
            {
                defender.card.ability.effect.Resolve(attacker);
            }
            else if (defender.card.ability.target.GetTarget() == EffectTarget.ThisCard)
            {
                defender.card.ability.effect.Resolve(defender);
            }
        }
    }

    void CheckGameResult()
    {
        if (MapManager.instance.livePlayerOutpost <= 0)
            GameManager.instance.GameResult(true);

        if (MapManager.instance.liveOpponentOutpost <= 0)
            GameManager.instance.GameResult(false);
    }
}
