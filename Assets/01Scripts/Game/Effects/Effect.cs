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

    // ���� ���¿� ���� ���� �߰��ؾ���. ������ΰ�, �������ΰ� ���
    public virtual bool AreTargetsAvailable(Card sourceCard, Target target)
    {
        return true;
    }
    public virtual void Resolve(Entity entity)
    {
    }
    // �׾����� ȿ�� ��ȿȭ ��Ű���� ����
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
    // ȿ���� ���� �� ī�� �Ǵ� ��� ī�带 �߰��ؼ� �� ȿ���� ����ް� ��������
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