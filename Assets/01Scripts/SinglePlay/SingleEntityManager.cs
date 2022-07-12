using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class SingleEntityManager : MonoBehaviour
{
    public static SingleEntityManager instance
    {
        get; set;
    }

    private void Awake()
    {
        instance = this;
    }

    public GraveManager graveManager;
    public EffectManager effectManager;

    [SerializeField] SingleCameraEffect cameraEffect;
    [SerializeField] GameObject entityPrefab;
    [SerializeField] Transform CardCreatePos;
    [SerializeField] Transform playerGraveyard;
    [SerializeField] Transform oppenentGraveyard;
    [SerializeField] GameObject TargetPicker;

    [SerializeField][Header("플레이어 엔티티 부모")] GameObject playerEntityParent;
    [SerializeField][Header("적군 엔티티 부모")] GameObject opponentEntityParent;

    [SerializeField] int maxMoveCount = 2;
    [SerializeField] int maxSummonCount = 2;

    public TMP_Text canMoveCountTMP;
    public TMP_Text canSummonCountTMP;

    public List<SingleEntity> playerEntities;
    public List<SingleEntity> opponentEntities;

    public SingleTile selectTile;
    Coordinate entityCoordinateData;

    // 카드 로그
    public PlayLogControl gameLog;

    // 제물 소환을 위한 변수, CardManager 에서 사용해서 public을 못지웠음
    public bool SelectMonsterMode = false;

    // 마법카드 발동을 위한 변수
    public SingleEffectsSolver effectSolver = new SingleEffectsSolver();

    SingleCardMove cardMove = new SingleCardMove();

    bool CanMouseInput => SingleTurnManager.instance.isLoading == false && onOtherPanel == false;
    bool onOtherPanel = false;

    SingleEntity selectEntity = null;
    public bool selectState => selectEntity != null;

    public SingleEntity targetPickEntity;
    List<SingleEntity> tributeEntity = new List<SingleEntity>();

    [HideInInspector] public int summonCount;
    [HideInInspector] public int canMoveCount;
    [HideInInspector] public int effect_Count;

    [Header("이펙트 모음")]
    [SerializeField] GameObject death_VFX;
    [SerializeField] GameObject explosion_VFX;
    [SerializeField] GameObject magic_VFX;
    [SerializeField] GameObject arrow_VFX;

    [SerializeField][Header("원거리 공격 이펙트")] GameObject bullet_Obj;


    string card_id;
    int entityIDCount = 0;
    int cardCost = 0;
    Card effectCard; // 카드 효과 발동시 정보를 임시 저장하는 변수
    bool effect_Target_Select_State = false;
    public bool tributeSummon, tributeMagic, tutorial_Summon;


    public List<SingleEntity> All_Entities
    {
        get
        {
            List<SingleEntity> allEntities = new List<SingleEntity>(playerEntities);
            allEntities.AddRange(opponentEntities);

            return allEntities;
        }
    }

    //private void Start()
    //{
    //    Init();

    //}
    //private void Update()
    //{
    //    if (targetPickEntity != null || (selectTile != null && selectTile.tileState == TileState.enermyOutpost))
    //    {
    //        ShowTargetPicker(true);
    //    }
    //    else
    //    {
    //        ShowTargetPicker(false);
    //    }
    //}
    //private void OnDestroy()
    //{
    //    SingleTurnManager.OnTurnStarted -= OnTurnStarted;
    //}

    //void Init()
    //{
    //    canMoveCount = maxMoveCount;
    //    summonCount = maxSummonCount;
    //    canMoveCountTMP.text = "0";
    //    canSummonCountTMP.text = "0";
    //    effect_Count = 0;
    //    SingleTurnManager.OnTurnStarted += OnTurnStarted;
    //    tutorial_Summon = false;
    //}

    //void CountTMP_Update()
    //{
    //    canMoveCountTMP.text = canMoveCount.ToString();
    //    canSummonCountTMP.text = summonCount.ToString();
    //}

    //public void MouseEventInit()
    //{
    //    ChangeColor_CanMovePos(false, selectEntity);
    //    selectEntity = null;
    //    targetPickEntity = null;
    //}

    //#region Entity 마우스 관련
    //public void EntityMouseDown(SingleEntity entity)
    //{
    //    selectEntity = entity;
    //    if (CanMouseInput == false)
    //    {
    //        selectEntity = null;
    //        return;
    //    }
    //    if (SelectMonsterMode || effect_Target_Select_State)
    //        return;

    //    if (entity.isMine && entity.attackable)
    //    {
    //        if (canMoveCount >= 0)
    //        {
    //            ChangeColor_CanMovePos(true, entity);
    //        }
    //    }
    //    else
    //    {
    //        ChangeColor_CanMovePos(true, entity, false);
    //    }

    //}

    //public void EntityMouseUP(SingleEntity entity)
    //{
    //    ChangeColor_CanMovePos(false, entity);


    //    if (CanMouseInput == false)
    //        return;

    //    if (entity.isMine && SelectMonsterMode)
    //    {
    //        if (entity.canTribute && entity.isDie == false)
    //        {
    //            SelectTribute(entity.id);
    //            return;
    //        }
    //    }

    //    if (effect_Target_Select_State)
    //    {
    //        Select_Effect_Target(entity.id, entity.isMine);
    //    }



    //    if (selectEntity != null && selectTile != null && selectEntity.attackable && canMoveCount >= 0)
    //    {
    //        if (targetPickEntity)
    //        {
    //            if (cardMove.Can_Attack_Position(selectEntity).Exists(x => x.vector3Pos == targetPickEntity.coordinate.vector3Pos))
    //            {
    //                Attack(selectEntity.id, targetPickEntity.id, true);
    //                canMoveCount--;
    //                return;
    //            }
    //        }

    //        if (selectTile.tileState == TileState.enermyOutpost)
    //        {
    //            if (cardMove.Can_Attack_Position(selectEntity).Exists(x => x.vector3Pos == selectTile.coordinate.vector3Pos))
    //            {
    //                OutpostAttack(selectEntity.id, selectTile.outpost.coordinate, true);
    //                canMoveCount--;
    //                return;
    //            }
    //        }
    //        else
    //        {
    //            if (CardMove(entity.id, entity.isMine, selectTile.coordinate, true))
    //            {
    //                canMoveCount--;
    //            }
    //        }

    //    }
    //    CountTMP_Update();
    //    selectEntity = null;
    //    targetPickEntity = null;
    //}

    //public void EntityMouseOver(SingleEntity entity)
    //{
    //    if (selectEntity == null)
    //        return;

    //    if (entity.isMine == false)
    //    {
    //        targetPickEntity = entity;
    //    }
    //}

    //public void EntityMouseDrag()
    //{
    //    if (!CanMouseInput || selectEntity == null)
    //        return;
    //    if (SelectMonsterMode)
    //        return;

    //    //if (selectTile?.tileState != TileState.onOpponentMonster)
    //    //{
    //    //    targetPickEntity = null;
    //    //}
    //}
    //#endregion

    //// CardEffects 스크립트에서 실행됨. 코드 정리할때 참고
    //public void AddRandomTargetLog(bool targetPlayer, int index)
    //{
    //    SingleEntity targetCard = targetPlayer ? playerEntities[index] : opponentEntities[index];
    //    //// gameLog.Log_Sorter(LogCategory.Effected, targetCard);
    //}

    //void OnTurnStarted(bool myTurn)
    //{
    //    if (selectEntity != null)
    //        ChangeColor_CanMovePos(false, selectEntity);


    //    selectEntity = null;
    //    AttackableReset(myTurn);
    //    canMoveCount = maxMoveCount;
    //    summonCount = maxSummonCount;
    //    effect_Count = 0;

    //    SelectMonsterMode = false;
    //    effect_Target_Select_State = false;
    //    tributeSummon = false;
    //    tributeMagic = false;

    //    if (SingleTurnManager.instance.firstTurn)
    //        summonCount = maxSummonCount - 1;

    //    CountTMP_Update();
    //    UpdateEntityState();
    //}

    //// 카드 소환시 타일 좌표로 정렬시켜주는 함수
    //void EntityAlignment(bool isMine, Vector3 spawnPos)
    //{
    //    var targetEntities = isMine ? playerEntities : opponentEntities;
    //    var targetEntity = targetEntities[targetEntities.Count - 1];
    //    targetEntity.transformPos = spawnPos;
    //    targetEntity.GetComponent<Order>()?.SetOriginOrder(10);
    //}

    //void ChangeColor_CanMovePos(bool changeColor, SingleEntity entity, bool CanMove = true)
    //{
    //    if (entity == null)
    //        return;

    //    foreach (var pos in cardMove.Can_Attack_Position(entity))
    //    {
    //        int x = (int)pos.x;
    //        int y = (int)pos.y;

    //        if (CanMove)
    //            SingleMapManager.instance.mapData[x, y].ColorChange_Rock(changeColor, Color.blue);
    //        else
    //            SingleMapManager.instance.mapData[x, y].ColorChange_Rock(changeColor, Color.black);
    //    }

    //    foreach (var pos in cardMove.FindCanMovePositionList(entity))
    //    {
    //        int x = (int)pos.x;
    //        int y = (int)pos.y;

    //        if (CanMove)
    //            SingleMapManager.instance.mapData[x, y].ColorChange_Rock(changeColor, Color.green);
    //        else
    //            SingleMapManager.instance.mapData[x, y].ColorChange_Rock(changeColor, Color.red);
    //    }
    //}

    //public bool CardMove(int entityID, bool targetPlayer, Coordinate moveCoord, bool isMine)
    //{
    //    if (isMine == false)
    //    {
    //        moveCoord.SetReverse(SingleMapManager.instance.mapSize);
    //        targetPlayer = !targetPlayer;
    //    }

    //    var targetEntities = targetPlayer ? playerEntities : opponentEntities;

    //    return cardMove.Move(targetEntities.Find(x => x.id == entityID), SingleMapManager.instance.coordinateTile(moveCoord));
    //}

    //#region 소환 관련
    //void SummonBase(bool isMine, Card card, SingleTile summonTile)
    //{
    //    Vector3 spawnPos = CardCreatePos.position;
    //    GameObject parent = isMine ? playerEntityParent : opponentEntityParent;

    //    if (isMine == false)
    //        spawnPos.y = -spawnPos.y;

    //    // SingleEntity emptyEntity = this.gameObject.AddComponent<SingleEntity>();

    //    var entityObject = Instantiate(entityPrefab, spawnPos, Utils.QI, parent.transform);

    //    string sumnmonPlayer = isMine ? "Player : " : "Opponent : ";
    //    entityObject.name = sumnmonPlayer + card.name + entityIDCount;

    //    var entity = entityObject.GetComponent<SingleEntity>();
    //    entity.Setup(card, isMine);


    //    if (isMine)
    //    {
    //        playerEntities.Add(entity);
    //        int index = playerEntities.IndexOf(entity);
    //        playerEntities[index] = entity;
    //        entity.id = entityIDCount;
    //        entity.coordinate = summonTile.coordinate;
    //        if (SelectMonsterMode)
    //        {
    //            entity.coordinate = entityCoordinateData;
    //            SingleTile targetTile = SingleMapManager.instance.coordinateTile(entityCoordinateData);
    //            targetTile.SetMonster(entity);
    //            EntityAlignment(isMine, targetTile.transformPos);
    //        }
    //        else
    //        {
    //            summonTile.SetMonster(entity);
    //            EntityAlignment(isMine, summonTile.transformPos);
    //        }
    //    }
    //    else
    //    {
    //        opponentEntities.Add(entity);
    //        entity.id = entityIDCount;
    //        entity.coordinate = summonTile.coordinate;

    //        summonTile.SetMonster(entity);
    //        EntityAlignment(isMine, summonTile.transformPos);
    //    }
    //    entityIDCount++;
    //    summonCount--;

    //    CountTMP_Update();

    //    // // gameLog.Log_Sorter(LogCategory.Summon, entity);

    //    Destroy(gameObject.GetComponent<Entity>());

    //    effectSolver.Add_Activated_Effect_To_Entity(entity);

    //    if (card.ability.effect_Time != EffectTime.Battle)
    //    {
    //        EffectTrigger(isMine, card.id);
    //    }
    //    entity.FrameColorRefresh();


    //}

    //public void SetCoordinateData(Vector3 coordVec)
    //{
    //    entityCoordinateData = new Coordinate(coordVec);
    //}


    //public void Summon(bool isMine, string card_id, Coordinate coordinate)
    //{
    //    Card card = DataManager.instance.CardData(card_id);

    //    SingleTile targetTile = SingleMapManager.instance.coordinateTile(coordinate);
    //    if (targetTile == null || targetTile.tileState != TileState.empty)
    //        return;
    //    if (summonCount <= 0)
    //        return;
    //    if (targetTile.canSpawn == CanSpawn.nothing)
    //        return;

    //    if (targetTile.canSpawn == CanSpawn.playerCanSpawn && isMine == false)
    //        return;
    //    if (targetTile.canSpawn == CanSpawn.opponentCanSpawn && isMine)
    //        return;
    //    this.card_id = card_id;

    //    cardCost = card.cost;

    //    if (card.cost == 0)
    //    {
    //        SummonBase(isMine, card, targetTile);
    //        SingleCardManager.instance.RemoveTargetCards(isMine);
    //        return;
    //    }
    //    else
    //    {
    //        if (isMine && playerEntities.Count >= card.cost)
    //        {
    //            SetCoordinateData(targetTile.coordinate.vector3Pos);
    //            onOtherPanel = true;
    //            //SingleManager.instance.Confirmation("제물이 " + card.cost + "마리 필요합니다.\n" + "소환\n하시겠습니까?");
    //            tributeSummon = true;
    //            ConfirmSelect(true);
    //        }
    //        return;
    //    }
    //}

    //public void ConfirmSelect(bool select)
    //{
    //    if (select)
    //    {
    //        SelectTributeSummon();
    //    }
    //    else
    //    {
    //        tributeSummon = false;
    //        tributeMagic = false;
    //    }
    //    onOtherPanel = false;
    //}

    //void SelectTributeSummon()
    //{
    //    if (tributeSummon)
    //    {
    //        tutorial_Summon = true;
    //        SingleManager.instance.Notification("제물을\n선택해주세요");
    //        SelectMonsterMode = true;
    //        SingleCardManager.instance.TributeSummonSet(false); // 제물소환시 소환할 카드 잠깐 안보이게함
    //    }
    //    else if (tributeMagic)
    //    {
    //        SingleManager.instance.Notification("제물을\n선택해주세요"); // 이미지 너무 구려
    //        SelectMonsterMode = true;
    //        SingleCardManager.instance.TributeSummonSet(false); // 제물소환시 소환할 카드 잠깐 안보이게함
    //    }
    //}


    //void TributeSummon(bool isMine, string card_id)
    //{
    //    if (!isMine)
    //        SingleCardManager.instance.Remove_OhterPlayer_HandCard();

    //    Card card = DataManager.instance.CardData(card_id);

    //    SingleCardManager.instance.RemoveTargetCards(isMine);


    //    foreach (var entity in tributeEntity)
    //    {
    //        // // gameLog.Log_Sorter(LogCategory.Sacrifice, entity);
    //        entity.isDie = true;

    //        UpdateEntityState(); // 죽는것과 제물을 구분해야하는가? 이펙트 생각하면 하는게 좋긴할듯 jsg
    //    }

    //    if (isMine == false)
    //    {
    //        entityCoordinateData.SetReverse(MapManager.instance.mapSize);
    //    }

    //    SingleTile targetTile = SingleMapManager.instance.coordinateTile(entityCoordinateData);
    //    SummonBase(isMine, card, targetTile);
    //    tributeEntity.Clear();

    //    cardCost = 0;
    //    SelectMonsterMode = false;
    //}

    //void TributeMagic(bool isMine, string card_id)
    //{
    //    Card card = DataManager.instance.CardData(card_id);

    //    if (isMine)
    //    {
    //        foreach (var entity in tributeEntity)
    //        {
    //            // GameManager.instance.localGamePlayerScript.CmdSelectTribute(entity.id, isServer);
    //            //OldNetworkManager.Inst.SelectedTribute(entity);
    //        }
    //    }
    //    else
    //    {
    //        SingleCardManager.instance.Remove_OhterPlayer_HandCard();
    //    }

    //    SingleCardManager.instance.RemoveTargetCards(isMine);

    //    foreach (var entity in tributeEntity)
    //    {
    //        // // gameLog.Log_Sorter(LogCategory.Sacrifice, entity);
    //        entity.isDie = true;

    //        UpdateEntityState(); // 죽는것과 제물을 구분해야하는가? 이펙트 생각하면 하는게 좋긴할듯 jsg
    //    }

    //    EffectSolve(card.id, isMine);
    //    tributeEntity.Clear();

    //    cardCost = 0;
    //    SelectMonsterMode = false;
    //}

    //public void SelectTribute(int entityID)
    //{
    //    SingleEntity entity = playerEntities.Find(x => x.id == entityID);

    //    entity.canTribute = false;
    //    tributeEntity.Add(entity);

    //    if (cardCost <= tributeEntity.Count)
    //    {
    //        if (DataManager.instance.CardData(card_id).cardType.card_category == CardCategory.Monster)
    //        {
    //            TributeSummon(true, this.card_id);
    //        }
    //        else if (DataManager.instance.CardData(card_id).cardType.card_category == CardCategory.Magic)
    //        {
    //            TributeMagic(true, card_id);
    //        }
    //    }
    //}
    //#endregion


    //#region Magic Card
    //IEnumerator DelaySelectTarget()
    //{
    //    yield return new WaitForSeconds(0.7f);
    //    effect_Target_Select_State = true;
    //}

    //public void ConfirmEffectTrigger(bool isTrigger, string msg = "타겟을\n 선택 하세요")
    //{
    //    if (isTrigger)
    //    {
    //        SingleManager.instance.Notification(msg);
    //        StartCoroutine(DelaySelectTarget());
    //    }
    //    else
    //    {

    //    }
    //}

    //public bool EffectTrigger(bool isMine, string card_id)
    //{
    //    if (selectTile == null && isMine)
    //        return false;
    //    effectCard = DataManager.instance.CardData(card_id);

    //    if (effectSolver.EffectRequireExist(!isMine, playerEntities, opponentEntities, effectCard) == false)
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
    //        // // gameLog.Log_Sorter(LogCategory.Magic, effectCard, isMine);
    //        return true;
    //    }
    //    else if (effectCard.ability.effect_Time == EffectTime.Triggered)
    //    {
    //        if (isMine)
    //        {
    //            EffectSolve(effectCard.id, isMine);
    //        }
    //        return true;
    //    }

    //    if (effectCard.cost == 0 || effectCard.cardType.card_category == CardCategory.Monster)
    //    {
    //        if (isMine)
    //        {
    //            EffectSolve(effectCard.id, isMine);
    //        }
    //        return true;
    //    }
    //    else if (effectCard.cost > 0)
    //    {
    //        if (effectCard.cost <= tributeEntity.Count)
    //        {
    //            EffectSolve(effectCard.id, isMine);
    //            return true;
    //        }

    //        if (effectCard.cost <= playerEntities.Count && isMine)
    //        {
    //            //SingleManager.instance.Confirmation("제물이 " + effectCard.cost + "마리 필요합니다.\n" + "사용\n하시겠습니까?");
    //            // // gameLog.Log_Sorter(LogCategory.Magic, effectCard, isMine);
    //            onOtherPanel = true;
    //            tributeMagic = true;
    //        }
    //    }
    //    return false;
    //}

    //public void EffectSolve(string card_id, bool isMine)
    //{
    //    Card effectCard = DataManager.instance.CardData(card_id);
    //    if (effectCard.ability == null)
    //        return;

    //    switch (effectCard.ability.target.GetTarget())
    //    {
    //        case EffectTarget.TargetCard:
    //            if (isMine)
    //            {
    //                ConfirmEffectTrigger(true);
    //            }
    //            break;
    //        case EffectTarget.RandomCard:
    //            if (playerEntities.Count != 0 || opponentEntities.Count != 0)
    //            {
    //                if (isMine)
    //                {
    //                    effectSolver.NonTargetEffectActive(effectCard, playerEntities, opponentEntities);

    //                    UpdateEntityState();
    //                }
    //            }
    //            break;
    //        case EffectTarget.AllCards:
    //            effectSolver.NonTargetEffectActive(effectCard, playerEntities, opponentEntities);
    //            UpdateEntityState();
    //            break;
    //        case EffectTarget.TribeTarget:
    //            if (isMine)
    //            {
    //                ConfirmEffectTrigger(true, effectCard.TribeStr() + " 타겟을\n선택해주세요");
    //            }
    //            break;
    //        case EffectTarget.PlayerWarrior:
    //            effectSolver.NonTargetEffectActive(effectCard, playerEntities, opponentEntities);
    //            UpdateEntityState();
    //            break;

    //        case EffectTarget.TargetWarrior:
    //            if (isMine)
    //            {
    //                //if (effectCard.cardType.card_category == CardCategory.Monster)
    //                //    //SingleManager.instance.EffectConfirm("효과를 발동 하시겠습니까?");
    //                //else
    //                ConfirmEffectTrigger(true);

    //            }
    //            break;
    //        default:
    //            return;
    //    }

    //    //// gameLog.Log_Sorter(LogCategory.Magic, effectCard, isMine); // 로그 추가
    //}

    //public void Select_Effect_Target(int entityID, bool targetPlayer)
    //{
    //    var targetEntities = targetPlayer ? playerEntities : opponentEntities;

    //    SingleEntity targetEntity = targetEntities.Find(x => x.id == entityID);

    //    if (targetEntity == null)
    //        return;

    //    EffectTarget effectTarget = effectCard.ability.target.GetTarget();

    //    if (effectTarget == EffectTarget.PlayerCard && targetEntity.isMine == targetPlayer)
    //    {
    //        effect_Target_Select_State = false;
    //    }
    //    else if (effectTarget == EffectTarget.EnermyCard && targetEntity.isMine)
    //    {
    //        effect_Target_Select_State = false;
    //    }
    //    else if (effectTarget == EffectTarget.TargetCard)
    //    {
    //        effect_Target_Select_State = false;
    //    }
    //    else if (effectTarget == EffectTarget.TribeTarget)
    //    {
    //        if (targetEntity.card.cardType.tribe == effectCard.cardType.tribe)
    //            effect_Target_Select_State = false;
    //        else
    //            SingleManager.instance.Notification(effectCard.TribeStr() + " 타겟을\n추가 선택 해주세요");
    //    }
    //    if (effect_Target_Select_State == false)
    //    {
    //        // // gameLog.Log_Sorter(LogCategory.Effected, targetEntity);

    //        foreach (var effect in effectCard.ability.effects)
    //        {
    //            effect.Resolve(targetEntity);
    //        }
    //        UpdateEntityState();
    //        effect_Count++;
    //        effectCard = null;
    //    }
    //}

    //public void RandomTargetEffect(SingleEntity targetEntity)
    //{
    //    // // gameLog.Log_Sorter(LogCategory.Effected, targetEntity);
    //    effectSolver.ReceiveRandomEffect(targetEntity, card_id); // jsg

    //    UpdateEntityState();
    //}

    //// 몬스터 사망시 처리되는 함수
    //public void UpdateEntityState()
    //{
    //    List<SingleEntity> destroiedEntity = new List<SingleEntity>();
    //    foreach (var playerEntity in playerEntities)
    //    {
    //        if (playerEntity.isDie)
    //        {
    //            if (playerEntity.card.ability.effect_Time == EffectTime.Activated)
    //            {
    //                effectSolver.ReverseEffect(playerEntity, playerEntities, opponentEntities);
    //            }
    //            SingleMapManager.instance.mapData[playerEntity.coordinate.x, playerEntity.coordinate.y].tileState = TileState.empty;
    //            GoToGraveyard(playerEntity);
    //            destroiedEntity.Add(playerEntity);
    //        }
    //    }

    //    foreach (var opponentEntity in opponentEntities)
    //    {
    //        if (opponentEntity.isDie)
    //        {
    //            if (opponentEntity.card.ability.effect_Time == EffectTime.Activated)
    //            {
    //                effectSolver.ReverseEffect(opponentEntity, playerEntities, opponentEntities);
    //            }
    //            SingleMapManager.instance.mapData[opponentEntity.coordinate.x, opponentEntity.coordinate.y].tileState = TileState.empty;
    //            GoToGraveyard(opponentEntity);
    //            destroiedEntity.Add(opponentEntity);
    //        }
    //    }

    //    foreach (var destroyEntity in destroiedEntity)
    //    {
    //        if (playerEntities.Contains(destroyEntity))
    //            playerEntities.Remove(destroyEntity);
    //        else if (opponentEntities.Contains(destroyEntity))
    //            opponentEntities.Remove(destroyEntity);
    //    }
    //}

    //void GoToGraveyard(SingleEntity entity)
    //{
    //    Vector3 gravePosition = entity.isMine ? playerGraveyard.position : oppenentGraveyard.position;

    //    graveManager.AddGraveCard(entity.card, entity.isMine);

    //    Sequence sequence = DOTween.Sequence()
    //            .Append(entity.transform.DOShakePosition(1.3f))
    //            .Append(entity.transform.DOMove(gravePosition, 0.5f))
    //            .Append(entity.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.OutCirc))
    //            .OnComplete(() =>
    //            {
    //                Destroy(gameObject.GetComponent<Entity>());
    //            });
    //}

    //#endregion

    //#region  Battle

    //public void Attack(int attackerID, int defenderID, bool isMine)
    //{
    //    SingleEntity attacker, defender;
    //    attacker = All_Entities.Find(x => x.id == attackerID);
    //    defender = All_Entities.Find(x => x.id == defenderID);

    //    int attackerBP = attacker.GetEffectiveValue("bp");
    //    int defenderBP = defender.GetEffectiveValue("bp");

    //    if (defender.card.cardType.attack_type == AttackType.shooter)
    //        attackerBP = attackerBP * 2;

    //    attacker.GetComponent<Order>().SetMostFrontOrder(true);

    //    Vector3 attackPosition = defender.transformPos;
    //    ResolverEffect(attacker, defender);
    //    if (attacker.card.cardType.attack_type != AttackType.shooter)
    //    {
    //        Sequence sequence = DOTween.Sequence()
    //                .Append(attacker.transform.DOMove(attackPosition, 0.4f)).SetEase(Ease.InSine)
    //                .AppendCallback(() =>
    //                {
    //                    GameObject spawnedVFX = Instantiate(explosion_VFX, attackPosition, transform.rotation) as GameObject;
    //                    attacker.Damaged(defenderBP);
    //                    defender.Damaged(attackerBP);
    //                    Destroy(spawnedVFX, 5f);
    //                }).OnComplete(() => AttackCallback(attacker, defender));
    //    }
    //    else
    //    {
    //        GameObject bullet = Instantiate(bullet_Obj, attacker.transformPos, transform.rotation) as GameObject;
    //        Sequence sequence = DOTween.Sequence()
    //                .Append(bullet.transform.DOMove(attackPosition, 0.4f)).SetEase(Ease.InSine)
    //                .AppendCallback(() =>
    //                {
    //                    GameObject spawnedVFX = Instantiate(explosion_VFX, defender.transform.position, transform.rotation) as GameObject;
    //                    Destroy(spawnedVFX, 5f);
    //                }).OnComplete(() => Destroy(bullet, 1f));

    //        defender.Damaged(attackerBP);
    //    }
    //    UpdateEntityState();
    //}

    //public void OutpostAttack(int attackerID, Coordinate outpostCoord, bool isMine)
    //{
    //    if (isMine == false)
    //        outpostCoord.SetReverse(SingleMapManager.instance.mapSize);

    //    SingleEntity attacker;
    //    Outpost outpost = SingleMapManager.instance.coordinateTile(outpostCoord).outpost;

    //    attacker = isMine ? playerEntities.Find(x => x.id == attackerID) : opponentEntities.Find(x => x.id == attackerID);
    //    attacker.GetComponent<Order>().SetMostFrontOrder(true);

    //    Vector3 attackPosition = outpost.transformPos;

    //    Sequence sequence = DOTween.Sequence()
    //    .Append(attacker.transform.DOMove(attackPosition, 0.4f)).SetEase(Ease.InSine)
    //    .AppendCallback(() =>
    //    {
    //        outpost.Damaged(attacker.GetEffectiveValue("bp"));
    //    }).OnComplete(() => OutpostAttackCallback(attacker, outpost));
    //}

    //void AttackCallback(SingleEntity attacker, SingleEntity defender)
    //{
    //    attacker.GetComponent<Order>().SetMostFrontOrder(false);

    //    if (defender.isDie)
    //    {
    //        defender.bottomTile.tileState = TileState.empty;

    //        if (attacker.isDie == false)
    //        {
    //            Sequence sequence = DOTween.Sequence()
    //            .Append(defender.transform.DOShakePosition(0.5f))
    //            .Append(defender.transform.DOScale(Vector3.one, 0.3f)).SetEase(Ease.OutCirc)
    //            .Append(attacker.transform.DOMove(defender.transformPos, 0.1f).SetEase(Ease.InSine))
    //            .AppendCallback(() =>
    //            {
    //                if (attacker.card.role != AttackType.shooter)
    //                    cardMove.Move(attacker, defender.bottomTile);
    //                else
    //                    attacker.MoveTransform(attacker.bottomTile.transformPos, true, 0.5f);
    //            });
    //        }
    //        else
    //        {
    //            attacker.bottomTile.tileState = TileState.empty;
    //        }
    //    }
    //    else
    //    {
    //        Sequence sequence = DOTween.Sequence()
    //            .Append(attacker.transform.DOShakePosition(0.5f))
    //            .AppendCallback(() =>
    //            {
    //                if (attacker.card.role != AttackType.shooter)
    //                    cardMove.AfterAttackMove(attacker, defender.coordinate);
    //                else
    //                    attacker.MoveTransform(attacker.bottomTile.transformPos, true, 0.5f);
    //            });
    //    }

    //    attacker.attackable = false;
    //    UpdateEntityState();
    //}

    //void OutpostAttackCallback(SingleEntity attacker, Outpost outpost)
    //{
    //    SingleTile outpostTile = SingleMapManager.instance.mapData[outpost.coordinate.x, outpost.coordinate.y];

    //    attacker.GetComponent<Order>().SetMostFrontOrder(false);
    //    if (outpost.isDie)
    //    {
    //        outpostTile.tileState = TileState.empty;

    //        Sequence sequence = DOTween.Sequence()
    //            .Append(outpost.transform.DOShakePosition(0.5f))
    //            .Append(attacker.transform.DOMove(outpost.transformPos, 0.1f).SetEase(Ease.InSine))
    //            .AppendCallback(() =>
    //            {
    //                if (attacker.card.role != AttackType.shooter)
    //                    cardMove.Move(attacker, outpostTile);

    //                SingleMapManager.instance.OutpostDestroy(outpost, attacker.isMine);

    //                outpostTile.outpost_object.SetActive(false);
    //                CheckGameResult();
    //            });
    //    }
    //    else
    //    {
    //        Sequence sequence = DOTween.Sequence()
    //            .Append(outpost.transform.DOShakePosition(0.5f))
    //            .AppendCallback(() =>
    //            {
    //                if (attacker.card.role != AttackType.shooter)
    //                    cardMove.AfterAttackMove(attacker, outpostTile.coordinate);
    //                else
    //                    attacker.MoveTransform(attacker.bottomTile.transformPos, true, 0.5f);
    //            });
    //    }
    //    attacker.attackable = false;
    //    SingleEntityManager.instance.UpdateEntityState();
    //}

    //void ResolverEffect(SingleEntity attacker, SingleEntity defender)
    //{
    //    if (attacker.card.ability.effect_Time == EffectTime.Battle)
    //    {
    //        if (attacker.card.ability.target.GetTarget() == EffectTarget.TargetCard)
    //        {
    //            foreach (var effect in attacker.card.ability.effects)
    //            {
    //                effect.Resolve(defender);
    //            }
    //        }
    //        else if (attacker.card.ability.target.GetTarget() == EffectTarget.ThisCard)
    //        {
    //            foreach (var effect in attacker.card.ability.effects)
    //            {
    //                effect.Resolve(attacker);
    //            }
    //        }
    //    }

    //    if (defender.card.ability.effect_Time == EffectTime.Battle)
    //    {
    //        if (defender.card.ability.target.GetTarget() == EffectTarget.TargetCard)
    //        {
    //            foreach (var effect in defender.card.ability.effects)
    //            {
    //                effect.Resolve(attacker);
    //            }
    //        }
    //        else if (defender.card.ability.target.GetTarget() == EffectTarget.ThisCard)
    //        {
    //            foreach (var effect in defender.card.ability.effects)
    //            {
    //                effect.Resolve(defender);
    //            }
    //        }
    //    }
    //}

    //void CheckGameResult()
    //{
    //    if (SingleMapManager.instance.livePlayerOutpost <= 0)
    //        SingleManager.instance.GameResult(true);

    //    if (SingleMapManager.instance.liveOpponentOutpost <= 0)
    //        SingleManager.instance.GameResult(false);
    //}
    //#endregion

    //// 공격시 조준표시 나오게 하는 함수
    //private void ShowTargetPicker(bool isShow)
    //{
    //    if (SingleTurnManager.instance.myTurn == false) { return; }

    //    TargetPicker.SetActive(isShow);

    //    if (targetPickEntity != null)
    //        TargetPicker.transform.position = targetPickEntity.transform.position;
    //    else if (selectTile != null && selectTile.tileState == TileState.enermyOutpost)
    //        TargetPicker.transform.position = selectTile.transform.position;
    //}

    //public void AttackableReset(bool isMine)
    //{
    //    var targetEntities = isMine ? playerEntities : opponentEntities;
    //    targetEntities.ForEach(x => x.attackable = true);
    //}
}
