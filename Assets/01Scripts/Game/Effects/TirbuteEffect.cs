using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TirbuteEffect : TributeBaseEffect
{
    Tribe targetTribe;
    
    public override void Resolve(Entity entity)
    {

    }
}

public class TargetTributeEffect : TributeBaseEffect
{
    public override void Resolve(Entity entity)
    {

    }

    public override bool AreTargetsAvailable(Card tributeCard, Card targetCard)
    {

        return base.AreTargetsAvailable(tributeCard, targetCard);
    }
}
