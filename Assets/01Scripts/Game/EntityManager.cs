using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mirror;

using UnityEngine;
using DG.Tweening;

public class EntityManager : MonoBehaviour
{
    public static EntityManager instance { get; set; }

    private void Awake()
    {
        instance = this;
    }

    public GraveSet graveManager;

    [SerializeField] CameraEffect cameraEffect;
    [SerializeField] GameObject entityPrefab;
    [SerializeField] Transform myCardCreatePos;
    [SerializeField] Transform otherCardCreatePos;
    [SerializeField] Transform playerGraveyard;
    [SerializeField] Transform oppenentGraveyard;
    [SerializeField] GameObject TargetPicker;
    [SerializeField] int maxMoveCount = 2;

    public List<Entity> playerEntities;
    public List<Entity> opponentEntities;

    public Tile selectTile;
    Coordinate entityCoordinateData;

    // 카드 로그
    public PlayLogControl gameLog;

    // 제물 소환을 위한 변수, CardManager 에서 사용해서 public을 못지웠음
    public bool SelectTributeMode = false;

    // 마법카드 발동을 위한 변수
    public EffectsSolver effectSolver = new EffectsSolver();
    public EffectExplanation effectExplanation;

    CardMove cardMove = new CardMove();
    Battle battle = new Battle();


    bool CanMouseInput => TurnManager.instance.isLoading == false && onOtherPanel == false;
    bool onOtherPanel = false;

    Entity selectEntity = null;
    public bool selectState => selectEntity != null;

    Entity targetPickEntity;
    List<Entity> tributeEntity = new List<Entity>();   

    string card_id;
    int summonCount;
    int moveCount = 0;
    int entityIDCount = 0;
    int cardCost = 0;
    Card effectCard; // 카드 효과 발동시 정보를 임시 저장하는 변수
    bool effect_Target_Select_State = false;
    bool tributeSummon, tributeMagic;


    private void Start()
    {
        TurnManager.OnTurnStarted += OnTurnStarted;

    }
    private void Update()
    {
        if (targetPickEntity != null || (selectTile != null && selectTile.tileState == TileState.opponentOutpost))
        {
            ShowTargetPicker(true);
        }
        else
        {
            ShowTargetPicker(false);
        }
    }
    private void OnDestroy()
    {
        TurnManager.OnTurnStarted -= OnTurnStarted;
    }

    public void MouseEventInit()
    {
        ChangeColor_CanMovePos(false, selectEntity);
        selectEntity = null;
        targetPickEntity = null;
    }

