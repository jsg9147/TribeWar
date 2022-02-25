using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum LogCategory
{
    Summon,
    Move,
    Magic,
    Effected,
    Sacrifice,
    Drop,
    Attack,
    Defend,
    Outpost_Attack
}
public enum EffectClass :int
{
    Triggered, 
    Activated,
    Battle,
    None
}

public enum Timing
{
    Magic, Common, Summon, Battle
}

public enum EcardState 
{ 
    Nothing, CanMouseOver, CanMouseDrag 
}

public enum MoveType
{
    Pawn, Rook, Bishop, Knight, Queen, King
}

public enum CardCategory
{
    Monster, Magic, Trap
}

public enum CardRole
{
    melee,
    shooter
}

public enum Tribe
{
    None, Dragon, Magician, Warrior
}

public enum EffectTarget
{
    Player,
    Opponent,
    TargetPlayer,
    RandomPlayer,
    AllPlayers,
    ThisCard,
    PlayerCard,
    OpponentCard,
    TargetCard,
    RandomPlayerCard,
    RandomOpponentCard,
    RandomCard,
    AllPlayerCards,
    AllOpponentCards,
    AllCards,
    PlayerWarrior,
    OpponentWarrior,
    PlayerMagician,
    OpponentMagician,
    PlayerDragon,
    OpponentDragon,
    TribeTarget,
    None
}

public enum TileState
{
    onPlayerMonster,
    onOpponentMonster,
    playerOutpost,
    opponentOutpost,
    empty,
    None
}

public enum CanSpawn
{
    playerCanSpawn,
    opponentCanSpawn,
    all,
    nothing
}