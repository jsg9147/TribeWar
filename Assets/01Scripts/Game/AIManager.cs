using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    [SerializeField] CardManager cardManager;
    [SerializeField] EntityManager entityManager;
    [SerializeField] MapManager mapManager;
    [SerializeField] GameManager gameManager;

    CardMove cardMove = new CardMove();

    [Header("SingleMode")]
    public bool SingleMode;

    public List<Hand> hands;

    public void HandRefresh()
    {
        
    }
    public void Entity_Active(List<Entity> allEntities)
    {
        List<Entity> entities = new List<Entity>();
        foreach (var entity in allEntities)
        {
            if (entity.belong == EntityBelong.AI)
            {
                entities.Add(entity);
            }
        }

        int randomIndex = Random.Range(0, entities.Count - 1);
        
        for (int i = 0; i < entities.Count; i++)
        {
            Random_Active(entities[randomIndex]);
            randomIndex = Random.Range(0, entities.Count - 1);
        }
    }

    // 아마 이동 불가능 할때도 그냥 한번 행동한것으로 해서 다음 행동 안할꺼임, 근데 그냥 그것도 나쁘진 않을꺼 같아서 수정안함
    // 만약 문제가 있다면 여기서부터 손보면 될것임
    void Random_Active(Entity entity)
    {
        List<Coordinate> canMovePos = cardMove.Can_Attack_Position(entity);
        int randomIndex = Random.Range(0, canMovePos.Count-1);
        List<Entity> targetEntities = new List<Entity>();
        List<Outpost> targetOutposts = new List<Outpost>();

        foreach (var pos in canMovePos)
        {
            if (mapManager.GetTile(pos).onEntity != null || mapManager.GetTile(pos).outpost.life > 0)
            {
                if (mapManager.GetTile(pos).onEntity != null && mapManager.GetTile(pos).onEntity.belong != EntityBelong.AI)
                {
                    targetEntities.Add(mapManager.GetTile(pos).onEntity);
                }

                else if (mapManager.GetTile(pos).outpost.life > 0)
                {
                    targetOutposts.Add(mapManager.GetTile(pos).outpost);
                }
            }
        }
        if (SingleMode == false)
        {
            if (targetOutposts.Count > 0 && targetEntities.Count > 0)
            {
                bool randomBool = (Random.value > 0.5f);
                if (randomBool)
                {
                    Outpost target = targetOutposts[Random.Range(0, targetEntities.Count - 1)];
                    gameManager.localGamePlayerScript.CmdOutpostAttack(entity.id, target.coordinate.vector3Pos, true);
                }
                else
                {
                    Entity target = targetEntities[Random.Range(0, targetEntities.Count - 1)];
                    gameManager.localGamePlayerScript.CmdAttack(entity.id, target.id, true);
                }
            }

            else if (targetOutposts.Count > 0)
            {
                Outpost target = targetOutposts[Random.Range(0, targetEntities.Count - 1)];
                gameManager.localGamePlayerScript.CmdOutpostAttack(entity.id, target.coordinate.vector3Pos, true);
            }

            else if (targetEntities.Count > 0)
            {
                Entity target = targetEntities[Random.Range(0, targetEntities.Count - 1)];
                gameManager.localGamePlayerScript.CmdAttack(entity.id, target.id, true);
            }
            else
            {
                gameManager.localGamePlayerScript.CmdCardMove(entity.id, false, canMovePos[randomIndex].vector3Pos, true);
            }
        }
        
        else
        {
            entityManager.CardMove(entity.id, false, canMovePos[randomIndex].vector3Pos, false);
        }
    }

    void Near_Enermy_Search()
    {
    
    }

    void Use_Hand()
    {
    
    }
}
