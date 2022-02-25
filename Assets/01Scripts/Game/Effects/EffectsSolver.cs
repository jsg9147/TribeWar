using System.Collections.Generic;
using UnityEngine;

public class EffectsSolver
{
    public bool randomPlayer;
    public int randomEntityIndex = 0;

    public List<Ability> player_Activated_Abilities = new List<Ability>();
    public List<Ability> opponent_Activated_Abilities = new List<Ability>();

    // 논타겟 효과 발동분류, 단순 발동은 그냥 발동 시키게 해야하고 
    // 공격력 증가같은건 Increase/attack/100 이렇게 구성되어서
    // 증가 -> 스텟 지정 -> 수치 이 순서가 되도록 해야함
    public void NonTargetEffectActive(Card card, List<Entity> playerEntities, List<Entity> opponentEntities, bool server)
    {
        switch (card.ability.target.GetTarget())
        {
            case EffectTarget.RandomCard:
                GameManager.instance.localGamePlayerScript.CmdRandomTargetAppoint(card.card_code, server);
                break;
            case EffectTarget.AllCards:
                AllCardsEffectTrigger(card, playerEntities, opponentEntities, server);
                break;
            default:
                Debug.Log("디폴트임");
                break;
        }
    }


    void AllCardsEffectTrigger(Card effectCard, List<Entity> playerEntities, List<Entity> opponentEntities, bool server)
    {
        List<Entity> allEntities = new List<Entity>(playerEntities);
        allEntities.AddRange(opponentEntities);

        foreach(var entity in allEntities)
        {
            effectCard.ability.effect.Resolve(entity);
        }
    }

    public void ReceiveRandomEffect(Entity entity, string card_id)
    {
        Card effectCard = CardDatabase.instance.CardData(card_id);

        effectCard.ability.effect.Resolve(entity);
    }

    // 효과 발동 가능 여부
    public bool EffectRequireExist(bool netReceive , List<Entity> playerEntities,List<Entity> opponentEntities, Card triggerCard)
    {
        List<Entity> targetEntities = netReceive ? opponentEntities : playerEntities;

        if (triggerCard.cardType.tribe == Tribe.None)
            return true;

        foreach(var entity in targetEntities)
        {
            if (entity.card.cardType.tribe == triggerCard.cardType.tribe)
            {
                return true;
            }
        }
        
        return false;
    }

    public void Activated_Effect(Ability ability,bool isMine, List<Entity> playerEntities, List<Entity> opponentEntities)
    {
        List<Entity> allEntities = new List<Entity>(playerEntities);
        allEntities.AddRange(opponentEntities);

        foreach(var entity in allEntities)
        {
            if(ability.target.GetTarget() == EffectTarget.PlayerWarrior)
            {
                if(entity.card.cardType.tribe == Tribe.Warrior && entity.isMine == isMine)
                {
                    entity.Add_Apply_Effect(ability);
                }
            }
        }
    }

    public void Add_Activated_Effect_To_Entity(Entity entity)
    { 
        foreach (var ability in player_Activated_Abilities)
        {
            if (ability.target.GetTarget() == EffectTarget.PlayerWarrior)
            {
                if (entity.card.cardType.tribe == Tribe.Warrior && entity.isMine == true)
                {
                    entity.Add_Apply_Effect(ability);
                }
            }
        }

        foreach (var ability in opponent_Activated_Abilities)
        {
            if (ability.target.GetTarget() == EffectTarget.PlayerWarrior)
            {
                if (entity.card.cardType.tribe == Tribe.Warrior && entity.isMine == false)
                {
                    entity.Add_Apply_Effect(ability);
                }
            }
        }
    }

    public void ReverseEffect(Entity effectEntity, List<Entity> playerEntities, List<Entity> opponentEntities)
    {
        RemoveActivatedAbilities(effectEntity);

        if (effectEntity.card.ability.effect_Class == EffectClass.Activated && effectEntity.isMine)
        {
            if (effectEntity.card.ability.target.GetTarget() == EffectTarget.PlayerWarrior)
            {
                EntityListApplyEffectCancel(effectEntity.card, playerEntities, Tribe.Warrior);
            }
            else if (effectEntity.card.ability.target.GetTarget() == EffectTarget.PlayerDragon)
            {
                EntityListApplyEffectCancel(effectEntity.card, playerEntities, Tribe.Dragon);
            }
        }
        else
        {
            if (effectEntity.card.ability.target.GetTarget() == EffectTarget.PlayerWarrior)
            {
                EntityListApplyEffectCancel(effectEntity.card, opponentEntities, Tribe.Warrior);
            }
            else if (effectEntity.card.ability.target.GetTarget() == EffectTarget.PlayerDragon)
            {
                EntityListApplyEffectCancel(effectEntity.card, opponentEntities, Tribe.Dragon);
            }
        }
    }

    void RemoveActivatedAbilities(Entity effectEntity)
    {
        var targetAbilities = effectEntity.isMine ? player_Activated_Abilities : opponent_Activated_Abilities;

        Ability removeAbility = targetAbilities.Find(x => x.Tag == effectEntity.card.ability.Tag);

        targetAbilities.Remove(removeAbility);
    }

    void EntityListApplyEffectCancel(Card effectCard, List<Entity> entities, Tribe tribe)
    {
        foreach (var entity in entities)
        {
            if (entity.card.cardType.tribe == tribe)
            {
                effectCard.ability.effect.Reverse(entity);
            }
        }
    }
}
