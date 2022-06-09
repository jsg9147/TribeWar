using System.Collections.Generic;
using UnityEngine;

public class SingleEffectsSolver
{
    public bool randomPlayer;
    public int randomEntityIndex = 0;

    public List<Ability> player_Activated_Abilities = new List<Ability>();
    public List<Ability> opponent_Activated_Abilities = new List<Ability>();

    public void NonTargetEffectActive(Card card, List<SingleEntity> playerEntities, List<SingleEntity> opponentEntities)
    {
        switch (card.ability.target.GetTarget())
        {
            case EffectTarget.RandomCard:
                RandomTargetAppoint(playerEntities, opponentEntities);
                break;
            case EffectTarget.AllCards:
                AllCardsEffectTrigger(card, playerEntities, opponentEntities);
                break;
            case EffectTarget.PlayerWarrior:
                Player_Warrior_EffectTrigger(card, playerEntities);
                break;
            default:
                Debug.Log("Non Target Func, default..");
                break;
        }
    }

    void RandomTargetAppoint(List<SingleEntity> playerEntities, List<SingleEntity> opponentEntities)
    {
        List<SingleEntity> allEntities = new List<SingleEntity>(playerEntities);
        allEntities.AddRange(opponentEntities);

        int randomIndex = Random.Range(0, allEntities.Count - 1);

        var targetEntity = allEntities[randomIndex];

        SingleEntityManager.instance.RandomTargetEffect(targetEntity);
    }


    void AllCardsEffectTrigger(Card effectCard, List<SingleEntity> playerEntities, List<SingleEntity> opponentEntities)
    {
        List<SingleEntity> allEntities = new List<SingleEntity>(playerEntities);
        allEntities.AddRange(opponentEntities);

        foreach (var entity in allEntities)
        {
            foreach (var effect in effectCard.ability.effects)
            {
                effect.Resolve(entity);
            }
        }
    }

    void Player_Warrior_EffectTrigger(Card effectCard, List<SingleEntity> playerEntities)
    {
        foreach (var entity in playerEntities)
        {
            foreach (var effect in effectCard.ability.effects)
            {
                if (entity.card.cardType.tribe == Tribe.Warrior)
                    effect.Resolve(entity);
            }
        }
    }

    public void ReceiveRandomEffect(SingleEntity entity, string card_id)
    {
        Card effectCard = CardDatabase.instance.CardData(card_id);

        foreach (var effect in effectCard.ability.effects)
        {
            effect.Resolve(entity);
        }
    }

    public bool EffectRequireExist(bool netReceive, List<SingleEntity> playerEntities, List<SingleEntity> opponentEntities, Card triggerCard)
    {
        List<SingleEntity> targetEntities = netReceive ? opponentEntities : playerEntities;

        if (triggerCard.cardType.tribe == Tribe.None)
            return true;

        foreach (var entity in targetEntities)
        {
            if (entity.card.cardType.tribe == triggerCard.cardType.tribe)
            {
                return true;
            }
        }

        return false;
    }

    public void Activated_Effect(Ability ability, bool isMine, List<SingleEntity> playerEntities, List<SingleEntity> opponentEntities)
    {
        List<SingleEntity> allEntities = new List<SingleEntity>(playerEntities);
        allEntities.AddRange(opponentEntities);

        foreach (var entity in allEntities)
        {
            if (ability.target.GetTarget() == EffectTarget.PlayerWarrior)
            {
                if (entity.card.cardType.tribe == Tribe.Warrior && entity.isMine == isMine)
                {
                    entity.Add_Apply_Effect(ability);
                }
            }
        }
    }

    public void Add_Activated_Effect_To_Entity(SingleEntity entity)
    {
        CardType cardType = entity.card.cardType;

        foreach (var ability in player_Activated_Abilities)
        {
            EffectTarget target = ability.target.GetTarget();
            if (target == EffectTarget.PlayerWarrior)
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
                if (cardType.tribe == Tribe.Warrior && entity.isMine == false)
                {
                    entity.Add_Apply_Effect(ability);
                }
            }
        }
    }

    public void ReverseEffect(SingleEntity effectEntity, List<SingleEntity> playerEntities, List<SingleEntity> opponentEntities)
    {
        RemoveActivatedAbilities(effectEntity);

        if (effectEntity.card.ability.effect_Time == EffectTime.Activated && effectEntity.isMine)
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

    void RemoveActivatedAbilities(SingleEntity effectEntity)
    {
        var targetAbilities = effectEntity.isMine ? player_Activated_Abilities : opponent_Activated_Abilities;

        Ability removeAbility = targetAbilities.Find(x => x.Tag == effectEntity.card.ability.Tag);

        targetAbilities.Remove(removeAbility);
    }

    void EntityListApplyEffectCancel(Card effectCard, List<SingleEntity> entities, Tribe tribe)
    {
        foreach (var entity in entities)
        {
            if (entity.card.cardType.tribe == tribe)
            {
                foreach (var effect in effectCard.ability.effects)
                {
                    effect.Resolve(entity);
                }
            }
        }
    }
}