    #region Entity 마우스 관련
    public void EntityMouseDown(Entity entity)
    {
        selectEntity = entity;
        if (CanMouseInput == false)
        {
            selectEntity = null;
            return;
        }
        if (SelectTributeMode || effect_Target_Select_State)
            return;

        if (entity.isMine && entity.attackable)
        {
            if (moveCount < maxMoveCount)
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
        ChangeColor_CanMovePos(false, entity);

        if (CanMouseInput == false)
            return;

        if (entity.isMine && SelectTributeMode)
        {
            if (entity.canTribute && entity.isDie == false)
            {
                GameManager.instance.localGamePlayerScript.CmdSelectTribute(entity.id, NetworkRpcFunc.instance.isServer);
                return;
            }
        }

        if (effect_Target_Select_State)
        {
            GameManager.instance.localGamePlayerScript.CmdSelectEffectTarget(entity.id, entity.isMine, NetworkRpcFunc.instance.isServer);
        }

        

        if (selectEntity != null && selectTile != null && selectEntity.attackable && moveCount < maxMoveCount)
        {
            if (targetPickEntity)
            {
                if(cardMove.Can_Attack_Position(selectEntity).Exists(x => x.vector3Pos == targetPickEntity.coordinate.vector3Pos))
                    GameManager.instance.localGamePlayerScript.CmdAttack(selectEntity.id, targetPickEntity.id, NetworkRpcFunc.instance.isServer);
            }

            if (selectTile.tileState == TileState.opponentOutpost)
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
                    entity.id, entity.isMine, selectTile.coordinate.vector3Pos, NetworkRpcFunc.instance.isServer);
                moveCount = (GameManager.instance.localGamePlayerScript.canMove) ? moveCount++ : moveCount;
                GameManager.instance.localGamePlayerScript.canMove = false;
            }
        }
        selectEntity = null;
        targetPickEntity = null;
    }

    public void EntityMouseOver(Entity entity)
    {
        if (selectEntity == null)
            return;

        if(entity.isMine == false)
        {
            targetPickEntity = entity;
        }
    }

    public void EntityMouseDrag()
    {
        if (!CanMouseInput || selectEntity == null)
            return;
        if (SelectTributeMode)
            return;

        //if (selectTile?.tileState != TileState.onOpponentMonster)
        //{
        //    targetPickEntity = null;
        //}
    }
    #endregion

    // CardEffects 스크립트에서 실행됨. 코드 정리할때 참고
    public void AddRandomTargetLog(bool targetPlayer, int index)
    {
        Entity targetCard = targetPlayer ? playerEntities[index] : opponentEntities[index];
        gameLog.Log_Sorter(LogCategory.Effected, targetCard);
    }

    void OnTurnStarted(bool myTurn)
    {
        if (selectEntity != null)
            ChangeColor_CanMovePos(false, selectEntity);

        selectEntity = null;
        AttackableReset(myTurn);
        moveCount = 0;
        SelectTributeMode = false;
        effect_Target_Select_State = false;
        tributeSummon = false;
        tributeMagic = false;
        summonCount = 0;
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
                MapManager.instance.mapData[x, y].CanMovePos_ChangeTheColor(changeColor, Color.blue);
            else
                MapManager.instance.mapData[x, y].CanMovePos_ChangeTheColor(changeColor, Color.black);
        }

        foreach (var pos in cardMove.FindCanMovePositionList(entity))
        {
            int x = (int)pos.x;
            int y = (int)pos.y;

            if(CanMove)
                MapManager.instance.mapData[x, y].CanMovePos_ChangeTheColor(changeColor, Color.green);
            else
                MapManager.instance.mapData[x, y].CanMovePos_ChangeTheColor(changeColor, Color.red);
        }       
    }

    public void CardMove(int entityID, bool targetPlayer, Vector3 movePos, bool server)
    {
        bool isMine = NetworkRpcFunc.instance.isServer == server;
        Coordinate moveCoord = new Coordinate(movePos);

        if (isMine == false)
        {
            moveCoord.SetReverse(MapManager.instance.mapSize);
            targetPlayer = !targetPlayer;
        }

        var targetEntities = targetPlayer ? playerEntities : opponentEntities;

        cardMove.Move(targetEntities.Find(x => x.id == entityID), MapManager.instance.coordinateTile(moveCoord));
    }

    public void Attack(int attackerID, int defenderID, bool server)
    {
        Entity attacker, defender;
        bool isMine = NetworkRpcFunc.instance.isServer == server;
        if (isMine)
        {
            attacker = playerEntities.Find(x => x.id == attackerID);
            defender = opponentEntities.Find(x => x.id == defenderID);
        }
        else
        {
            attacker = opponentEntities.Find(x => x.id == attackerID);
            defender = playerEntities.Find(x => x.id == defenderID);
        }
        
        battle.Attack(attacker, defender);
    }

    public void OutpostAttack(int attackerID, Coordinate outpostCoord, bool server)
    {
        bool isMine = NetworkRpcFunc.instance.isServer == server;
        if (isMine == false)
            outpostCoord.SetReverse(MapManager.instance.mapSize);

        Entity attacker;
        Outpost targetOutpost = MapManager.instance.coordinateTile(outpostCoord).outpost;

        if (isMine)
            attacker = playerEntities.Find(x => x.id == attackerID);        
        else
            attacker = opponentEntities.Find(x => x.id == attackerID);

        Debug.Log("공격자 : " + attacker.name + " 거점 좌표 : " + targetOutpost.coordinate.vector3Pos);

        battle.Attack(attacker, targetOutpost, NetworkRpcFunc.instance.isServer);
    }

    #region 소환 관련
    void SummonBase(bool isMine, Card card, Tile summonTile)
    {
        Vector3 spawnPos = isMine ? myCardCreatePos.position : otherCardCreatePos.position; ;
        Entity emptyEntity = this.gameObject.AddComponent<Entity>();
        var entityObject = Instantiate(entityPrefab, spawnPos, Utils.QI);

        string sumnmonPlayer = isMine ? "Player : " : "Opponent : ";
        entityObject.name = sumnmonPlayer + card.name + entityIDCount;
        
        if (!isMine)
            entityObject.GetComponent<Entity>().OppenentFeildCardColor(Color.red);

        var entity = entityObject.GetComponent<Entity>();
        entity.isMine = isMine;
        entity.Setup(card);
        if (isMine)
        {
            playerEntities.Add(emptyEntity);
            int index = playerEntities.IndexOf(emptyEntity);
            playerEntities[index] = entity;
            entity.id = entityIDCount;
            entity.coordinate = summonTile.coordinate;
            if (SelectTributeMode)
            {
                entity.coordinate = entityCoordinateData;
                Tile targetTile = MapManager.instance.coordinateTile(entityCoordinateData);
                targetTile.SetMonster(entity);
                EntityAlignment(isMine, targetTile.transformPos);
            }
            else
            {
                summonTile.SetMonster(entity);
                EntityAlignment(isMine, summonTile.transformPos);
            }
        }
        else
        {
            opponentEntities.Add(entity);
            entity.id = entityIDCount;
            entity.coordinate = summonTile.coordinate;

            summonTile.SetMonster(entity);
            EntityAlignment(isMine, summonTile.transformPos);
        }
        entityIDCount++;
        summonCount++;

        gameLog.Log_Sorter(LogCategory.Summon ,entity);

        Destroy(gameObject.GetComponent<Entity>());

        effectSolver.Add_Activated_Effect_To_Entity(entity);

        if(card.ability.effect_Class != EffectClass.Battle)
        {
            EffectTrigger(isMine, card.card_code);
        }
    }

    public void SetCoordinateData(Vector3 coordVec)
    {
        entityCoordinateData = new Coordinate(coordVec);
    }


    public void Summon(bool server, string card_id, Coordinate coordinate)
    {
        bool isMine = server == NetworkRpcFunc.instance.isServer; ;
        Card card = CardDatabase.instance.CardData(card_id);


        if(isMine == false)
        {
            coordinate.SetReverse(MapManager.instance.mapSize);
        }
        Tile targetTile = MapManager.instance.coordinateTile(coordinate);

        if (targetTile == null|| targetTile.tileState != TileState.empty)
            return;
        if (summonCount >= 2)
            return;
        if (TurnManager.instance.firstTurn && summonCount >= 1)
            return;
        if (targetTile.canSpawn == CanSpawn.nothing)
            return;
        if (targetTile.canSpawn == CanSpawn.playerCanSpawn && isMine == false)
            return;
        if (targetTile.canSpawn == CanSpawn.opponentCanSpawn && isMine)
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
                GameManager.instance.localGamePlayerScript.CmdSetSummonCoord(targetTile.coordinate.vector3Pos);
                onOtherPanel = true;
                GameManager.instance.Confirmation("제물이 " + card.cost + "마리 필요합니다.\n" + "소환\n하시겠습니까?");
                tributeSummon = true;
            }
            return;
        }
    }

    public void ConfirmSelect(bool select)
    {
        if (select)
        {
            SelectTributeSummon();
        }
        else
        {
            tributeSummon = false;
            tributeMagic = false;
        }
        onOtherPanel = false;
    }

    void SelectTributeSummon()
    {
        if(tributeSummon)
        {
            GameManager.instance.Notification("제물을\n선택해주세요"); 
            SelectTributeMode = true;
            CardManager.instance.TributeSummonSet(false); // 제물소환시 소환할 카드 잠깐 안보이게함
        }
        else if(tributeMagic)
        {
            GameManager.instance.Notification("제물을\n선택해주세요"); // 이미지 너무 구려
            SelectTributeMode = true;
            CardManager.instance.TributeSummonSet(false); // 제물소환시 소환할 카드 잠깐 안보이게함
        }
    }

    void TributeSummon(bool isMine, string card_id)
    {
        if(!isMine)
            CardManager.instance.Remove_OhterPlayer_HandCard();

        Card card = CardDatabase.instance.CardData(card_id);

        CardManager.instance.RemoveTargetCards(isMine);


        foreach (var entity in tributeEntity)
        {
            gameLog.Log_Sorter(LogCategory.Sacrifice, entity);
            entity.isDie = true;

            UpdateEntityState(); // 죽는것과 제물을 구분해야하는가? 이펙트 생각하면 하는게 좋긴할듯 jsg
        }

        if (isMine == false)
            entityCoordinateData.SetReverse(MapManager.instance.mapSize);

        Tile targetTile = MapManager.instance.coordinateTile(entityCoordinateData);
        SummonBase(isMine, card, targetTile);
        tributeEntity.Clear();

        cardCost = 0;
        SelectTributeMode = false;
    }

    void TributeMagic(bool isMine, string card_id)
    {
        Card item = CardDatabase.instance.CardData(card_id);

        if (isMine)
        {
            foreach (var entity in tributeEntity)
            {
                // GameManager.instance.localGamePlayerScript.CmdSelectTribute(entity.id, isServer);
                //OldNetworkManager.Inst.SelectedTribute(entity);
            }
        }
        else
        {
            CardManager.instance.Remove_OhterPlayer_HandCard();
        }

        CardManager.instance.RemoveTargetCards(isMine);

        foreach (var entity in tributeEntity)
        {
            gameLog.Log_Sorter(LogCategory.Sacrifice , entity);
            entity.isDie = true;

            UpdateEntityState(); // 죽는것과 제물을 구분해야하는가? 이펙트 생각하면 하는게 좋긴할듯 jsg
        }

        GameManager.instance.localGamePlayerScript.CmdEffectSolve(item.card_code, NetworkRpcFunc.instance.isServer);
        tributeEntity.Clear();

        cardCost = 0;
        SelectTributeMode = false;
    }

    public void SelectTribute(bool server, int entityID)
    {
        bool isMine = NetworkRpcFunc.instance.isServer == server;
        Entity entity;
        if(isMine)
        {
            entity = playerEntities.Find(x => x.id == entityID);
        }
        else
        {
            entity = opponentEntities.Find(x => x.id == entityID);
        }

        entity.canTribute = false;
        tributeEntity.Add(entity);

        if (cardCost <= tributeEntity.Count)
        {
            if(CardDatabase.instance.CardData(card_id).cardType.card_category == CardCategory.Monster)
            {
                TributeSummon(isMine, this.card_id);
            }
            else if(CardDatabase.instance.CardData(card_id).cardType.card_category == CardCategory.Magic)
            {
                TributeMagic(isMine, card_id);
            }
        }
    }
    #endregion


    #region Magic Card
    IEnumerator DelaySelectTarget()
    {
        yield return new WaitForSeconds(0.7f);
        effect_Target_Select_State = true;
    }

    // 마법카드 발동 (몬스터 소환시에도 불러오네)
    // 코드가 여기 말고 Effect에 있어야한것 같음, 아니면 GameManager를 하나 만들어야함
    public bool EffectTrigger(bool isMine, string card_id)
    {
        if (selectTile == null && isMine)
            return false;

        effectCard = CardDatabase.instance.CardData(card_id);

        if (effectSolver.EffectRequireExist(!isMine, playerEntities, opponentEntities, effectCard) == false)
            return false;
        this.card_id = card_id;
        cardCost = effectCard.cost;

        if (effectCard.ability.effect_Class == EffectClass.Activated)
        {
            if (isMine)
            {
                effectSolver.player_Activated_Abilities.Add(effectCard.ability);
            }
            else
            {
                effectSolver.opponent_Activated_Abilities.Add(effectCard.ability);
            }
            effectSolver.Activated_Effect(effectCard.ability, isMine, playerEntities, opponentEntities);
            gameLog.Log_Sorter(LogCategory.Magic, effectCard, isMine);
        }
        if (effectCard.cost == 0 || effectCard.cardType.card_category == CardCategory.Monster)
        {
            if(isMine)
            {
                GameManager.instance.localGamePlayerScript.CmdEffectSolve(effectCard.card_code, NetworkRpcFunc.instance.isServer);
            }
            return true;
        }
        else if(effectCard.cost > 0)
        {
            if (effectCard.cost <= tributeEntity.Count)
            {
                GameManager.instance.localGamePlayerScript.CmdEffectSolve(effectCard.card_code, NetworkRpcFunc.instance.isServer);
                return true;
            }

            if(effectCard.cost <= playerEntities.Count && isMine)
            {
                GameManager.instance.Confirmation("제물이 " + effectCard.cost + "마리 필요합니다.\n" + "사용\n하시겠습니까?");
                gameLog.Log_Sorter(LogCategory.Magic, effectCard, isMine);
                onOtherPanel = true;
                tributeMagic = true;
            }
        }
        return false;
    }

    public void EffectSolve(string card_id, bool server)
    {
        bool isMine = NetworkRpcFunc.instance.isServer == server;
        Card effectCard = CardDatabase.instance.CardData(card_id);

        switch(effectCard.ability.target.GetTarget())
        {
            case EffectTarget.TargetCard:
                if (isMine)
                {
                    StartCoroutine(DelaySelectTarget()); // SelectMagicTarget 변수를 딜레이주고 변경, jsg
                    GameManager.instance.Notification("타겟을\n선택해주세요");
                }
                break;
            case EffectTarget.RandomCard:
                if (playerEntities.Count != 0 || opponentEntities.Count != 0)
                {
                    if (isMine)
                    {
                        effectSolver.NonTargetEffectActive(effectCard, playerEntities, opponentEntities, NetworkRpcFunc.instance.isServer);

                        UpdateEntityState();
                    }
                }
                break;
            case EffectTarget.AllCards:
                effectSolver.NonTargetEffectActive(effectCard, playerEntities, opponentEntities, NetworkRpcFunc.instance.isServer);
                UpdateEntityState();
                break;
            case EffectTarget.TribeTarget:
                if (isMine)
                {
                    StartCoroutine(DelaySelectTarget()); // SelectMagicTarget 변수를 딜레이주고 변경, jsg
                    GameManager.instance.Notification(effectCard.TribeStr() + " 타겟을\n선택해주세요");
                }
                break;
            default:
                return;
        }

        gameLog.Log_Sorter(LogCategory.Magic, effectCard, isMine); // 로그 추가
    }

    public void Select_Effect_Target(int entityID, bool targetPlayer, bool server)
    {
        bool isMine = server == NetworkRpcFunc.instance.isServer; ;

        if (isMine == false)
            targetPlayer = !targetPlayer;

        var targetEntities = targetPlayer ? playerEntities : opponentEntities;
        
        Entity targetEntity = targetEntities.Find(x => x.id == entityID);

        if(targetEntity == null) 
            return;

        EffectTarget effectTarget = effectCard.ability.target.GetTarget();

        if (effectTarget == EffectTarget.PlayerCard && targetEntity.isMine == isMine)
            effect_Target_Select_State = false;
        else if (effectTarget == EffectTarget.OpponentCard && targetEntity.isMine != isMine)
            effect_Target_Select_State = false;
        else if (effectTarget == EffectTarget.TargetCard)
            effect_Target_Select_State = false;
        else if(effectTarget == EffectTarget.TribeTarget)
        {
            if (targetEntity.card.cardType.tribe == effectCard.cardType.tribe)
                effect_Target_Select_State = false;
            else
                GameManager.instance.Notification(effectCard.TribeStr() + " 타겟을\n선택해주세요");
        }
        if (effect_Target_Select_State == false)
        {
            gameLog.Log_Sorter(LogCategory.Effected ,targetEntity);
            effectCard.ability.effect.Resolve(targetEntity);
            UpdateEntityState();
            effectCard = null;
        }
    }

    // 랜덤 이펙트시 랜덤타겟 받는 함수인데.. 그냥 서버에서 랜덤으로 뿌리면 되지 않나?
    // 이거 봤을때 별 문제 없으면 밑에 두개 삭제 해도 되는것, 그 밑에가 두개 포함해서 작동
    public void OpponentTargetEffect(int entityID, string card_id, bool server)
    {
        bool isMine = server == NetworkRpcFunc.instance.isServer;
        Entity targetEntity = 
            isMine ? playerEntities.Find(x => x.id == entityID) : opponentEntities.Find(x => x.id == entityID);

        gameLog.Log_Sorter(LogCategory.Effected, targetEntity);
        effectSolver.ReceiveRandomEffect(targetEntity, card_id); // jsg
        UpdateEntityState();
    }

    public void PlayerTargetEffect(int entityID, string card_id, bool server)
    {
        bool isMine = server == NetworkRpcFunc.instance.isServer; ;
        Entity targetEntity;
        targetEntity = opponentEntities.Find(x => x.id == entityID);

        gameLog.Log_Sorter(LogCategory.Effected, targetEntity);
        effectSolver.ReceiveRandomEffect(targetEntity, card_id); // jsg
        UpdateEntityState();
    }

    public void RandomTargetEffect(bool targetPlayer, int entityIndex, string card_id, bool server)
    {
        bool isMine = server == NetworkRpcFunc.instance.isServer;

        targetPlayer = isMine ? targetPlayer : !targetPlayer;

        Entity targetEntity = 
            targetPlayer ? playerEntities[entityIndex] : opponentEntities[entityIndex];

        gameLog.Log_Sorter(LogCategory.Effected, targetEntity);
        effectSolver.ReceiveRandomEffect(targetEntity, card_id); // jsg
        
        UpdateEntityState();
    }

    // 몬스터 사망시 처리되는 함수
    public void UpdateEntityState()
    {
        List<Entity> destroiedEntity = new List<Entity>();
        foreach(var playerEntity in playerEntities)
        {
            if (playerEntity.isDie)
            {
                if(playerEntity.card.ability.effect_Class == EffectClass.Activated)
                {
                    effectSolver.ReverseEffect(playerEntity, playerEntities, opponentEntities);
                }
                MapManager.instance.mapData[playerEntity.coordinate.x, playerEntity.coordinate.y].tileState = TileState.empty;
                GoToGraveyard(playerEntity);
                destroiedEntity.Add(playerEntity);
            }
        }

        foreach (var opponentEntity in opponentEntities)
        {
            if (opponentEntity.isDie)
            {
                if(opponentEntity.card.ability.effect_Class == EffectClass.Activated)
                {
                    effectSolver.ReverseEffect(opponentEntity, playerEntities, opponentEntities);
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
        print(entity.isMine);
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

    // CardManager로 이동해야함 jsg, 카드 드로우가 아니라 5장 고정이면서 사용 안되는중
    // 마법 카드 효과 발동시 필드에 보여줬다 사라지는거..
    public void DropHandCard(bool isMine, string card_id)
    {
        Card card = CardDatabase.instance.CardData(card_id);
        Vector3 spawnPos;

        if (isMine)
        {
            spawnPos = myCardCreatePos.position;
        }
        else
        {
            spawnPos = otherCardCreatePos.position;
        }

        // 마법카드 이미지 따로 만들면 그거로 나오게 해야함 jsg
        var entityObject = Instantiate(entityPrefab, spawnPos, Utils.QI);
        if (!isMine)
            entityObject.GetComponent<Entity>().OppenentFeildCardColor(Color.red);
        var entity = entityObject.GetComponent<Entity>();
        entity.Setup(card);
        entity.isMine = isMine;
        entity.isDie = true;

        GoToGraveyard(entity);
        // 사용 용도가 애매한 함수라 로그가 나오면 안댐
        //gameLog.Log_Sorter(LogCategory.Drop , card, isMine); // 로그 추가 
        
        Destroy(gameObject.GetComponent<Entity>());
    }

    // 공격시 조준표시 나오게 하는 함수
    private void ShowTargetPicker(bool isShow)
    {
        if(TurnManager.instance.myTurn == false) { return; }

        TargetPicker.SetActive(isShow);

        if(targetPickEntity != null)
            TargetPicker.transform.position = targetPickEntity.transform.position;
        else if(selectTile != null && selectTile.tileState == TileState.opponentOutpost)
            TargetPicker.transform.position = selectTile.transform.position;
    }

    public void AttackableReset(bool isMine)
    {
        var targetEntities = isMine ? playerEntities : opponentEntities;
        targetEntities.ForEach(x => x.attackable = true);
    }
}
