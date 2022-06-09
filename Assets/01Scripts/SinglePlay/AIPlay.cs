using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlay : MonoBehaviour
{
    public void AI_Card_Move(List<SingleEntity> playerEntities, List<SingleEntity> AI_Entities)
    {
        Coordinate entity_Pos;
        int distance = 0;
        SingleEntity moveEntity;

        if (playerEntities.Count > 0)
        {
            int random_Index = Random.Range(0, AI_Entities.Count - 1);
            foreach (var entity in playerEntities)
            {
                if (distance > Distance_Cal(AI_Entities[random_Index].coordinate, entity.coordinate))
                {
                    distance = Distance_Cal(AI_Entities[random_Index].coordinate, entity.coordinate);
                    entity_Pos = entity.coordinate;
                    moveEntity = AI_Entities[random_Index];
                }
            }
        }
    }

    int Distance_Cal(Coordinate coordinate1, Coordinate coordinate2)
    {
        int x, y;
        x = System.Math.Abs(coordinate1.x - coordinate2.x);
        y = System.Math.Abs(coordinate1.y - coordinate2.y);

        return (x + y);
    }
}
