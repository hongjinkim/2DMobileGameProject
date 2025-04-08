using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatManager : BasicSingleton<StatManager>
{

    public float WeaponDamage => Random.Range(2, 6);
}
