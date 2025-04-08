using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : State<EnemyControl>
{
    public override void Enter(EnemyControl entity)
    {
        entity.Anim.SetBool("IsAttacking", true);
        entity.Anim.speed = entity.AttackClip.length / entity.State.AttackTermTime;
    }

    public override void Execute(EnemyControl entity)
    {
        entity.Attack();
    } 

    public override void Exit(EnemyControl entity)
    {
        entity.Anim.SetBool("IsAttacking", false);
        entity.Anim.speed = 1;
        entity.State.AttackTermTimer = 0;
    }
}
