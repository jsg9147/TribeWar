using System.Collections;
using System.Collections.Generic;

public abstract class Effect
{
    /// <summary>
    /// Returns true if there are any targets available for this effect and false otherwise.
    /// </summary>
    /// <param name="state">The game's state.</param>
    /// <param name="sourceCard">The card containing the effect.</param>
    /// <param name="target">The target type of the effect.</param>
    /// <returns>True if there are any targets available for this effect; false otherwise.</returns>
    /// 

    // 게임 상태에 대한 정의 추가해야함. 상대턴인가, 전투중인가 등등
    public virtual bool AreTargetsAvailable(Card sourceCard, Target target)
    {
        return true;
    }
    public virtual void Resolve(Entity entity)
    {
    }
    // 죽었을때 효과 무효화 시키려고 만듬
    public virtual void Reverse(Entity entity)
    {

    }
}

public abstract class CardBaseEffect : Effect
{

    public virtual void Resolve(Card card)
    {
    }

}


/// <summary>
/// The base class for effects that target players.
/// </summary>
public abstract class PlayerBaseEffect : Effect
{        
    /// <summary>
    /// Resolves this effect on the specified player.
    /// </summary>
    /// <param name="state">The game's state.</param>
    /// <param name="player">The player on which to resolve this effect.</param>
    public virtual void Resolve(UserInfo player)
    {

    }

    public override bool AreTargetsAvailable(Card sourceCard, Target target)
    {
        switch(target.GetTarget())
        {
            case EffectTarget.Player:
                break;

            case EffectTarget.Opponent:
                break;

            case EffectTarget.TargetPlayer:
                break;

            case EffectTarget.RandomPlayer:
                break;

            case EffectTarget.AllPlayers:
                break;

            default:
                break;
        }
        return base.AreTargetsAvailable(sourceCard, target);
    }
}


/// <summary>
/// The base class for card effects.
/// </summary>
public abstract class CardEffect : CardBaseEffect
{
    // 효과에 따라 내 카드 또는 상대 카드를 추가해서 그 효과를 적용받게 만들어야함
    public override bool AreTargetsAvailable(Card sourceCard, Target target)
    {
        switch (target.GetTarget())
        {
            case EffectTarget.ThisCard:
                break;

            case EffectTarget.PlayerCard:
                break;

            case EffectTarget.OpponentCard:
                break;

            case EffectTarget.TargetCard:
                break;

            case EffectTarget.RandomPlayerCard:
                break;

            case EffectTarget.RandomCard:
                break;

            case EffectTarget.AllPlayerCards:
                break;

            case EffectTarget.AllOpponentCards:
                break;

            case EffectTarget.AllCards:
                break;

            default:
                break;
        }
        return base.AreTargetsAvailable(sourceCard, target);
    }
}

public abstract class CardStatEffect : CardEffect
{
    /// <summary>
    /// The unique identifier of the stat.
    /// 0 = attack, 1 = defense.
    /// </summary>
    public int statID;

    public void SetStatID(string statName)
    {
        if (statName == "attack")
            statID = 0;
        else if (statName == "defense")
            statID = 1;
    }
}

public abstract class CardDestroyEffect : CardEffect
{
    /// <summary>
    /// The Card destroy
    /// </summary>
    public int cardIndex;
}