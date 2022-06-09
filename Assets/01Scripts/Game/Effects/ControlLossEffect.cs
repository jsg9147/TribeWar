using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlLossEffect : ControllEffect
{
    public override bool AreTargetsAvailable(Card sourceCard, Target target)
    {
        return base.AreTargetsAvailable(sourceCard, target);
    }
    public override void Resolve(Entity entity)
    {
        originBelong = entity.belong;
        entity.belong = EntityBelong.AI;
        entity.isMine = false;

        base.Resolve(entity);
    }

    public override void Reverse(Entity entity)
    {
        entity.belong = originBelong;
        if (originBelong == EntityBelong.Player)
        {
            entity.isMine = true;
        }

        base.Reverse(entity);
    }
}
