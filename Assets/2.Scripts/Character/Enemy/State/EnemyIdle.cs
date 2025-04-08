using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdle : State<EnemyControl>
{
    public override void Enter(EnemyControl entity)
    {
        entity.Anim.SetBool("IsIdle", true);
    }

    public override void Execute(EnemyControl entity)
    {
        if (entity.State.NoneMove == true) return;

        //타켓이 없는 경우 타겟 서치
        if (entity.Target == null) { entity.Target = entity.NearPlayer(); }

        //타겟이 있는 경우
        else
        {
            entity.Target = entity.NearPlayer();
            float Distance = Vector3.Distance(entity.CenterPoint.position, entity.Target.CenterPoint.position);

            //사거리 안에 있는 경우
            if (Distance < entity.State.Range)
            {
                //공격 가능하면 공격발동
                if (entity.State.AttackTermTimer >= entity.State.AttackTermTime)
                {
                    entity.ChangeState(EActType.Attack);
                }
            }
            else
            {
                entity.ChangeState(EActType.Move);
            }
        }
    }

    public override void Exit(EnemyControl entity)
    {
        entity.Anim.SetBool("IsIdle", false);
    }
}
