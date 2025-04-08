using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXPoolManager : PoolManager<FXPoolManager, EFXPoolType>
{
    public GameObject PopDamageText(Vector3 position, AttackInfo attackInfo, float damage)
    {
        var obj = Pop(EFXPoolType.DamageText);
        obj.transform.position = position;
        obj.GetComponent<DamageFloating>().SetDamage(attackInfo, damage);
        return obj;
    }
}
