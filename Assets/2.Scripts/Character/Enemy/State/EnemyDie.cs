using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDie : State<EnemyControl>
{
    public override void Enter(EnemyControl entity)
    {
        entity.Anim.SetBool("IsDead", true);
        entity.State.DieTimer = 0;
    }

    public override void Execute(EnemyControl entity)
    {
        //if (entity.State.DieTimer >= entity.State.DieTime)
            entity.DieDisable();
    }

    public override void Exit(EnemyControl entity)
    {
    }
}
