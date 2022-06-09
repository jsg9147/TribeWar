using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class EntityManager : MonoBehaviour
{
    public static EntityManager instance
    {
        get; set;
    }

    private void Awake()
    {
        instance = this;
    }

    public GraveSet graveManager;
    public AIManager aiManager;
    public EffectManager effectManager;

    [SerializeField] CameraEffect cameraEffect;
    [SerializeField] GameObject entityPrefab;
    [SerializeField] Transform CardCreatePos;
    [SerializeField] Transform playerGraveyard;
    [SerializeField] Transform oppenentGraveyard;
    [SerializeField] GameObject TargetPicker;

    [SerializeField][Header("플레이어 엔티티 부모")] Transform playerEntityParent;
    [SerializeField][Header("적군 엔티티 부모")] Transform opponentEntityParent;
    [SerializeField][Header("중립 엔티티 부모")] Transform neutralityEntityParent;

    [SerializeField] int maxMoveCount = 2;
    [SerializeField] int maxSummonCount = 2;

    public TMP_Text canMoveCountTMP;
    public TMP_Text canSummonCountTMP;

    public List<Entity> playerEntities;
    public List<Entity> opponentEntities;
    public List<Entity> aiEntities;

    public Tile selectTile;
    Coordinate entityCoordinateData;

    // 카드 로그
    public PlayLogControl gameLog;

    // 제물 소환을 위한 변수, CardManager 에서 사용해서 public을 못지웠음
    public bool SelectMonsterMode = false;

    // 마법카드 발동을 위한 변수
    //public EffectsSolver effectSolver = new EffectsSolver();

    CardMove cardMove = new CardMove();

    //bool CanMouseInput => TurnManager.instance.isLoading == false && onOtherPanel == false; 아마도 필요 없음
    //bool onOtherPanel = false; 220603
    public bool clickBlock;

    Entity selectEntity = null;
    public bool selectState => selectEntity != null;

    Entity enermyEntityPick;

    public List<Entity> tributeEntities = new List<Entity>();

    int summonCount, canMoveCount, entityIDCount, cardCost;

    // cardCost, card_id , effectCard 는 없앨수 있을꺼 같음

    // 왜 따로??
    string card_id; // 제물 및 타 효과시 임시 카드 저장 정보
    Card effectCard; // 카드 효과 발동시 정보를 임시 저장하는 변수

    //bool effect_Target_Select_State = false; //jsg
    //bool moveEffect; //jsg 220603 effectManager 에서 쓰게 작동중

    //bool tributeSummon, tributeMagic, targetEffect; // 늘 그렇듯 별견시 버그 없으면 삭제 220526 그냥 지우지 말고 찾아서 지워야함

    public Dictionary<Tribe, int> tribeSummonCount;

    [Header("이펙트 모음")]
    [SerializeField] GameObject death_VFX;
    [SerializeField] GameObject explosion_VFX;
    [SerializeField] GameObject magic_VFX;
    [SerializeField] GameObject arrow_VFX;
    public GameObject portal_VFX;

    [SerializeField][Header("원거리 공격 이펙트")] GameObject bullet_Obj;

    private void Start()
    {
        Init();

    }
    private void Update()
    {
        //if (enermyEntityPick != null || (selectTile != null))
        //{
        //    ShowTargetPicker(true);
        //}
        //else
        //{
        //    ShowTargetPicker(false);
        //}
    }
    private void OnDestroy()
    {
        TurnManager.OnTurnStarted -= OnTurnStarted;
    }

    void Init()
    {
        canMoveCount = maxMoveCount;
        summonCount = maxSummonCount;
        canMoveCountTMP.text = "0";
        canSummonCountTMP.text = "0";
        TurnManager.OnTurnStarted += OnTurnStarted;
        opponentEntities = new List<Entity>();
        aiEntities = new List<Entity>();
        tribeSummonCount = new Dictionary<Tribe, int>();

        entityIDCount = 0;
        cardCost = 0;

        clickBlock = false;
        //moveEffect = false;
    }

    void CountTMP_Update()
    {
        canMoveCountTMP.text = canMoveCount.ToString();
        canSummonCountTMP.text = summonCount.ToString();
    }

    public void MouseEventInit()
    {
        ChangeColor_CanMovePos(false, selectEntity);
        selectEntity = null;
        enermyEntityPick = null;
    }

    public List<Entity> All_Entities
    {
        get
        {
            List<Entity> allEntities = new List<Entity>(playerEntities);
            allEntities.AddRange(opponentEntities);
            allEntities.AddRange(aiEntities);

            return allEntities;
        }
        
    }

    #region Entity 마우스 관련
    public void EntityMouseDown(Entity entity)
    {
        if (clickBlock)
            return;

        selectEntity = entity;
        //if (CanMouseInput == false)
        //{
        //    selectEntity = null;
        //    return;
        //}

        if (SelectMonsterMode)
            return;

        if (entity.isMine && entity.attackable)
        {
            if (canMoveCount >= 0)
            {
                ChangeColor_CanMovePos(true, entity);
            }
        }
        else
        {
            ChangeColor_CanMovePos(true, entity, false);
        }

    }

    public void EntityMouseUP(Entity entity)
    {
        effectManager.Select_Effect_Target(entity);

        //if (effect_Target_Select_State)
        //{
        //    GameManager.instance.localGamePlayerScript.CmdSelectEffectTarget(entity.id, entity.isMine, NetworkRpcFunc.instance.isServer);
        //    return;
        //}

        if (entity.isMine && SelectMonsterMode)
        {
            if (entity.canTribute && entity.isDie == false)
            {
                GameManager.instance.localGamePlayerScript.CmdSelectTribute(entity.id, NetworkRpcFunc.instance.isServer);
                return;
            }
        }
        else
        {
            ChangeColor_CanMovePos(false, entity);
        }

        if (clickBlock)
            return;

        Entity_Active();
        selectEntity = null;
        enermyEntityPick = null;
    }

    public void EntityMouseOver(Entity entity)
    {
        if (selectEntity == null)
            return;
        if (clickBlock)
            return;
        
        if (entity.isMine == false)
        {
            enermyEntityPick = entity;
        }
    }

    public void EntityMouseDrag()
    {
        //if (!CanMouseInput || selectEntity == null)
        //    return;
        //if (SelectMonsterMode)
        //    return;
        //if (clickBlock)
        //    return;
    }
    #endregion

    void OnTurnStarted(bool myTurn)
    {
        if (selectEntity != null)
            ChangeColor_CanMovePos(false, selectEntity);

        selectEntity = null;
        AttackableReset(myTurn);
        canMoveCount = maxMoveCount;
        summonCount = maxSummonCount;
        SelectMonsterMode = false;
        //effect_Target_Select_State = false;

        clickBlock = false;

        //tributeSummon = false;
        //tributeMagic = false;
        //moveEffect = false;

        effectManager.effect_Activated = false;

        if (TurnManager.instance.firstTurn)
            summonCount = maxSummonCount - 1;

        if (NetworkRpcFunc.instance.isServer)
        {
            aiManager.Entity_Active(All_Entities);
        }

        CountTMP_Update();
    }

    // 카드 소환시 타일 좌표로 정렬시켜주는 함수
    void EntityAlignment(bool isMine, Vector3 spawnPos)
    {
        var targetEntities = isMine ? playerEntities : opponentEntities;
        var targetEntity = targetEntities[targetEntities.Count - 1];
        targetEntity.transformPos = spawnPos;
        targetEntity.GetComponent<Order>()?.SetOriginOrder(10);
    }

    void ChangeColor_CanMovePos(bool changeColor, Entity entity, bool CanMove = true)
    {
        if (entity == null)
            return;

        foreach (var pos in cardMove.Can_Attack_Position(entity))
        {
            int x = (int)pos.x;
            int y = (int)pos.y;

            if (CanMove)
            {
                MapManager.instance.mapData[x, y].ColorChange_Rock(changeColor, Color.yellow);
            }
            else
            {
                MapManager.instance.mapData[x, y].ColorChange_Rock(changeColor, Color.black);
            }
        }

        foreach (var pos in cardMove.FindCanMovePositionList(entity))
        {
            int x = (int)pos.x;
            int y = (int)pos.y;

            if (CanMove)
            {
                MapManager.instance.mapData[x, y].ColorChange_Rock(changeColor, Color.green);
            }
            else
            {
                MapManager.instance.mapData[x, y].ColorChange_Rock(changeColor, Color.red);
            }
        }
    }

    void Entity_Active()
    {
        if (selectEntity == null || selectTile == null)
            return;
        if (canMoveCount <= 0)
            return;
        if (selectEntity.attackable == false)
            return;
        
        if (enermyEntityPick)
        {
            if (cardMove.Can_Attack_Position(selectEntity).Exists(x => x.vector3Pos == enermyEntityPick.coordinate.vector3Pos))
                GameManager.instance.localGamePlayerScript.CmdAttack(selectEntity.id, enermyEntityPick.id, NetworkRpcFunc.instance.isServer);
        }

        if (selectTile.tileState == TileState.enermyOutpost)
        {
            if (cardMove.Can_Attack_Position(selectEntity).Exists(x => x.vector3Pos == selectTile.coordinate.vector3Pos))
            {
                GameManager.instance.localGamePlayerScript.CmdOutpostAttack(
                    selectEntity.id, selectTile.outpost.coordinate.vector3Pos, NetworkRpcFunc.instance.isServer);
            }
        }
        else
        {
            GameManager.instance.localGamePlayerScript.CmdCardMove(
                selectEntity.id, selectEntity.isMine, selectTile.coordinate.vector3Pos, NetworkRpcFunc.instance.isServer);

            if (GameManager.instance.localGamePlayerScript.canMove)
            {
                canMoveCount--;
            }

            if (canMoveCount == 0)
                GameManager.instance.localGamePlayerScript.canMove = false;
        }

        CountTMP_Update();
        //220603
        //if (selectEntity != null && selectTile != null && selectEntity.attackable && canMoveCount >= 0)
        //{
        //    if (targetPickEntity)
        //    {
        //        if (cardMove.Can_Attack_Position(selectEntity).Exists(x => x.vector3Pos == targetPickEntity.coordinate.vector3Pos))
        //            GameManager.instance.localGamePlayerScript.CmdAttack(selectEntity.id, targetPickEntity.id, NetworkRpcFunc.instance.isServer);
        //    }

        //    if (selectTile.tileState == TileState.opponentOutpost)
        //    {
        //        if (cardMove.Can_Attack_Position(selectEntity).Exists(x => x.vector3Pos == selectTile.coordinate.vector3Pos))
        //        {
        //            GameManager.instance.localGamePlayerScript.CmdOutpostAttack(
        //                selectEntity.id, selectTile.outpost.coordinate.vector3Pos, NetworkRpcFunc.instance.isServer);
        //        }
        //    }
        //    else
        //    {
        //        GameManager.instance.localGamePlayerScript.CmdCardMove(
        //            entity.id, entity.isMine, selectTile.coordinate.vector3Pos, NetworkRpcFunc.instance.isServer);

        //        if (GameManager.instance.localGamePlayerScript.canMove)
        //        {
        //            canMoveCount--;
        //        }

        //        if (canMoveCount == 0)
        //            GameManager.instance.localGamePlayerScript.canMove = false;
        //    }
        //}
    }

    public void CardMove(int entityID, bool targetPlayer, Vector3 movePos, bool server)
    {
        bool isMine = NetworkRpcFunc.instance.isServer == server;
        var targetEntity = All_Entities.Find(x => x.id == entityID);
        
        Coordinate moveCoord = new Coordinate(movePos);

        if (isMine == false)
        {
            moveCoord.SetReverse(MapManager.instance.mapSize);
        }

        cardMove.Move(targetEntity, MapManager.instance.coordinateTile(moveCoord));
    }

    #region 소환 관련
    void SummonBase(bool isMine, Card card, Tile summonTile)
    {
        Vector3 spawnPos = CardCreatePos.position;
        Transform entityParent = isMine ? playerEntityParent : opponentEntityParent;
        string sumnmonPlayer = isMine ? "Player : " : "Opponent : ";

        if (isMine == false)
            spawnPos.y = -spawnPos.y;

        var entityObject = Instantiate(entityPrefab, spawnPos, Utils.QI, entityParent);
        entityObject.name = sumnmonPlayer + card.name + entityIDCount;

        var entity = entityObject.GetComponent<Entity>();
        entity.Setup(card, isMine);

        if (isMine)
        {
            playerEntities.Add(entity);
            int index = playerEntities.IndexOf(entity);
            playerEntities[index] = entity;
            entity.id = entityIDCount;
            entity.coordinate = summonTile.coordinate;
            if (SelectMonsterMode)
            {
                entity.coordinate = entityCoordinateData;
                Tile targetTile = MapManager.instance.coordinateTile(entityCoordinateData);
                //targetTile.SetMonster(entity);
                targetTile.onEntity = entity;
                EntityAlignment(isMine, targetTile.transformPos);
            }
            else
            {
                //summonTile.SetMonster(entity);
                summonTile.onEntity = entity;
                EntityAlignment(isMine, summonTile.transformPos);
            }
        }
        else
        {
            opponentEntities.Add(entity);
            entity.id = entityIDCount;
            entity.coordinate = summonTile.coordinate;

            //summonTile.SetMonster(entity);
            summonTile.onEntity = entity;
            EntityAlignment(isMine, summonTile.transformPos);
        }

        if (tribeSummonCount.ContainsKey(card.cardType.tribe))
        {
            if (tribeSummonCount[card.cardType.tribe] > 0)
                tribeSummonCount[card.cardType.tribe] = tribeSummonCount[card.cardType.tribe] - 1;
            else
                summonCount--;
        }
        else
        {
            summonCount--;
        }
        entityIDCount++;

        CountTMP_Update();

        // gameLog.Log_Sorter(LogCategory.Summon, entity);

        Destroy(gameObject.GetComponent<Entity>());

        effectManager.Add_Activated_Effect_To_Entity(entity);
        //effectSolver.Add_Activated_Effect_To_Entity(entity);

        if (card.ability.effect_Time != EffectTime.Battle)
        {
            effectManager.EffectTrigger(isMine, card.id);
        }
    }

    // 카드 
    public bool canUseCard(bool isMine, Card card)
    {
        int tributeCount = playerEntities.Count;
        if (card.cardType.card_category == CardCategory.Monster)
        {
            if (summonCount > 0)
            {
                foreach (var fieldCard in playerEntities)
                {
                    if (fieldCard.card.ability.effect_Time == EffectTime.Tribute)
                    {
                        if (fieldCard.card.ability.targetID == card.id)
                            return true;

                        foreach (var effect in fieldCard.card.ability.effects)
                        {
                            if (effect.effectClass == EffectClass.tribute)
                            {
                                tributeCount = tributeCount + effect.value;
                            }
                        
                        }
                    }
                }

                if (card.cost > 0 && tributeCount < card.cost)
                {
                    return false;
                }
                return true;
            }

            if (tribeSummonCount.ContainsKey(card.cardType.tribe))
            {
                if (tribeSummonCount[card.cardType.tribe] > 0)
                {
                    if (card.cost > 0 && playerEntities.Count < card.cost)
                    {
                        return false;
                    }
                    return true;
                }
            }
        }
        else if (card.cardType.card_category == CardCategory.Magic)
        {
            if (card.cost > 0 && playerEntities.Count < card.cost)
            {
                return false;
            }
            return true;
        }

        // 조건 다 안걸렸다는 뜻
        return false;
    }

    bool canSummon(bool isMine, Card card, Tile tile)
    {
        if (tile == null || tile.tileState != TileState.empty)
            return false;
        if (tile.canSpawn == CanSpawn.nothing)
            return false;
        if (tile.canSpawn == CanSpawn.playerCanSpawn && isMine == false)
            return false;
        if (tile.canSpawn == CanSpawn.opponentCanSpawn && isMine)
            return false;

        if (tribeSummonCount.ContainsKey(card.cardType.tribe))
        {
            if (tribeSummonCount[card.cardType.tribe] > 0)
                return true;
        }

        if (summonCount <= 0)
            return false;

        return true;
    }


    public void Summon(bool server, string card_id, Coordinate coordinate)
    {
        // bool isMine = server == NetworkRpcFunc.instance.isServer; 테스트중
        bool isMine = server;

        Card card = CardDatabase.instance.CardData(card_id);

        if (isMine == false)
            coordinate.SetReverse(MapManager.instance.mapSize);

        Tile targetTile = MapManager.instance.coordinateTile(coordinate);

        if (canSummon(isMine, card, targetTile) == false)
            return;

        this.card_id = card_id;

        cardCost = card.cost;
        if (card.cost == 0)
        {
            SummonBase(isMine, card, targetTile);
            CardManager.instance.RemoveTargetCards(isMine);
            return;
        }
        else
        {
            if (isMine && playerEntities.Count >= card.cost)
            {
                entityCoordinateData = new Coordinate(targetTile.coordinate.vector3Pos);
                Select_Monster(true);
            }
            else if (isMine && playerEntities.Count <= card.cost)
            {
                foreach (var fieldCard in playerEntities)
                {
                    if (fieldCard.card.ability.effect_Time == EffectTime.Tribute)
                    {
                        if (fieldCard.card.ability.targetID == card.id)
                        {
                            entityCoordinateData = new Coordinate(targetTile.coordinate.vector3Pos);
                            Select_Monster(true);
                        }
                    }
                }
            }
        }
    }


    public void Select_Monster(bool isSummon)
    {
        GameManager.instance.Notification("몬스터를\n선택해주세요");
        CardManager.instance.TributeSummonSet(false); // 제물소환시 소환할 카드 잠깐 안보이게함
        if (isSummon)
        {
            foreach (var fieldEntity in playerEntities)
            {
                if (fieldEntity.card.ability.targetID == card_id)
                {
                    fieldEntity.ClickMark(true);
                }
            }

            SelectMonsterMode = true;
        }
        else
        {
            effectManager.effect_Activated = true;
        }
    }

    void TributeSummon(bool isMine, string card_id)
    {
        if (!isMine)
            CardManager.instance.Remove_OhterPlayer_HandCard();

        Card card = CardDatabase.instance.CardData(card_id);

        CardManager.instance.RemoveTargetCards(isMine);


        foreach (var entity in tributeEntities)
        {
            // gameLog.Log_Sorter(LogCategory.Sacrifice, entity);
            entity.isDie = true;

            UpdateEntityState(); // 죽는것과 제물을 구분해야하는가? 이펙트 생각하면 하는게 좋긴할듯 jsg
        }

        if (isMine == false)
            entityCoordinateData.SetReverse(MapManager.instance.mapSize);

        Tile targetTile = MapManager.instance.coordinateTile(entityCoordinateData);
        SummonBase(isMine, card, targetTile);
        tributeEntities.Clear();

        cardCost = 0;
        SelectMonsterMode = false;
    }

    void TributeMagic(bool isMine, string card_id)
    {
        Card card = CardDatabase.instance.CardData(card_id);

        if (!isMine)
        {
            CardManager.instance.Remove_OhterPlayer_HandCard();
        }

        CardManager.instance.RemoveTargetCards(isMine);

        foreach (var entity in tributeEntities)
        {
            // gameLog.Log_Sorter(LogCategory.Sacrifice, entity);
            entity.isDie = true;

            UpdateEntityState(); // 죽는것과 제물을 구분해야하는가? 이펙트 생각하면 하는게 좋긴할듯 jsg
        }
        GameManager.instance.localGamePlayerScript.CmdEffectSolve(card.id, NetworkRpcFunc.instance.isServer);
        tributeEntities.Clear();

        cardCost = 0;
        SelectMonsterMode = false;
    }

    public void SelectMonster(bool server, int entityID)
    {
        bool isMine = NetworkRpcFunc.instance.isServer == server;
        Entity entity = isMine ? playerEntities.Find(x => x.id == entityID) : opponentEntities.Find(x => x.id == entityID);

        tributeEntities.Add(entity);

        entity.ClickMark(false);
        entity.canTribute = false;

        int tribute_Count = tributeEntities.Count;

        if (cardCost <= tributeEntities.Count)
        {
            if (CardDatabase.instance.CardData(card_id).cardType.card_category == CardCategory.Monster)
                TributeSummon(isMine, card_id);

            else if (CardDatabase.instance.CardData(card_id).cardType.card_category == CardCategory.Magic)
                TributeMagic(isMine, card_id);
        }
        else
        {
            foreach (var tribute in tributeEntities)
            {
                if (tribute.card.ability.effect_Time == EffectTime.Tribute)
                {
                    if (tribute.card.ability.targetID == card_id)
                    {
                        TributeSummon(isMine, card_id);
                        return;
                    }
                    else
                    {
                        foreach (var effec in tribute.card.ability.effects)
                        {
                            print(effec.effectClass);
                        }

                        if (tribute.card.ability.effects.Exists(x => x.effectClass == EffectClass.tribute))
                        {
                            Effect effect = tribute.card.ability.effects.Find(x => x.effectClass == EffectClass.tribute);
                            tribute_Count = tribute_Count + effect.value;
                        }
                    }
                }
            }
            if (cardCost <= tribute_Count)
                TributeSummon(isMine, this.card_id);
        }
    }
    #endregion


    #region Magic Card
    //IEnumerator DelaySelectTarget()
    //{
    //    yield return new WaitForSeconds(0.7f);
    //    effect_Target_Select_State = true;
    //}

    public void ConfirmEffectTrigger(string msg = "타겟을\n 선택 하세요")
    {
        GameManager.instance.Notification(msg);
        //StartCoroutine(DelaySelectTarget());
    }

    //public bool EffectTrigger(bool isMine, string card_id)
    //{
    //    if (selectTile == null && isMine)
    //        return false;

    //    effectCard = CardDatabase.instance.CardData(card_id);

    //    if (effectSolver.EffectRequireExist(isMine, playerEntities, opponentEntities, effectCard) == false)
    //        return false;

    //    this.card_id = card_id;
    //    cardCost = effectCard.cost;

    //    if (effectCard.ability.effect_Time == EffectTime.Activated)
    //    {
    //        if (isMine)
    //        {
    //            effectSolver.player_Activated_Abilities.Add(effectCard.ability);
    //        }
    //        else
    //        {
    //            effectSolver.opponent_Activated_Abilities.Add(effectCard.ability);
    //        }
    //        effectSolver.Activated_Effect(effectCard.ability, isMine, playerEntities, opponentEntities);
    //        return true;
    //    }
    //    else if (effectCard.ability.effect_Time == EffectTime.Triggered)
    //    {
    //        if (isMine) 
    //        {
    //            GameManager.instance.localGamePlayerScript.CmdEffectSolve(effectCard.id, NetworkRpcFunc.instance.isServer);
    //        }
    //        return true;
    //    }

    //    if (effectCard.cost == 0 || effectCard.cardType.card_category == CardCategory.Monster)
    //    {
    //        if (isMine)
    //        {
    //            GameManager.instance.localGamePlayerScript.CmdEffectSolve(effectCard.id, NetworkRpcFunc.instance.isServer);
    //        }
    //        return true;
    //    }
    //    else if (effectCard.cost > 0)
    //    {
    //        if (effectCard.cost <= tributeEntities.Count)
    //        {
    //            GameManager.instance.localGamePlayerScript.CmdEffectSolve(effectCard.id, NetworkRpcFunc.instance.isServer);
    //            return true;
    //        }

    //        if (effectCard.cost <= playerEntities.Count && isMine)
    //        {
    //            Select_Monster(false);
    //            //GameManager.instance.Confirmation("제물이 " + effectCard.cost + "마리 필요합니다.\n" + "사용\n하시겠습니까?");
    //            // gameLog.Log_Sorter(LogCategory.Magic, effectCard, isMine);
    //            //onOtherPanel = true;
    //            //tributeMagic = true;
    //        }
    //    }
    //    return false;
    //}

    //public void EffectSolve(string card_id, bool server)
    //{
    //    bool isMine = NetworkRpcFunc.instance.isServer == server;
    //    Card effectCard = CardDatabase.instance.CardData(card_id);
    //    if (effectCard.ability == null)
    //        return;

    //    switch (effectCard.ability.target.GetTarget())
    //    {
    //        case EffectTarget.TargetCard:
    //            if (isMine)
    //            {
    //                if (effectCard.cardType.card_category == CardCategory.Monster)
    //                {
    //                    Select_Monster(false);
    //                }
    //                else
    //                {

    //                    //if (effectCard.ability.Tag.Contains("move"))
    //                    //{
    //                    //    moveEffect = true;
    //                    //}
    //                    ConfirmEffectTrigger();
    //                }
    //            }
    //            break;

    //        case EffectTarget.RandomCard:
    //            if (playerEntities.Count != 0 || opponentEntities.Count != 0)
    //            {
    //                if (isMine)
    //                {
    //                    effectSolver.NonTargetEffectActive(effectCard, playerEntities, opponentEntities, server);
    //                    UpdateEntityState();
    //                }
    //            }
    //            break;
    //        case EffectTarget.AllCards:
    //            effectSolver.NonTargetEffectActive(effectCard, playerEntities, opponentEntities, server);
    //            UpdateEntityState();
    //            break;
    //        case EffectTarget.TribeTarget:
    //            if (isMine)
    //            {
    //                ConfirmEffectTrigger(effectCard.TribeStr() + " 타겟을\n선택해주세요");
    //            }
    //            break;
    //        case EffectTarget.PlayerWarrior:
    //            effectSolver.NonTargetEffectActive(effectCard, playerEntities, opponentEntities, server);
    //            UpdateEntityState();
    //            break;
    //        case EffectTarget.Player:
    //            effectSolver.PlayerTargetEffect(this, effectCard, isMine);
    //            break;
    //        default:
    //            return;
    //    }
    //    // gameLog.Log_Sorter(LogCategory.Magic, effectCard, isMine); // 로그 추가
    //}

    //jsg
    //public void Select_Effect_Target(int entityID, bool targetPlayer, bool server)
    //{
    //    bool isMine = server == NetworkRpcFunc.instance.isServer;

    //    Entity targetEntity = All_Entities.Find(x => x.id == entityID);

    //    if (targetEntity == null)
    //        return;

    //    EffectTarget effectTarget = effectCard.ability.target.GetTarget();

    //    switch (effectTarget)
    //    {
    //        case EffectTarget.PlayerCard:
    //            if (targetEntity.belong == EntityBelong.Player)
    //            {
    //                effect_Target_Select_State = false;
    //            }
    //            break;

    //        case EffectTarget.EnermyCard:
    //            if (targetEntity.belong == EntityBelong.Enermy)
    //            {
    //                effect_Target_Select_State = false;
    //            }
    //            break;

    //        case EffectTarget.TargetCard:
    //            {
    //                effect_Target_Select_State = false;
    //            }
    //            break;

    //        case EffectTarget.TribeTarget:
    //            if (targetEntity.card.cardType.tribe == effectCard.cardType.tribe)
    //                effect_Target_Select_State = false;
    //            else
    //                GameManager.instance.Notification(effectCard.TribeStr() + " 타겟을\n추가 선택 해주세요");
    //            break;

    //        case EffectTarget.Tile:
    //            break;

    //        default:
    //            break;
    //    }

    //    if (moveEffect)
    //    {
    //        clickBlock = true;

    //        MapManager.instance.SelectMode(targetEntity, effectCard.ability);
    //    }
    //    else if (moveEffect && isMine == false)
    //    {

    //    }
    //    else
    //    {
    //        if (effect_Target_Select_State == false)
    //        {
    //            // gameLog.Log_Sorter(LogCategory.Effected, targetEntity);

    //            foreach (var effect in effectCard.ability.effects)
    //            {
    //                effect.Resolve(targetEntity);
    //            }
    //            UpdateEntityState();
    //        }
    //    }
    //}

    public void Target_Effect_Solver(int entity_Id, Vector3 tilePos)
    {
        List<Entity> allEntities = new List<Entity>(playerEntities);
        allEntities.AddRange(opponentEntities);

        Entity entity = allEntities.Find(x => x.id == entity_Id);
        Coordinate movePos = new Coordinate(tilePos);

        Tile targetTile;

        if (entity.isMine)
            targetTile = MapManager.instance.mapData[movePos.x, movePos.y];
        else
        {
            movePos.SetReverse(MapManager.instance.mapSize);
            targetTile = MapManager.instance.mapData[movePos.x, movePos.y];
        }

        foreach (var effect in effectCard.ability.effects)
        {
            effect.Resolve(this, entity, targetTile);
        }
        UpdateEntityState();
        effectCard = null;

        MapManager.instance.MapTileInit();
    }

    public GameObject VFX_Instatiate(Vector3 pos)
    {
        GameObject portal = Instantiate(portal_VFX, pos, transform.rotation);
        portal.transform.localScale = new Vector3(5, 5, 5);

        return portal;
    }
    public void Destroy_VFX(GameObject vfx_effect)
    {
        Destroy(vfx_effect);
    }


    public void RandomTargetEffect(int entity_Id, string card_id)
    {
        Entity target_Entity = All_Entities.Find(x => x.id == entity_Id);
        // gameLog.Log_Sorter(LogCategory.Effected, targetEntity);
        //effectSolver.ReceiveRandomEffect(target_Entity, card_id); // jsg
        effectManager.ReceiveRandomEffect(target_Entity, card_id); // jsg

        UpdateEntityState();
    }

    // 몬스터 사망시 처리되는 함수
    public void UpdateEntityState()
    {
        List<Entity> destroiedEntity = new List<Entity>();
        foreach (var playerEntity in playerEntities)
        {
            if (playerEntity.isDie)
            {
                if (playerEntity.card.ability.effect_Time == EffectTime.Activated)
                {
                    effectManager.ReverseEffect(playerEntity, All_Entities);
                    //effectSolver.ReverseEffect(playerEntity, playerEntities, opponentEntities);
                }
                MapManager.instance.mapData[playerEntity.coordinate.x, playerEntity.coordinate.y].tileState = TileState.empty;
                MapManager.instance.mapData[playerEntity.coordinate.x, playerEntity.coordinate.y].onEntity = null;
                GoToGraveyard(playerEntity);
                destroiedEntity.Add(playerEntity);
            }
        }

        foreach (var opponentEntity in opponentEntities)
        {
            if (opponentEntity.isDie)
            {
                if (opponentEntity.card.ability.effect_Time == EffectTime.Activated)
                {
                    //effectSolver.ReverseEffect(opponentEntity, playerEntities, opponentEntities);
                    effectManager.ReverseEffect(opponentEntity, All_Entities);
                }
                MapManager.instance.mapData[opponentEntity.coordinate.x, opponentEntity.coordinate.y].tileState = TileState.empty;
                GoToGraveyard(opponentEntity);
                destroiedEntity.Add(opponentEntity);
            }
        }

        foreach (var destroyEntity in destroiedEntity)
        {
            if (playerEntities.Contains(destroyEntity))
                playerEntities.Remove(destroyEntity);
            else if (opponentEntities.Contains(destroyEntity))
                opponentEntities.Remove(destroyEntity);

        }
    }

    void GoToGraveyard(Entity entity)
    {
        Vector3 gravePosition = entity.isMine ? playerGraveyard.position : oppenentGraveyard.position;

        graveManager.AddGraveCard(entity.card, entity.isMine);

        Sequence sequence = DOTween.Sequence()
                .Append(entity.transform.DOShakePosition(1.3f))
                .Append(entity.transform.DOMove(gravePosition, 0.5f))
                .Append(entity.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.OutCirc))
                .OnComplete(() =>
                {
                    Destroy(gameObject.GetComponent<Entity>());
                });
    }

    #endregion

    #region Battle

    public void Attack(int attackerID, int defenderID, bool server)
    {
        bool isMine = NetworkRpcFunc.instance.isServer == server;

        List<Entity> allEntities = new List<Entity>(playerEntities);
        allEntities.AddRange(opponentEntities);

        Entity attacker, defender;
        Sequence sequence;

        if (isMine)
        {
            attacker = allEntities.Find(x => x.id == attackerID);
            defender = allEntities.Find(x => x.id == defenderID);
        }
        else
        {
            attacker = allEntities.Find(x => x.id == attackerID);
            defender = allEntities.Find(x => x.id == defenderID);
        }

        int attackerBP = attacker.GetEffectiveValue("bp");
        int defenderBP = defender.GetEffectiveValue("bp");

        if (defender.card.cardType.attack_type == AttackType.shooter)
            attackerBP = attackerBP * 2;

        attacker.GetComponent<Order>().SetMostFrontOrder(true);

        Vector3 attackPosition = defender.transformPos;

        ResolverEffect(attacker, defender);

        if (attacker.card.cardType.attack_type == AttackType.shooter)
        {
            GameObject bullet = Instantiate(bullet_Obj, attacker.transformPos, transform.rotation) as GameObject;

            sequence = DOTween.Sequence()
                    .Append(bullet.transform.DOMove(attackPosition, 0.4f)).SetEase(Ease.InSine)
                    .AppendCallback(() =>
                    {
                        GameObject spawnedVFX = Instantiate(explosion_VFX, defender.transform.position, transform.rotation) as GameObject;
                        Destroy(spawnedVFX, 5f);
                    }).OnComplete(() => Destroy(bullet, 1f));

            defender.Damaged(attackerBP);
        }
        else
        {
            sequence = DOTween.Sequence()
                    .Append(attacker.transform.DOMove(attackPosition, 0.4f)).SetEase(Ease.InSine)
                    .AppendCallback(() =>
                    {
                        GameObject spawnedVFX = Instantiate(explosion_VFX, attackPosition, transform.rotation) as GameObject;
                        attacker.Damaged(defenderBP);
                        defender.Damaged(attackerBP);
                        Destroy(spawnedVFX, 5f);
                    }).OnComplete(() => AttackCallback(attacker, defender));
        }

        attacker.attackable = false;

        canMoveCount--;

        UpdateEntityState();


        // PlayLogControl.instance.Log_Sorter(LogCategory.Attack, attacker, attackerBP, attacker.GetEffectiveValue("bp"));
        // PlayLogControl.instance.Log_Sorter(LogCategory.Defend, defender, defenderBP, defender.GetEffectiveValue("bp"));
    }

    public void OutpostAttack(int attackerID, Coordinate outpostCoord, bool server)
    {
        bool isMine = NetworkRpcFunc.instance.isServer == server;

        if (isMine == false)
            outpostCoord.SetReverse(MapManager.instance.mapSize);

        Entity attacker;
        Outpost outpost = MapManager.instance.coordinateTile(outpostCoord).outpost;

        attacker = All_Entities.Find(x => x.id == attackerID);
        attacker.GetComponent<Order>().SetMostFrontOrder(true);

        Vector3 attackPosition = outpost.transformPos;

        Sequence sequence = DOTween.Sequence()
        .Append(attacker.transform.DOMove(attackPosition, 0.4f)).SetEase(Ease.InSine)
        .AppendCallback(() =>
        {
            outpost.Damaged(attacker.GetEffectiveValue("bp"));
            //PlayLogControl.instance.Log_Sorter(LogCategory.Outpost_Attack, attacker, outpost);
        }).OnComplete(() => OutpostAttackCallback(attacker, outpost));
    }

    void AttackCallback(Entity attacker, Entity defender)
    {
        attacker.GetComponent<Order>().SetMostFrontOrder(false);

        if (defender.isDie)
        {
            defender.bottomTile.tileState = TileState.empty;

            if (attacker.isDie == false)
            {
                Sequence sequence = DOTween.Sequence()
                .Append(defender.transform.DOShakePosition(0.5f))
                .Append(defender.transform.DOScale(Vector3.one, 0.3f)).SetEase(Ease.OutCirc)
                .Append(attacker.transform.DOMove(defender.transformPos, 0.1f).SetEase(Ease.InSine))
                .AppendCallback(() =>
                {
                    if (attacker.card.cardType.attack_type != AttackType.shooter)
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
                    if (attacker.card.cardType.attack_type != AttackType.shooter)
                        cardMove.AfterAttackMove(attacker, defender.coordinate); // 공격후 디펜더 바로앞으로 이동시키는것
                    else
                        attacker.MoveTransform(attacker.bottomTile.transformPos, true, 0.5f);
                });
        }

        attacker.attackable = false;
        UpdateEntityState();
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
                    if (attacker.card.cardType.attack_type != AttackType.shooter)
                        cardMove.Move(attacker, outpostTile);

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
                    if (attacker.card.cardType.attack_type != AttackType.shooter)
                        cardMove.AfterAttackMove(attacker, outpostTile.coordinate);
                    else
                        attacker.MoveTransform(attacker.bottomTile.transformPos, true, 0.5f);
                });
        }
        attacker.attackable = false;
        UpdateEntityState();
    }

    void ResolverEffect(Entity attacker, Entity defender)
    {
        if (attacker.card.ability.effect_Time == EffectTime.Battle)
        {
            if (attacker.card.ability.target.GetTarget() == EffectTarget.TargetCard)
            {
                foreach (var effect in attacker.card.ability.effects)
                {
                    effect.Resolve(defender);
                }
            }
            else if (attacker.card.ability.target.GetTarget() == EffectTarget.ThisCard)
            {
                foreach (var effect in attacker.card.ability.effects)
                {
                    effect.Resolve(attacker);
                }
            }
        }

        if (defender.card.ability.effect_Time == EffectTime.Battle)
        {
            if (defender.card.ability.target.GetTarget() == EffectTarget.TargetCard)
            {
                foreach (var effect in defender.card.ability.effects)
                {
                    effect.Resolve(attacker);
                }
            }
            else if (defender.card.ability.target.GetTarget() == EffectTarget.ThisCard)
            {
                foreach (var effect in defender.card.ability.effects)
                {
                    effect.Resolve(defender);
                }
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

    #endregion

    // 공격시 조준표시 나오게 하는 함수
    private void ShowTargetPicker(bool isShow)
    {
        if (TurnManager.instance.myTurn == false) { return; }

        TargetPicker.SetActive(isShow);

        if (enermyEntityPick != null)
            TargetPicker.transform.position = enermyEntityPick.transform.position;
        else if (selectTile != null && selectTile.tileState == TileState.enermyOutpost)
            TargetPicker.transform.position = selectTile.transform.position;
    }

    public void AttackableReset(bool isMine)
    {
        var targetEntities = isMine ? playerEntities : opponentEntities;
        targetEntities.ForEach(x => x.attackable = true);
    }
}
