using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// The base ability class.
/// </summary>
public class Ability
{
    public EffectClass effect_Class; // 효과 발동 분류

    public string Tag;

    /// <summary>
    /// The effect of this ability.
    /// </summary>
    public Effect effect;

    /// <summary>
    /// The target of this ability.
    /// </summary>
    public Target target;
    const string BattlePower = "BP";

    public void SetEffectSubject(string effectTargetStr)
    {
        switch(effectTargetStr)
        {
            case "target":
                target = new TargetCard();
                break;
            case "random":
                target = new RandomCard();
                break;
            case "all":
                target = new AllCards();
                break;
            case "self":
                target = new CardSelf();
                break;
            case "player_warrior":
                target = new PlayerWarrior();
                break;
            case "tribe_target":
                target = new TribeTarget();
                break;
            default:
                target = new NoneEffect();
                break;
        }
    }

    public void SetAbility(string effect_Str, string effect_subject_Str)
    {
        SetEffect(effect_Str);
        SetEffectSubject(effect_subject_Str);
    }

    public void SetEffect(string effectStr)
    {
        if (effectStr == null)
            return;

        Tag = effectStr;
        int value, duration;
        if(effectStr.Contains("/"))
        {
            value = Int32.Parse(effectStr.Split('/')[1]);
            duration = Int32.Parse(effectStr.Split('/')[2]);
        }
        else
        {
            value = 0;
            duration = 0;
        }
        switch (effectStr.Split('/')[0])
        {
            case "destroy":
                effect = new DestroyCardEntityEffect();
                break;
            case "increase":
                var increaseCardStatEffect = new IncreaseCardStatEffect();
                increaseCardStatEffect.SetStatID(BattlePower);
                increaseCardStatEffect.value = value;
                increaseCardStatEffect.duration = duration;
                effect = increaseCardStatEffect;
                break;
            case "decrease":
                var decreaseCardStatEffect = new DecreaseCardStatEffect();
                decreaseCardStatEffect.SetStatID(BattlePower);
                decreaseCardStatEffect.value = value;
                decreaseCardStatEffect.duration = duration;
                effect = decreaseCardStatEffect;
                break;
            case "decrese_damage":
                break;
            case "increase_damage":
                break;
        }

        
    }
}

/// <summary>
/// Triggered abilities are abilities that get resolved when their
/// associated trigger takes place.
/// </summary>
public class TriggerAbility : Ability
{
    /// <summary>
    /// The trigger of this ability.
    /// </summary>
    // public Trigger trigger; // 카드 효과 세분화로 추정

    /// <summary>
    /// Constructor.
    /// </summary>
    public TriggerAbility()
    {
        effect_Class = EffectClass.Triggered;
    }
}

/// <summary>
/// Activated abilities are abilities that get resolved when the player
/// pays a cost/s.
/// </summary>
public class ActivateAbility : Ability
{
    /// <summary>
    /// The costs of this ability.
    /// </summary>
    // public List<Cost> costs = new List<Cost>(); // 추후에 마법카드 발동 조건때문에 추가될듯

    /// <summary>
    /// Constructor.
    /// </summary>
    public ActivateAbility()
    {
        effect_Class = EffectClass.Activated;
    }
}

public class BattleAbility : Ability
{
    /// <summary>
    /// The costs of this ability.
    /// </summary>
    // public List<Cost> costs = new List<Cost>(); // 추후에 마법카드 발동 조건때문에 추가될듯

    /// <summary>
    /// Constructor.
    /// </summary>
    public BattleAbility()
    {
        effect_Class = EffectClass.Battle;
    }
}
