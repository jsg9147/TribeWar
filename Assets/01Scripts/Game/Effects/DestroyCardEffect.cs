using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCardEntityEffect : CardDestroyEffect
{
    // 묘지 기능 생기면 아마도 파괴가 아니라 묘지로 보내는게 되겠지.. 
    public override void Resolve(Entity entity)
    {
        entity.isDie = true;
    }
}
