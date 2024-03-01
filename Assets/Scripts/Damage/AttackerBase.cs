using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AttackerType
{
    Default,
    HitScan,
    Projectile,
    Melee
}
public abstract class AttackerBase
{
    public AttackerType attackerType;
}
