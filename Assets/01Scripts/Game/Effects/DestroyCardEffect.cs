using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCardEntityEffect : CardDestroyEffect
{
    public override void Resolve(Entity entity)
    {
        entity.isDie = true;
    }

    public override void Resolve(SingleEntity entity)
    {
        entity.isDie = true;
    }
}
