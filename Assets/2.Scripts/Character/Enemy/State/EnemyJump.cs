using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyJump : State<EnemyControl>
{
    public override void Enter(EnemyControl entity)
    {
       
    }

    public override void Execute(EnemyControl entity)
    {
        
    }

    public override void FixedExecute(EnemyControl entity)
    {
        entity.Rigid.velocity = new Vector2(-1.0f * entity.State.Speed * Time.fixedDeltaTime * 50, 1.0f * entity.State.Speed * Time.fixedDeltaTime * 50);
    }

    public override void Exit(EnemyControl entity)
    {
    }
}
