using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCardEntityEffect : CardDestroyEffect
{
    // ���� ��� ����� �Ƹ��� �ı��� �ƴ϶� ������ �����°� �ǰ���.. 
    public override void Resolve(Entity entity)
    {
        entity.isDie = true;
    }
}
