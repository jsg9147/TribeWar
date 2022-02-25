using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkRpcFunc : NetworkBehaviour
{
    public static NetworkRpcFunc instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        
    }

    #region Entity Manager
    [ClientRpc]
    public void RpcCardMove(int entityID, bool targetPlayer, Vector3 movePos, bool server) => EntityManager.instance?.CardMove(entityID, targetPlayer, movePos,server);

    [ClientRpc]
    public void RpcAttack(int attackerID, int defenderID, bool server) => EntityManager.instance?.Attack(attackerID, defenderID, server);

    [ClientRpc]
    public void RpcOutpostAttack(int attackerID, Coordinate outpostCoord, bool server) => EntityManager.instance?.OutpostAttack(attackerID, outpostCoord, server);
    [ClientRpc]
    public void RpcSetCoordinateData(Vector3 coordVec) => EntityManager.instance?.SetCoordinateData(coordVec);

    [ClientRpc]
    public void RpcSummon(bool server, string card_id, Coordinate coordinate) => EntityManager.instance?.Summon(server, card_id, coordinate);

    [ClientRpc]
    public void RpcSelectTribute(bool server, int entityID) => EntityManager.instance?.SelectTribute(server, entityID);

    [ClientRpc]
    public void RpcEffectSolve(string card_id, bool server) => EntityManager.instance?.EffectSolve(card_id, server);

    [ClientRpc]
    public void RpcSelect_Effect_Target(int entityID, bool targetPlayer, bool server) => EntityManager.instance?.Select_Effect_Target(entityID, targetPlayer, server);

    [ClientRpc]
    public void RpcOpponentTargetEffect(int entityID, string card_id, bool server) => EntityManager.instance?.OpponentTargetEffect(entityID, card_id, server);
    [ClientRpc]
    public void RpcPlayerTargetEffect(int entityID, string card_id, bool server) => EntityManager.instance?.PlayerTargetEffect(entityID, card_id, server);
    [ClientRpc]
    public void RpcRandomTargetEffect(bool targetPlayer, int entityIndex, string card_id, bool server) => EntityManager.instance?.RandomTargetEffect(targetPlayer, entityIndex, card_id, server);

    #endregion

    #region Game Manager
    [ClientRpc]
    public void RpcGameResult(bool gameResult, bool server) => GameManager.instance?.GameResult(gameResult, server);

    [ClientRpc]
    public void RpcLoadingComplite()
    {
        GameManager.instance?.LoadingComplited();
    }

    #endregion

    #region Card Manager

    [ClientRpc]
    public void RpcStartCardDealing() => CardManager.instance?.StartCardDealing();
    [ClientRpc]
    public void RpcReloadCard(bool server) => CardManager.instance?.ReloadCard(server);

    [ClientRpc]
    public void RpcTryPutCard(bool server, string card_id, Vector3 selectPos) => CardManager.instance?.TryPutCard(server, card_id, selectPos);

    [ClientRpc]
    public void RpcSetMostFrontOrderInit(bool server) => CardManager.instance?.SetMostFrontOrderInit(server);

    #endregion

    #region Map Manager
    [ClientRpc]
    public void RpcSetOutpost(Coordinate coordinate, bool server) => MapManager.instance?.SetOutpost(coordinate, server);

    #endregion

    #region Turn Manager
    [ClientRpc]
    public void RpcTurnSetup(int randomTurn) => TurnManager.instance?.TurnSetup(randomTurn);

    [ClientRpc]
    public void RpcTurnEnd() => TurnManager.instance?.TurnEnd();

    #endregion
}
